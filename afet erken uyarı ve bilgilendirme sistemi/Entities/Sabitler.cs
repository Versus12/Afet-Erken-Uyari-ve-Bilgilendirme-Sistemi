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
        public static string Ad(string kanal)
        {
            return kanal == Eposta ? "E-posta" : kanal == Uygulama ? "Uygulama içi" : kanal;
        }
    }

    /// <summary>Bildirim gönderim durumları. CK_Bildirimler_Durum ile aynıdır.</summary>
    public static class BildirimDurumlari
    {
        public const string Bekliyor = "Bekliyor";
        public const string Gonderildi = "Gonderildi";
        public const string DenemeModu = "DenemeModu";
        public const string Basarisiz = "Basarisiz";

        public static string Ad(string durum)
        {
            switch (durum)
            {
                case Gonderildi: return "Gönderildi";
                case DenemeModu: return "Deneme modu (dosyaya yazıldı)";
                case Basarisiz: return "Başarısız";
                default: return "Bekliyor";
            }
        }
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
