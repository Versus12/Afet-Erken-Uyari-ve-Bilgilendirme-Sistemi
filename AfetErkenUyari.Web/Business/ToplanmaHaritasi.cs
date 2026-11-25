using AfetErkenUyari.Web.Data;
using AfetErkenUyari.Web.Entities;

namespace AfetErkenUyari.Web.Business;

// =====================================================================================
//  Hafta 11 · İleri Modül: Toplanma Alanı Haritası (en yakın alan + harita gösterimi)
//  (Masaüstü uygulamasındaki KonumHesap.cs ve ToplanmaAlaniManager.cs ile aynı mantık.)
// =====================================================================================

/// <summary>
/// İki nokta arasındaki kuş uçuşu mesafe — Haversine formülü:
///   a = sin²(Δφ/2) + cos φ1 · cos φ2 · sin²(Δλ/2),  d = 2 · R · atan2(√a, √(1−a)),  R = 6371 km.
/// </summary>
public static class KonumHesap
{
    public const double DunyaYaricapiKm = 6371.0;
    public const double YurumeHiziKmSa = 4.5;

    public static double MesafeKm(double enlem1, double boylam1, double enlem2, double boylam2)
    {
        double fi1 = Radyan(enlem1), fi2 = Radyan(enlem2);
        double dFi = Radyan(enlem2 - enlem1), dLambda = Radyan(boylam2 - boylam1);
        double a = Math.Sin(dFi / 2) * Math.Sin(dFi / 2) +
                   Math.Cos(fi1) * Math.Cos(fi2) * Math.Sin(dLambda / 2) * Math.Sin(dLambda / 2);
        return 2 * DunyaYaricapiKm * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }

    public static double MesafeKm(decimal enlem1, decimal boylam1, decimal enlem2, decimal boylam2) =>
        MesafeKm((double)enlem1, (double)boylam1, (double)enlem2, (double)boylam2);

    /// <summary>Kuş uçuşu mesafeye göre kaba yürüme süresi (dakika).</summary>
    public static int YurumeDakika(double km) => (int)Math.Ceiling(km / YurumeHiziKmSa * 60);

    public static void Dogrula(decimal enlem, decimal boylam)
    {
        if (enlem is < -90 or > 90) throw new IsKuraliHatasi("Enlem -90 ile 90 arasında olmalıdır.");
        if (boylam is < -180 or > 180) throw new IsKuraliHatasi("Boylam -180 ile 180 arasında olmalıdır.");
    }

    private static double Radyan(double derece) => derece * Math.PI / 180.0;
}

/// <summary>En yakın alan hesabının bir satırı.</summary>
public record YakinAlan(ToplanmaAlani Alan, double MesafeKm, int YurumeDakika);

/// <summary>En yakın alan aramasının sonucu.</summary>
public class EnYakinSonucu
{
    public decimal Enlem { get; set; }
    public decimal Boylam { get; set; }
    public string KonumAciklamasi { get; set; } = "";
    public List<YakinAlan> Alanlar { get; set; } = new();
    /// <summary>Koordinatı girilmediği için hesaba katılamayan alan sayısı.</summary>
    public int KonumsuzAlanSayisi { get; set; }
    public string Uyari { get; set; } = "";
}

/// <summary>
/// Toplanma alanı iş kuralları.
/// Hafta 11:
/// - Kural 1: haritada gösterilebilmesi için her alanın koordinatı ZORUNLUDUR.
/// - Kural 2: alan, bağlı olduğu bölgenin merkezine en fazla 50 km uzaklıkta olmalıdır.
/// - En yakın alan: Haversine mesafesine göre sıralanır; koordinatsız kayıtlar hesaba katılmaz ve sayısı bildirilir.
/// - En yakın alan 10 km'den uzaksa kullanıcı uyarılır.
/// </summary>
public class ToplanmaAlaniManager(ToplanmaAlaniRepository repo, AbonelikRepository abonelikRepo, BolgeRepository bolgeRepo,
                                  HaritaServisi harita)
{
    public const double BolgeyeEnFazlaKm = 50;
    public const double UzakUyariKm = 10;
    public const int VarsayilanAdet = 3;

    /// <summary>sadeceAboneliklerim: abonelik modülündeki bölgelerle filtreler (modül entegrasyonu).</summary>
    public List<ToplanmaAlani> Listele(AktifKullanici kullanici, int? bolgeId, bool sadeceAboneliklerim)
    {
        var liste = repo.Listele(bolgeId);
        if (sadeceAboneliklerim)
        {
            var aboneBolgeler = abonelikRepo.KullaniciAbonelikleri(kullanici.Id).Select(a => a.BolgeId).ToHashSet();
            liste = liste.Where(t => aboneBolgeler.Contains(t.BolgeId)).ToList();
        }
        return liste;
    }

    public ToplanmaAlani? Getir(int alanId) => repo.Getir(alanId);

    public void Kaydet(AktifKullanici kullanici, ToplanmaAlani t)
    {
        kullanici.YoneticiOlmali();
        if (t.BolgeId <= 0) throw new IsKuraliHatasi("Bölge seçiniz.");
        t.AlanAdi = Dogrula.Metin(t.AlanAdi, "Alan adı", 3, 150);
        t.Adres = Dogrula.Metin(t.Adres, "Adres", 5, 300);
        if (t.Kapasite <= 0) throw new IsKuraliHatasi("Kapasite 0'dan büyük olmalıdır.");
        if (t.Enlem is not { } enlem || t.Boylam is not { } boylam)
            throw new IsKuraliHatasi("Haritada gösterilebilmesi için enlem ve boylam zorunludur. \"Adresten konum bul\" ile doldurabilirsiniz.");
        KonumHesap.Dogrula(enlem, boylam);

        var bolge = bolgeRepo.Getir(t.BolgeId) ?? throw new IsKuraliHatasi("Seçilen bölge bulunamadı.");
        if (bolge.Enlem is { } be && bolge.Boylam is { } bb)
        {
            double km = KonumHesap.MesafeKm(be, bb, enlem, boylam);
            if (km > BolgeyeEnFazlaKm)
                throw new IsKuraliHatasi($"Alan, {bolge.BolgeAdi} bölgesinin merkezine {km:0} km uzakta. " +
                                         $"En fazla {BolgeyeEnFazlaKm} km olabilir; bölgeyi ya da koordinatı kontrol ediniz.");
        }

        if (t.AlanId > 0) repo.Guncelle(t);
        else t.AlanId = repo.Ekle(t);
    }

    public void Sil(AktifKullanici kullanici, int alanId)
    {
        kullanici.YoneticiOlmali();
        repo.Sil(alanId);
    }

    /// <summary>Verilen konuma en yakın toplanma alanları.</summary>
    public EnYakinSonucu EnYakinlar(AktifKullanici kullanici, decimal enlem, decimal boylam, string? konumAciklamasi,
                                    bool sadeceAboneliklerim, int adet = VarsayilanAdet)
    {
        KonumHesap.Dogrula(enlem, boylam);
        if (adet is < 1 or > 20) throw new IsKuraliHatasi("Gösterilecek alan sayısı 1-20 olmalıdır.");

        var alanlar = Listele(kullanici, null, sadeceAboneliklerim);
        var sonuc = new EnYakinSonucu
        {
            Enlem = enlem,
            Boylam = boylam,
            KonumAciklamasi = string.IsNullOrWhiteSpace(konumAciklamasi) ? "Seçilen konum" : konumAciklamasi,
            KonumsuzAlanSayisi = alanlar.Count(a => a.Enlem is null || a.Boylam is null),
            Alanlar = alanlar
                .Where(a => a.Enlem.HasValue && a.Boylam.HasValue)
                .Select(a =>
                {
                    double km = KonumHesap.MesafeKm(enlem, boylam, a.Enlem!.Value, a.Boylam!.Value);
                    return new YakinAlan(a, Math.Round(km, 2), KonumHesap.YurumeDakika(km));
                })
                .OrderBy(y => y.MesafeKm)
                .ThenByDescending(y => y.Alan.Kapasite)   // eşit uzaklıkta büyük kapasiteli alan önce
                .Take(adet)
                .ToList()
        };

        if (sonuc.Alanlar.Count == 0)
            sonuc.Uyari = sadeceAboneliklerim
                ? "Abone olduğunuz bölgelerde koordinatı kayıtlı toplanma alanı yok. Filtreyi kaldırıp tekrar deneyiniz."
                : "Koordinatı kayıtlı toplanma alanı yok.";
        else if (sonuc.Alanlar[0].MesafeKm > UzakUyariKm)
            sonuc.Uyari = $"En yakın toplanma alanı {sonuc.Alanlar[0].MesafeKm:0} km uzakta. " +
                          "Yakınınızda kayıtlı alan yok; yerel yönetimin duyurularını takip ediniz.";
        return sonuc;
    }

    /// <summary>Bir bölgenin merkezinden en yakın alanlar.</summary>
    public EnYakinSonucu BolgeMerkezindenEnYakinlar(AktifKullanici kullanici, int bolgeId, bool sadeceAboneliklerim)
    {
        var b = bolgeRepo.Getir(bolgeId) ?? throw new IsKuraliHatasi("Bölge seçiniz.");
        if (b.Enlem is not { } e || b.Boylam is not { } bb)
            throw new IsKuraliHatasi($"{b.BolgeAdi} bölgesinin merkez koordinatı kayıtlı değil.");
        return EnYakinlar(kullanici, e, bb, $"{b.BolgeAdi} (bölge merkezi)", sadeceAboneliklerim);
    }

    /// <summary>Yazılan adresten (Hafta 9 harita API'si ile) en yakın alanlar.</summary>
    public EnYakinSonucu AdrestenEnYakinlar(AktifKullanici kullanici, string? adres, bool sadeceAboneliklerim)
    {
        var konum = harita.AdresBul(adres);
        return EnYakinlar(kullanici, konum.Enlem, konum.Boylam, konum.BulunanAdres, sadeceAboneliklerim);
    }

    /// <summary>Kullanıcının abone olduğu ilk bölge (sayfa ilk açıldığında varsayılan konum için).</summary>
    public int? IlkAbonelikBolgesi(AktifKullanici kullanici) =>
        abonelikRepo.KullaniciAbonelikleri(kullanici.Id).Select(a => (int?)a.BolgeId).FirstOrDefault();
}
