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
