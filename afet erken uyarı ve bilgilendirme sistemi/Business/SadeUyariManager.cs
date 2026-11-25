using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Data;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Business
{
    /// <summary>
    /// Hafta 10 - Yapay zekâ bileşeni: duyuru özetleme ve bölgeye özel sade uyarı.
    /// Akış:
    ///  1) Girdi doğrulanır (yalnızca yönetici, 20-4000 karakter kaynak, en az bir bölge).
    ///  2) ANTHROPIC_API_KEY varsa Claude'dan JSON taslak istenir ve CiktiDogrulayici ile denetlenir.
    ///  3) AI yoksa / hata verirse / çıktı doğrulanamazsa kural tabanlı taslak üretilir (yine doğrulanır).
    ///  4) Her deneme istem + ham çıktı + doğrulama notlarıyla YapayZekaKayitlari tablosuna yazılır (AI günlüğü).
    ///  5) Taslak YAYINLANMAZ; yönetici okuyup düzenledikten sonra normal yayın akışıyla yayınlar.
    /// </summary>
    public class SadeUyariManager
    {
        public const int KaynakEnAz = 20;
        public const int KaynakEnCok = 4000;

        private readonly YapayZekaRepository _repo = new YapayZekaRepository();
        private readonly BolgeRepository _bolgeRepo = new BolgeRepository();
        private readonly IYapayZekaIstemcisi _ai;

        /// <summary>Ortam değişkeninde anahtar varsa Claude kullanılır, yoksa yalnızca kural tabanlı yöntem.</summary>
        public SadeUyariManager() : this(ClaudeIstemcisi.OrtamdanOlustur()) { }

        /// <summary>Test için farklı (sahte) bir AI istemcisi verilebilir; null ise AI kullanılmaz.</summary>
        public SadeUyariManager(IYapayZekaIstemcisi ai)
        {
            _ai = ai;
        }

        public bool YapayZekaHazir { get { return _ai != null; } }

        public string YontemAdi
        {
            get { return _ai != null ? _ai.Ad : "Kural tabanlı (" + ClaudeIstemcisi.AnahtarDegiskeni + " tanımlı değil)"; }
        }

        public SadeUyariTaslagi TaslakUret(string kaynakMetin, List<int> bolgeIdleri, string afetTuruOnerisi)
        {
            Kullanici yonetici = Oturum.YoneticiGerekli();

            kaynakMetin = (kaynakMetin ?? "").Trim();
            if (kaynakMetin.Length < KaynakEnAz || kaynakMetin.Length > KaynakEnCok)
                throw new IsKuraliHatasi("Duyuru metni " + KaynakEnAz + "-" + KaynakEnCok + " karakter olmalıdır.");
            List<Bolge> bolgeler = _bolgeRepo.Listele().Where(b => (bolgeIdleri ?? new List<int>()).Contains(b.BolgeId)).ToList();
            if (bolgeler.Count == 0)
                throw new IsKuraliHatasi("Taslak bölgeye özel hazırlanır; önce en az bir hedef bölge seçiniz.");

            string bolgeMetni = string.Join(", ", bolgeler.Select(b => b.BolgeAdi));
            string kullaniciIstemi = SadeUyariIstemi.Kullanici(kaynakMetin, bolgeler, afetTuruOnerisi);
            string istem = "[SİSTEM]\n" + SadeUyariIstemi.Sistem + "\n\n[KULLANICI]\n" + kullaniciIstemi;
            string aiNotu;

            if (_ai != null)
            {
                Stopwatch sure = Stopwatch.StartNew();
                string ham = null;
                SadeUyariTaslagi aiTaslak;
                try
                {
                    ham = _ai.Tamamla(SadeUyariIstemi.Sistem, kullaniciIstemi, SadeUyariIstemi.Sema());
                    aiTaslak = CiktiDogrulayici.JsonuDogrula(ham, kaynakMetin, bolgeler);
                }
                catch (YapayZekaHatasi ex)
                {
                    aiTaslak = new SadeUyariTaslagi();
                    aiTaslak.Hatalar.Add(ex.Message);
                }
                sure.Stop();
                aiTaslak.Saglayici = _ai.Ad;
                aiTaslak.YapayZekaKullanildi = true;
                Kaydet(yonetici, aiTaslak, bolgeMetni, kaynakMetin, istem, ham, (int)sure.ElapsedMilliseconds);

                if (aiTaslak.Gecerli) return aiTaslak;
                aiNotu = "Yapay zekâ taslağı kullanılamadı: " + string.Join(" ", aiTaslak.Hatalar) +
                         " Aşağıdaki taslak kural tabanlı yöntemle üretildi.";
            }
            else
            {
                aiNotu = "Yapay zekâ anahtarı (" + ClaudeIstemcisi.AnahtarDegiskeni + ") tanımlı olmadığı için taslak kural tabanlı yöntemle üretildi.";
            }

            SadeUyariTaslagi kural = KuralTabanliOzetleyici.Olustur(kaynakMetin, bolgeler, afetTuruOnerisi);
            kural.Not = aiNotu;
            Kaydet(yonetici, kural, bolgeMetni, kaynakMetin, "[KURAL TABANLI - istem kullanılmadı]\n" + istem, null, 0);
            return kural;
        }

        /// <summary>AI günlüğü (son kayıtlar).</summary>
        public List<YapayZekaKaydi> Gunluk(int adet = 50)
        {
            Oturum.YoneticiGerekli();
            return _repo.SonKayitlar(adet);
        }

        private void Kaydet(Kullanici k, SadeUyariTaslagi t, string bolgeler, string kaynak, string istem, string ham, int sureMs)
        {
            List<string> notlar = t.Hatalar.Select(h => "HATA: " + h).Concat(t.Uyarilar.Select(u => "UYARI: " + u)).ToList();
            _repo.Ekle(new YapayZekaKaydi
            {
                KullaniciId = k.Id,
                Saglayici = t.Saglayici,
                HedefBolgeler = bolgeler,
                KaynakMetin = kaynak,
                Istem = istem,
                HamCikti = ham,
                Baslik = string.IsNullOrEmpty(t.Baslik) ? null : t.Baslik,
                Mesaj = string.IsNullOrEmpty(t.Mesaj) ? null : t.Mesaj,
                OnerilenSeviye = t.Seviye >= 1 && t.Seviye <= 4 ? t.Seviye : (int?)null,
                Gecerli = t.Gecerli,
                DogrulamaNotlari = notlar.Count == 0 ? "Doğrulamadan geçti." : string.Join(Environment.NewLine, notlar),
                SureMs = sureMs > 0 ? sureMs : (int?)null
            });
        }
    }
}
