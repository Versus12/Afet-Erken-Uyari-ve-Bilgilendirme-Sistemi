using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Data;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Business
{
    /// <summary>
    /// Toplu e-posta bildirimi (Hafta 9).
    /// - Uyarı yayınlanınca kuyruğa alınan e-postaları sırayla gönderir.
    /// - Hız sınırı: iki gönderim arasında en az 250 ms.
    /// - Hata yönetimi: geçici hatada (zaman aşımı, 429, 5xx) en fazla 3 deneme, aralarında artan bekleme;
    ///   kalıcı hatada hemen "Başarısız". API anahtarı yanlışsa kalan gönderimler durdurulur.
    /// - Bir e-postanın başarısız olması uyarının yayınını geri almaz; uyarı uygulama içinde yine görünür.
    /// </summary>
    public class BildirimManager
    {
        public const int MaksimumDeneme = 3;

        private static readonly HizSinirlayici Sinirlayici = new HizSinirlayici(TimeSpan.FromMilliseconds(250));

        private readonly BildirimRepository _repo = new BildirimRepository();
        private readonly UyariRepository _uyariRepo = new UyariRepository();
        private readonly IEpostaGonderici _gonderici;

        public BildirimManager() : this(EpostaGondericiSecici.Olustur()) { }

        /// <summary>Test için farklı bir gönderici verilebilir.</summary>
        public BildirimManager(IEpostaGonderici gonderici)
        {
            _gonderici = gonderici;
        }

        public string SaglayiciAdi { get { return _gonderici.Ad; } }

        /// <summary>Uyarının kuyruktaki e-postalarını gönderir.</summary>
        public BildirimSonucu UyariIcinGonder(int uyariId)
        {
            Oturum.YoneticiGerekli();
            return BekleyenleriGonder(uyariId);
        }

        /// <summary>Başarısız gönderimleri yeniden dener.</summary>
        public BildirimSonucu BasarisizlariYenidenDene(int uyariId)
        {
            Oturum.YoneticiGerekli();
            _repo.BasarisizlariKuyrugaAl(uyariId);
            return BekleyenleriGonder(uyariId);
        }

        public List<Bildirim> Kayitlar(int uyariId)
        {
            Oturum.YoneticiGerekli();
            return _repo.Listele(uyariId);
        }

        private BildirimSonucu BekleyenleriGonder(int uyariId)
        {
            BildirimSonucu sonuc = new BildirimSonucu { SaglayiciAdi = _gonderici.Ad };
            List<Bildirim> bekleyenler = _repo.Bekleyenler(uyariId);
            if (bekleyenler.Count == 0) return sonuc;

            Uyari uyari = _uyariRepo.Getir(uyariId);
            if (uyari == null) throw new IsKuraliHatasi("Uyarı bulunamadı.");

            string durdurmaNedeni = null;
            foreach (Bildirim b in bekleyenler)
            {
                sonuc.Toplam++;

                if (durdurmaNedeni != null)
                {
                    _repo.SonucYaz(b.BildirimId, BildirimDurumlari.Basarisiz, b.DenemeSayisi, durdurmaNedeni, null);
                    sonuc.Basarisiz++;
                    continue;
                }

                EpostaMesaji mesaj = MesajOlustur(uyari, b);
                int deneme = b.DenemeSayisi;
                for (int i = 1; ; i++)
                {
                    deneme++;
                    try
                    {
                        Sinirlayici.Bekle();
                        string mesajId = _gonderici.Gonder(mesaj);
                        string durum = _gonderici.GercekGonderim ? BildirimDurumlari.Gonderildi : BildirimDurumlari.DenemeModu;
                        _repo.SonucYaz(b.BildirimId, durum, deneme, null, mesajId);
                        if (_gonderici.GercekGonderim) sonuc.Gonderilen++; else sonuc.DenemeModu++;
                        break;
                    }
                    catch (BildirimGonderimHatasi h)
                    {
                        if (h.TekrarDenenebilir && i < MaksimumDeneme)
                        {
                            // Artan bekleme: 1 sn, 2 sn ... (servis Retry-After verdiyse o kadar, en fazla 10 sn)
                            TimeSpan bekle = h.Bekleme ?? TimeSpan.FromSeconds(i);
                            if (bekle > TimeSpan.FromSeconds(10)) bekle = TimeSpan.FromSeconds(10);
                            Thread.Sleep(bekle);
                            continue;
                        }

                        _repo.SonucYaz(b.BildirimId, BildirimDurumlari.Basarisiz, deneme, h.Message, null);
                        sonuc.Basarisiz++;
                        if (h.YapilandirmaHatasi) durdurmaNedeni = "Gönderim durduruldu: " + h.Message;
                        break;
                    }
                }
            }
            return sonuc;
        }

        /// <summary>Uyarıdan sade bir e-posta metni üretir.</summary>
        public static EpostaMesaji MesajOlustur(Uyari u, Bildirim b)
        {
            string seviye = UyariSeviyeleri.Ad(u.Seviye).ToUpper(new System.Globalization.CultureInfo("tr-TR"));
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Merhaba " + b.KullaniciAdi + ",");
            sb.AppendLine();
            sb.AppendLine("Abone olduğunuz bölge için yeni bir " + seviye + " seviyesinde uyarı yayınlandı.");
            sb.AppendLine();
            sb.AppendLine("Başlık   : " + u.Baslik);
            sb.AppendLine("Afet türü: " + u.AfetTuru);
            sb.AppendLine("Bölgeler : " + u.HedefBolgeler);
            sb.AppendLine("Yayın    : " + u.YayinTarihi.ToString("dd.MM.yyyy HH:mm"));
            sb.AppendLine("Geçerlilik: " + (u.BitisTarihi.HasValue ? u.BitisTarihi.Value.ToString("dd.MM.yyyy HH:mm") : "Süresiz"));
            sb.AppendLine();
            sb.AppendLine(u.Mesaj);
            sb.AppendLine();
            sb.AppendLine("Hazırlık rehberi ve toplanma alanları için uygulamaya giriş yapabilirsiniz.");
            sb.AppendLine("Bu e-postayı bildirim kanalı olarak E-posta seçtiğiniz için aldınız; tercihinizi Aboneliklerim ekranından değiştirebilirsiniz.");

            return new EpostaMesaji
            {
                Alici = b.Alici,
                Konu = "[" + seviye + "] " + u.Baslik + " - Afet Erken Uyarı",
                Govde = sb.ToString()
            };
        }
    }
}
