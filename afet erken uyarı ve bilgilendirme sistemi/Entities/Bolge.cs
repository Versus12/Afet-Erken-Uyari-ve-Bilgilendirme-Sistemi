namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities
{
    /// <summary>Uyarıların hedeflendiği coğrafi bölge (il / ilçe). Tablo: Bolgeler</summary>
    public class Bolge
    {
        public int BolgeId { get; set; }
        public string Il { get; set; } = string.Empty;
        public string Ilce { get; set; } = string.Empty;
        public string BolgeAdi { get; set; } = string.Empty;

        /// <summary>1 (düşük) - 5 (çok yüksek)</summary>
        public int RiskSeviyesi { get; set; } = 1;

        public decimal? Enlem { get; set; }
        public decimal? Boylam { get; set; }

        public override string ToString()
        {
            return BolgeAdi;
        }
    }
}
