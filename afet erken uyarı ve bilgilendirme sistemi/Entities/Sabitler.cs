namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities
{
    /// <summary>Kullanıcı rolleri. Veritabanındaki CK_Kullanicilar_Rol kısıtıyla aynı olmalıdır.</summary>
    public static class Roller
    {
        public const string Vatandas = "Vatandas";
        public const string Yonetici = "Yonetici";
    }

    /// <summary>Afet türleri. CK_Uyarilar_AfetTuru ve CK_HazirlikRehberi_AfetTuru ile aynıdır.</summary>
    public static class AfetTurleri
    {
        public static readonly string[] Tumu =
        {
            "Deprem", "Sel", "Yangın", "Fırtına", "Heyelan", "Çığ", "Kar", "Diğer"
        };
    }

    /// <summary>Bildirim kanalları. CK_Abonelikler_Kanal ile aynıdır.</summary>
    public static class BildirimKanallari
    {
        public static readonly string[] Tumu = { "Uygulama", "SMS", "Eposta" };
    }

    /// <summary>Rehber aşamaları (afetten önce / sırasında / sonra).</summary>
    public static class RehberAsamalari
    {
        public static readonly string[] Tumu = { "Önce", "Sırasında", "Sonra" };
    }

    /// <summary>Uyarı seviyeleri: 1=Bilgi, 2=Dikkat, 3=Tehlike, 4=Acil.</summary>
    public static class UyariSeviyeleri
    {
        public const int EnDusuk = 1;
        public const int EnYuksek = 4;

        public static readonly string[] Adlar = { "", "Bilgi", "Dikkat", "Tehlike", "Acil" };

        public static string Ad(int seviye)
        {
            return seviye >= EnDusuk && seviye <= EnYuksek ? Adlar[seviye] : "?";
        }
    }
}
