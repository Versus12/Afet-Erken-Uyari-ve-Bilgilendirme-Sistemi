namespace AfetErkenUyari.Web.Entities;

// Veritabanı tablolarına karşılık gelen sade sınıflar (Hafta 3 ER modeli).
// Şema: ../afet erken uyarı ve bilgilendirme sistemi/Database/01_Sema.sql

/// <summary>Tablo: Kullanicilar</summary>
public class Kullanici
{
    public int Id { get; set; }
    public string KullaniciAdi { get; set; } = "";
    public string Eposta { get; set; } = "";
    public string SifreHash { get; set; } = "";
    public string Rol { get; set; } = Roller.Vatandas;
    public DateTime OlusturmaTarihi { get; set; }
    public bool AktifMi { get; set; } = true;
}

/// <summary>Tablo: Bolgeler (il / ilçe)</summary>
public class Bolge
{
    public int BolgeId { get; set; }
    public string Il { get; set; } = "";
    public string Ilce { get; set; } = "";
    public string BolgeAdi { get; set; } = "";
    /// <summary>1 (düşük) - 5 (çok yüksek)</summary>
    public int RiskSeviyesi { get; set; } = 1;
    public decimal? Enlem { get; set; }
    public decimal? Boylam { get; set; }
}

/// <summary>Tablo: Abonelikler — Kullanıcı ile Bölge arasındaki N-N ilişki + tercihler.</summary>
public class Abonelik
{
    public int AbonelikId { get; set; }
    public int KullaniciId { get; set; }
    public int BolgeId { get; set; }
    public string BildirimKanali { get; set; } = "Uygulama";
    public int MinSeviye { get; set; } = 1;
    public DateTime AboneTarihi { get; set; }
    public bool AktifMi { get; set; } = true;

    // Listeleme için (JOIN ile doldurulur)
    public string BolgeAdi { get; set; } = "";
    public int RiskSeviyesi { get; set; }
}

/// <summary>Tablo: Uyarilar (+ hedef bölgeler: UyariBolgeleri)</summary>
public class Uyari
{
    public int UyariId { get; set; }
    public string Baslik { get; set; } = "";
    public string Mesaj { get; set; } = "";
    public string AfetTuru { get; set; } = "";
    /// <summary>1=Bilgi, 2=Dikkat, 3=Tehlike, 4=Acil</summary>
    public int Seviye { get; set; } = 1;
    public DateTime YayinTarihi { get; set; }
    public DateTime? BitisTarihi { get; set; }
    public int YayinlayanId { get; set; }
    public bool AktifMi { get; set; } = true;
    public List<int> BolgeIdleri { get; set; } = new();

    // Listeleme için
    public string HedefBolgeler { get; set; } = "";
    public string YayinlayanAdi { get; set; } = "";

    public string SeviyeAdi => UyariSeviyeleri.Ad(Seviye);

    public bool GecerliMi(DateTime simdi) =>
        AktifMi && YayinTarihi <= simdi && (!BitisTarihi.HasValue || BitisTarihi.Value > simdi);

    public string Durum(DateTime simdi) =>
        !AktifMi ? "Kaldırıldı" : GecerliMi(simdi) ? "Yayında" : "Süresi doldu";
}

/// <summary>Tablo: HazirlikRehberi (afet türü × aşama)</summary>
public class HazirlikRehberi
{
    public int RehberId { get; set; }
    public string AfetTuru { get; set; } = "";
    /// <summary>Önce / Sırasında / Sonra</summary>
    public string Asama { get; set; } = "";
    public string Baslik { get; set; } = "";
    public string Icerik { get; set; } = "";
    public int Sira { get; set; } = 1;
    public DateTime GuncellemeTarihi { get; set; }
}

/// <summary>Tablo: ToplanmaAlanlari</summary>
public class ToplanmaAlani
{
    public int AlanId { get; set; }
    public int BolgeId { get; set; }
    public string AlanAdi { get; set; } = "";
    public string Adres { get; set; } = "";
    public int Kapasite { get; set; }
    public decimal? Enlem { get; set; }
    public decimal? Boylam { get; set; }
    public string BolgeAdi { get; set; } = "";
}

/// <summary>
/// Tablo: Bildirimler — bir uyarının bir kullanıcıya e-postayla gönderim kaydı (Hafta 9).
/// Kuyruk + günlük görevi görür: önce "Bekliyor" eklenir, gönderimden sonra sonucu yazılır.
/// </summary>
public class Bildirim
{
    public int BildirimId { get; set; }
    public int UyariId { get; set; }
    public int KullaniciId { get; set; }
    public string Kanal { get; set; } = BildirimKanallari.Eposta;
    public string Alici { get; set; } = "";
    public string Durum { get; set; } = BildirimDurumlari.Bekliyor;
    public int DenemeSayisi { get; set; }
    public string? SonHata { get; set; }
    public string? SaglayiciMesajId { get; set; }
    public DateTime OlusturmaTarihi { get; set; }
    public DateTime? GonderimTarihi { get; set; }

    // Listeleme için (JOIN)
    public string KullaniciAdi { get; set; } = "";
}

/// <summary>E-posta gönderim sonucunun özeti.</summary>
public class BildirimSonucu
{
    public int Toplam { get; set; }
    public int Gonderilen { get; set; }
    public int DenemeModu { get; set; }
    public int Basarisiz { get; set; }
    public string SaglayiciAdi { get; set; } = "";

    public override string ToString()
    {
        if (Toplam == 0) return "E-posta tercih eden abone yok.";
        string s = $"{Toplam} e-posta: {Gonderilen} gönderildi";
        if (DenemeModu > 0) s += $", {DenemeModu} deneme modunda dosyaya yazıldı";
        if (Basarisiz > 0) s += $", {Basarisiz} başarısız (yeniden denenebilir)";
        return s + ".";
    }
}

/// <summary>Yayın sonucu: uyarı numarası, ulaşacağı abone sayısı ve e-posta gönderim özeti.</summary>
public class YayinSonucu
{
    public int UyariId { get; set; }
    public int AliciSayisi { get; set; }
    public BildirimSonucu Eposta { get; set; } = new();
}

/// <summary>Harita servisinin adresten bulduğu konum (Hafta 9).</summary>
public record KonumSonucu(decimal Enlem, decimal Boylam, string BulunanAdres);

/// <summary>Tablo: YapayZekaKayitlari — AI günlüğü (Hafta 10). Her taslak üretme denemesi kaydedilir.</summary>
public class YapayZekaKaydi
{
    public int KayitId { get; set; }
    public int KullaniciId { get; set; }
    public DateTime Tarih { get; set; }
    public string Saglayici { get; set; } = "";
    public string HedefBolgeler { get; set; } = "";
    public string KaynakMetin { get; set; } = "";
    public string Istem { get; set; } = "";
    public string? HamCikti { get; set; }
    public string? Baslik { get; set; }
    public string? Mesaj { get; set; }
    public int? OnerilenSeviye { get; set; }
    public bool Gecerli { get; set; }
    public string? DogrulamaNotlari { get; set; }
    public int? SureMs { get; set; }

    // Listeleme için (JOIN)
    public string KullaniciAdi { get; set; } = "";
}

/// <summary>Duyurudan üretilen sade uyarı taslağı (Hafta 10). Yayınlanmaz; yönetici kontrol edip yayınlar.</summary>
public class SadeUyariTaslagi
{
    public string Baslik { get; set; } = "";
    public string Mesaj { get; set; } = "";
    public int Seviye { get; set; } = 2;
    public string AfetTuru { get; set; } = "";
    /// <summary>"Claude (claude-opus-5-5)" veya "Kural tabanlı".</summary>
    public string Saglayici { get; set; } = "";
    public bool YapayZekaKullanildi { get; set; }
    /// <summary>Doğrulamadan geçti mi? Geçmediyse yayın formuna aktarılmaz.</summary>
    public bool Gecerli { get; set; }
    public List<string> Hatalar { get; set; } = new();
    public List<string> Uyarilar { get; set; } = new();
    /// <summary>Kullanıcıya gösterilecek bilgi (örn. neden kural tabanlı taslak gösterildiği).</summary>
    public string Not { get; set; } = "";
}
