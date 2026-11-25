using System;
using System.Collections.Generic;
using System.Linq;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Data;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Business
{
    /// <summary>
    /// Toplanma alanı iş kuralları.
    /// Hafta 11 - Toplanma Alanı Haritası Modülü:
    /// - Kural 1: Haritada gösterilebilmesi için her alanın koordinatı ZORUNLUDUR (yeni kayıt ve güncelleme).
    /// - Kural 2: Alan, bağlı olduğu bölgenin merkezine en fazla <see cref="BolgeyeEnFazlaKm"/> km uzaklıkta olmalıdır
    ///   (yanlış bölge seçimini ya da enlem/boylamın ters yazılmasını yakalar).
    /// - En yakın alan: Haversine mesafesine göre sıralanır; koordinatı olmayan eski kayıtlar hesaba katılmaz ve sayısı bildirilir.
    /// - En yakın alan <see cref="UzakUyariKm"/> km'den uzaksa kullanıcı uyarılır.
    /// </summary>
    public class ToplanmaAlaniManager
    {
        public const double BolgeyeEnFazlaKm = 50;
        public const double UzakUyariKm = 10;
        public const int VarsayilanAdet = 3;

        private readonly ToplanmaAlaniRepository _repo = new ToplanmaAlaniRepository();
        private readonly BolgeRepository _bolgeRepo = new BolgeRepository();
        private readonly AbonelikRepository _abonelikRepo = new AbonelikRepository();

        public List<ToplanmaAlani> Listele(int? bolgeId)
        {
            Oturum.GirisGerekli();
            return _repo.Listele(bolgeId);
        }

        public void Kaydet(ToplanmaAlani t)
        {
            Oturum.YoneticiGerekli();

            t.AlanAdi = (t.AlanAdi ?? "").Trim();
            t.Adres = (t.Adres ?? "").Trim();

            if (t.BolgeId <= 0) throw new IsKuraliHatasi("Bölge seçiniz.");
            if (t.AlanAdi.Length < 3 || t.AlanAdi.Length > 150) throw new IsKuraliHatasi("Alan adı 3-150 karakter olmalıdır.");
            if (t.Adres.Length < 5 || t.Adres.Length > 300) throw new IsKuraliHatasi("Adres 5-300 karakter olmalıdır.");
            if (t.Kapasite <= 0) throw new IsKuraliHatasi("Kapasite 0'dan büyük olmalıdır.");
            if (!t.Enlem.HasValue || !t.Boylam.HasValue)
                throw new IsKuraliHatasi("Haritada gösterilebilmesi için enlem ve boylam zorunludur. \"Adresten Konum Bul\" ile doldurabilirsiniz.");
            KonumHesap.Dogrula(t.Enlem.Value, t.Boylam.Value);

            Bolge bolge = _bolgeRepo.Listele().FirstOrDefault(b => b.BolgeId == t.BolgeId);
            if (bolge == null) throw new IsKuraliHatasi("Seçilen bölge bulunamadı.");
            if (bolge.Enlem.HasValue && bolge.Boylam.HasValue)
            {
                double km = KonumHesap.MesafeKm(bolge.Enlem.Value, bolge.Boylam.Value, t.Enlem.Value, t.Boylam.Value);
                if (km > BolgeyeEnFazlaKm)
                    throw new IsKuraliHatasi("Alan, " + bolge.BolgeAdi + " bölgesinin merkezine " + km.ToString("0") +
                                             " km uzakta. En fazla " + BolgeyeEnFazlaKm + " km olabilir; bölgeyi ya da koordinatı kontrol ediniz.");
            }

            if (t.AlanId > 0) _repo.Guncelle(t);
            else t.AlanId = _repo.Ekle(t);
        }

        public void Sil(int alanId)
        {
            Oturum.YoneticiGerekli();
            _repo.Sil(alanId);
        }

        /// <summary>
        /// Verilen konuma en yakın toplanma alanları (Hafta 11).
        /// </summary>
        /// <param name="sadeceAboneliklerim">true ise yalnızca kullanıcının abone olduğu bölgelerdeki alanlar (abonelik modülüyle entegrasyon).</param>
        public EnYakinSonucu EnYakinlar(decimal enlem, decimal boylam, string konumAciklamasi, bool sadeceAboneliklerim, int adet = VarsayilanAdet)
        {
            Kullanici k = Oturum.GirisGerekli();
            KonumHesap.Dogrula(enlem, boylam);
            if (adet < 1 || adet > 20) throw new IsKuraliHatasi("Gösterilecek alan sayısı 1-20 olmalıdır.");

            List<ToplanmaAlani> alanlar = _repo.Listele(null);
            if (sadeceAboneliklerim)
            {
                HashSet<int> aboneBolgeler = new HashSet<int>(_abonelikRepo.KullaniciAbonelikleri(k.Id).Select(a => a.BolgeId));
                alanlar = alanlar.Where(a => aboneBolgeler.Contains(a.BolgeId)).ToList();
            }

            EnYakinSonucu sonuc = new EnYakinSonucu
            {
                Enlem = enlem,
                Boylam = boylam,
                KonumAciklamasi = string.IsNullOrWhiteSpace(konumAciklamasi) ? "Seçilen konum" : konumAciklamasi,
                KonumsuzAlanSayisi = alanlar.Count(a => !a.Enlem.HasValue || !a.Boylam.HasValue)
            };

            sonuc.Alanlar = alanlar
                .Where(a => a.Enlem.HasValue && a.Boylam.HasValue)
                .Select(a =>
                {
                    double km = KonumHesap.MesafeKm(enlem, boylam, a.Enlem.Value, a.Boylam.Value);
                    return new YakinAlan { Alan = a, MesafeKm = Math.Round(km, 2), YurumeDakika = KonumHesap.YurumeDakika(km) };
                })
                .OrderBy(y => y.MesafeKm)
                .ThenByDescending(y => y.Alan.Kapasite)   // eşit uzaklıkta büyük kapasiteli alan önce
                .Take(adet)
                .ToList();

            if (sonuc.Alanlar.Count == 0)
                sonuc.Uyari = sadeceAboneliklerim
                    ? "Abone olduğunuz bölgelerde koordinatı kayıtlı toplanma alanı yok. Filtreyi kaldırıp tekrar deneyiniz."
                    : "Koordinatı kayıtlı toplanma alanı yok.";
            else if (sonuc.Alanlar[0].MesafeKm > UzakUyariKm)
                sonuc.Uyari = "En yakın toplanma alanı " + sonuc.Alanlar[0].MesafeKm.ToString("0") +
                              " km uzakta. Yakınınızda kayıtlı alan yok; yerel yönetimin duyurularını takip ediniz.";
            return sonuc;
        }

        /// <summary>Bir bölgenin merkezinden en yakın alanlar (kullanıcı konumunu bilmiyorsa).</summary>
        public EnYakinSonucu BolgeMerkezindenEnYakinlar(int bolgeId, bool sadeceAboneliklerim)
        {
            Oturum.GirisGerekli();
            Bolge b = _bolgeRepo.Listele().FirstOrDefault(x => x.BolgeId == bolgeId);
            if (b == null) throw new IsKuraliHatasi("Bölge seçiniz.");
            if (!b.Enlem.HasValue || !b.Boylam.HasValue) throw new IsKuraliHatasi(b.BolgeAdi + " bölgesinin merkez koordinatı kayıtlı değil.");
            return EnYakinlar(b.Enlem.Value, b.Boylam.Value, b.BolgeAdi + " (bölge merkezi)", sadeceAboneliklerim);
        }

        /// <summary>Yazılan adresten (Hafta 9 harita API'si ile) en yakın alanlar.</summary>
        public EnYakinSonucu AdrestenEnYakinlar(string adres, bool sadeceAboneliklerim)
        {
            KonumSonucu konum = new HaritaServisi().AdresBul(adres);
            return EnYakinlar(konum.Enlem, konum.Boylam, konum.BulunanAdres, sadeceAboneliklerim);
        }
    }
}
