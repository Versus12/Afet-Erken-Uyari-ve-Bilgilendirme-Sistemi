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

/// <summary>Bildirim kanalları. CK_Abonelikler_Kanal ile aynıdır.</summary>
public static class BildirimKanallari
{
    public static readonly string[] Tumu = { "Uygulama", "SMS", "Eposta" };
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
