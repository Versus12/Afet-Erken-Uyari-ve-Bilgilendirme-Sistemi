using AfetErkenUyari.Web.Data;
using AfetErkenUyari.Web.Entities;

namespace AfetErkenUyari.Web.Business;

/// <summary>
/// Uyarı yayını iş kuralları (Hafta 7).
/// - Yalnızca yönetici yayınlar / yayından kaldırır.
/// - Uyarı en az bir bölgeye hedeflenmelidir.
/// - Bitiş tarihi yayın tarihinden sonra olmalı; en fazla 30 gün geçerli olabilir.
/// </summary>
public class UyariManager(UyariRepository repo, AbonelikRepository abonelikRepo, BolgeRepository bolgeRepo,
                          BildirimManager bildirimManager)
{
    public const int MaksimumGecerlilikGun = 30;

    /// <summary>
    /// Uyarıyı yayınlar, ulaşacağı abone sayısını hesaplar ve E-posta kanalını seçen abonelere
    /// e-posta gönderir (Hafta 9). E-posta hatası yayını geri almaz; sonuç özette raporlanır.
    /// </summary>
    public YayinSonucu Yayinla(AktifKullanici kullanici, Uyari u)
    {
        kullanici.YoneticiOlmali();

        u.Baslik = Dogrula.Metin(u.Baslik, "Başlık", 5, 150);
        u.Mesaj = Dogrula.Metin(u.Mesaj, "Mesaj", 10, 2000);
        Dogrula.Listede(u.AfetTuru, AfetTurleri.Tumu, "afet türü");
        if (u.Seviye is < UyariSeviyeleri.EnDusuk or > UyariSeviyeleri.EnYuksek)
            throw new IsKuraliHatasi("Geçerli bir uyarı seviyesi seçiniz.");

        u.BolgeIdleri = u.BolgeIdleri.Distinct().ToList();
        if (u.BolgeIdleri.Count == 0) throw new IsKuraliHatasi("Uyarı en az bir bölgeye hedeflenmelidir.");
        var gecerliBolgeler = bolgeRepo.Listele().Select(b => b.BolgeId).ToHashSet();
        if (u.BolgeIdleri.Any(id => !gecerliBolgeler.Contains(id))) throw new IsKuraliHatasi("Geçersiz bölge seçimi.");

        u.YayinTarihi = DateTime.Now;
        if (u.BitisTarihi.HasValue)
        {
            if (u.BitisTarihi.Value <= u.YayinTarihi) throw new IsKuraliHatasi("Bitiş tarihi yayın tarihinden sonra olmalıdır.");
            if ((u.BitisTarihi.Value - u.YayinTarihi).TotalDays > MaksimumGecerlilikGun)
                throw new IsKuraliHatasi($"Bir uyarı en fazla {MaksimumGecerlilikGun} gün geçerli olabilir.");
        }

        u.YayinlayanId = kullanici.Id;
        u.AktifMi = true;
        u.UyariId = repo.Ekle(u);   // uyarı + hedef bölgeler + e-posta kuyruğu (tek transaction)

        return new YayinSonucu
        {
            UyariId = u.UyariId,
            AliciSayisi = abonelikRepo.UyariAliciSayisi(u.UyariId),
            Eposta = bildirimManager.UyariIcinGonder(kullanici, u.UyariId)
        };
    }

    public List<Uyari> TumUyarilar(AktifKullanici kullanici)
    {
        kullanici.YoneticiOlmali();
        return repo.TumunuListele();
    }

    /// <summary>Kullanıcının abone olduğu bölgelerdeki güncel uyarılar (bildirim kutusu).</summary>
    public List<Uyari> Uyarilarim(AktifKullanici kullanici) => repo.KullaniciIcinGecerliUyarilar(kullanici.Id);

    public void YayindanKaldir(AktifKullanici kullanici, int uyariId)
    {
        kullanici.YoneticiOlmali();
        Uyari u = repo.Getir(uyariId) ?? throw new IsKuraliHatasi("Uyarı bulunamadı.");
        if (!u.AktifMi) throw new IsKuraliHatasi("Bu uyarı zaten yayından kaldırılmış.");
        repo.YayindanKaldir(uyariId);
    }
}

/// <summary>Hazırlık rehberi (Hafta 7). Herkes okur; yalnızca yönetici düzenler.</summary>
public class RehberManager(RehberRepository repo)
{
    public List<HazirlikRehberi> Listele(string? afetTuru) =>
        repo.Listele(string.IsNullOrWhiteSpace(afetTuru) ? null : afetTuru);

    public HazirlikRehberi? Getir(int rehberId) => repo.Getir(rehberId);

    public void Kaydet(AktifKullanici kullanici, HazirlikRehberi r)
    {
        kullanici.YoneticiOlmali();
        Dogrula.Listede(r.AfetTuru, AfetTurleri.Tumu, "afet türü");
        Dogrula.Listede(r.Asama, RehberAsamalari.Tumu, "aşama");
        r.Baslik = Dogrula.Metin(r.Baslik, "Başlık", 3, 150);
        r.Icerik = Dogrula.Metin(r.Icerik, "İçerik", 10, 4000);
        if (r.Sira is < 1 or > 100) throw new IsKuraliHatasi("Sıra 1-100 arasında olmalıdır.");

        if (r.RehberId > 0) repo.Guncelle(r);
        else r.RehberId = repo.Ekle(r);
    }

    public void Sil(AktifKullanici kullanici, int rehberId)
    {
        kullanici.YoneticiOlmali();
        repo.Sil(rehberId);
    }
}

// ToplanmaAlaniManager → Business/ToplanmaHaritasi.cs (Hafta 11)
