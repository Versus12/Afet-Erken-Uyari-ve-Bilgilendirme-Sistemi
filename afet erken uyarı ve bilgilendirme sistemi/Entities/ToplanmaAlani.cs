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
}
