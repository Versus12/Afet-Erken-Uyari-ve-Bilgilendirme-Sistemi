namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities
{
    /// <summary>Bölgeye bağlı afet toplanma alanı. Tablo: ToplanmaAlanlari</summary>
    public class ToplanmaAlani
    {
        public int AlanId { get; set; }
        public int BolgeId { get; set; }
        public string AlanAdi { get; set; } = string.Empty;
        public string Adres { get; set; } = string.Empty;
        public int Kapasite { get; set; }
        public decimal? Enlem { get; set; }
        public decimal? Boylam { get; set; }

        // Listeleme için
        public string BolgeAdi { get; set; } = string.Empty;
    }

    /// <summary>Hafta 11: en yakın alan hesabının bir satırı (alan + kuş uçuşu mesafe + tahmini yürüme süresi).</summary>
    public class YakinAlan
    {
        public ToplanmaAlani Alan { get; set; }
        public double MesafeKm { get; set; }
        public int YurumeDakika { get; set; }
    }

    /// <summary>Hafta 11: en yakın alan aramasının sonucu.</summary>
    public class EnYakinSonucu
    {
        public decimal Enlem { get; set; }
        public decimal Boylam { get; set; }
        public string KonumAciklamasi { get; set; } = string.Empty;
        public System.Collections.Generic.List<YakinAlan> Alanlar { get; set; } = new System.Collections.Generic.List<YakinAlan>();
        /// <summary>Koordinatı girilmediği için hesaba katılamayan alan sayısı.</summary>
        public int KonumsuzAlanSayisi { get; set; }
        /// <summary>Kullanıcıya gösterilecek uyarı (örn. en yakın alan çok uzak).</summary>
        public string Uyari { get; set; } = string.Empty;
    }
}
