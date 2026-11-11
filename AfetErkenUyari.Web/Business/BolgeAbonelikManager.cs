using AfetErkenUyari.Web.Data;
using AfetErkenUyari.Web.Entities;

namespace AfetErkenUyari.Web.Business;

/// <summary>Bölge modülü iş kuralları (Hafta 6).</summary>
public class BolgeManager(BolgeRepository repo)
{
    public List<Bolge> Listele() => repo.Listele();

    public Bolge? Getir(int bolgeId) => repo.Getir(bolgeId);

    public void Kaydet(AktifKullanici kullanici, Bolge b)
    {
        kullanici.YoneticiOlmali();

        b.Il = Dogrula.Metin(b.Il, "İl adı", 2, 50);
        b.Ilce = Dogrula.Metin(b.Ilce, "İlçe adı", 2, 50);
        if (b.RiskSeviyesi is < 1 or > 5) throw new IsKuraliHatasi("Risk seviyesi 1 ile 5 arasında olmalıdır.");
        Dogrula.Koordinat(b.Enlem, b.Boylam);
        if (repo.AyniBolgeVarMi(b.Il, b.Ilce, b.BolgeId))
            throw new IsKuraliHatasi($"{b.Il} / {b.Ilce} bölgesi zaten kayıtlı.");
        b.BolgeAdi = $"{b.Il} / {b.Ilce}";

        if (b.BolgeId > 0) repo.Guncelle(b);
        else b.BolgeId = repo.Ekle(b);
    }

    /// <summary>İş kuralı: aboneliği, uyarısı veya toplanma alanı olan bölge silinemez.</summary>
    public void Sil(AktifKullanici kullanici, int bolgeId)
    {
        kullanici.YoneticiOlmali();
        int bagli = repo.BagliKayitSayisi(bolgeId);
        if (bagli > 0)
            throw new IsKuraliHatasi($"Bu bölgeye bağlı {bagli} kayıt (abonelik/uyarı/toplanma alanı) var. Önce bunları kaldırınız.");
        repo.Sil(bolgeId);
    }
}

/// <summary>
/// Abonelik modülü iş kuralları (Hafta 6).
/// - Bir kullanıcı birden çok bölgeye abone olabilir (en fazla 10).
/// - Aynı bölgeye ikinci kez abone olunamaz.
/// - Kullanıcı yalnızca KENDİ aboneliğini değiştirebilir/silebilir (sahiplik kontrolü).
/// </summary>
public class AbonelikManager(AbonelikRepository repo, BolgeRepository bolgeRepo)
{
    public const int MaksimumAbonelik = 10;

    public List<Abonelik> Aboneliklerim(AktifKullanici kullanici) => repo.KullaniciAbonelikleri(kullanici.Id);

    public string AboneOl(AktifKullanici kullanici, int bolgeId, string? kanal, int minSeviye)
    {
        Bolge bolge = bolgeRepo.Getir(bolgeId) ?? throw new IsKuraliHatasi("Lütfen bir bölge seçiniz.");
        TercihDogrula(kanal, minSeviye);
        if (repo.AbonelikVarMi(kullanici.Id, bolgeId)) throw new IsKuraliHatasi("Bu bölgeye zaten abonesiniz.");
        if (repo.AbonelikSayisi(kullanici.Id) >= MaksimumAbonelik)
            throw new IsKuraliHatasi($"En fazla {MaksimumAbonelik} bölgeye abone olabilirsiniz.");

        repo.Ekle(new Abonelik { KullaniciId = kullanici.Id, BolgeId = bolgeId, BildirimKanali = kanal!, MinSeviye = minSeviye });
        return bolge.BolgeAdi;
    }

    public void TercihGuncelle(AktifKullanici kullanici, int abonelikId, string? kanal, int minSeviye, bool aktifMi)
    {
        SahiplikKontrol(kullanici, abonelikId);
        TercihDogrula(kanal, minSeviye);
        repo.TercihGuncelle(abonelikId, kanal!, minSeviye, aktifMi);
    }

    public void AbonelikIptal(AktifKullanici kullanici, int abonelikId)
    {
        SahiplikKontrol(kullanici, abonelikId);
        repo.Sil(abonelikId);
    }

    private void SahiplikKontrol(AktifKullanici kullanici, int abonelikId)
    {
        int sahibi = repo.SahibiniGetir(abonelikId) ?? throw new IsKuraliHatasi("Abonelik bulunamadı.");
        if (sahibi != kullanici.Id)
            throw new YetkisizErisimHatasi("Başka bir kullanıcının aboneliğini değiştiremezsiniz.");
    }

    private static void TercihDogrula(string? kanal, int minSeviye)
    {
        Dogrula.Listede(kanal, BildirimKanallari.Tumu, "bildirim kanalı");
        if (minSeviye is < UyariSeviyeleri.EnDusuk or > UyariSeviyeleri.EnYuksek)
            throw new IsKuraliHatasi("Geçersiz uyarı seviyesi tercihi.");
    }
}
