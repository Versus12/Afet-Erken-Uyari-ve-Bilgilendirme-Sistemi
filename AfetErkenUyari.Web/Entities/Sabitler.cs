namespace AfetErkenUyari.Web.Entities;

/// <summary>Kullanıcı rolleri. Veritabanındaki CK_Kullanicilar_Rol kısıtıyla aynıdır.</summary>
public static class Roller
{
    public const string Vatandas = "Vatandas";
    public const string Yonetici = "Yonetici";
}

/// <summary>Afet türleri. CK_Uyarilar_AfetTuru ve CK_HazirlikRehberi_AfetTuru ile aynıdır.</summary>
public static class AfetTurleri
{
    public static readonly string[] Tumu = { "Deprem", "Sel", "Yangın", "Fırtına", "Heyelan", "Çığ", "Kar", "Diğer" };
}

/// <summary>
/// Bildirim kanalları. CK_Abonelikler_Kanal ile aynıdır.
/// Hafta 9: SMS kaldırıldı. Tek dış kanal E-posta'dır ve kullanıcının kayıtlı
/// e-posta adresine gönderilir (ayrı bir "mail" adresi tutulmaz).
/// </summary>
public static class BildirimKanallari
{
    public const string Uygulama = "Uygulama";
    public const string Eposta = "Eposta";

    public static readonly string[] Tumu = { Uygulama, Eposta };

    /// <summary>Ekranda gösterilen ad.</summary>
    public static string Ad(string? kanal) => kanal switch
    {
        Eposta => "E-posta",
        Uygulama => "Uygulama içi",
        _ => kanal ?? ""
    };
}

/// <summary>Bildirim gönderim durumları. CK_Bildirimler_Durum ile aynıdır.</summary>
public static class BildirimDurumlari
{
    public const string Bekliyor = "Bekliyor";
    public const string Gonderildi = "Gonderildi";
    public const string DenemeModu = "DenemeModu";
    public const string Basarisiz = "Basarisiz";

    public static string Ad(string durum) => durum switch
    {
        Gonderildi => "Gönderildi",
        DenemeModu => "Deneme modu (dosyaya yazıldı)",
        Basarisiz => "Başarısız",
        _ => "Bekliyor"
    };

    /// <summary>Bootstrap renk sınıfı.</summary>
    public static string Renk(string durum) => durum switch
    {
        Gonderildi => "success",
        DenemeModu => "info",
        Basarisiz => "danger",
        _ => "warning"
    };
}

/// <summary>Rehber aşamaları.</summary>
public static class RehberAsamalari
{
    public static readonly string[] Tumu = { "Önce", "Sırasında", "Sonra" };
}

/// <summary>Uyarı seviyeleri: 1=Bilgi, 2=Dikkat, 3=Tehlike, 4=Acil.</summary>
public static class UyariSeviyeleri
{
    public const int EnDusuk = 1;
    public const int EnYuksek = 4;
    private static readonly string[] Adlar = { "", "Bilgi", "Dikkat", "Tehlike", "Acil" };

    public static string Ad(int seviye) => seviye is >= EnDusuk and <= EnYuksek ? Adlar[seviye] : "?";

    /// <summary>Bootstrap renk sınıfı (tablo satırı / rozet).</summary>
    public static string Renk(int seviye) => seviye switch
    {
        4 => "danger",
        3 => "warning",
        2 => "info",
        _ => "secondary"
    };
}
