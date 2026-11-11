using System;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities
{
    /// <summary>Afet türüne ve aşamasına göre hazırlık yönergesi. Tablo: HazirlikRehberi</summary>
    public class HazirlikRehberi
    {
        public int RehberId { get; set; }
        public string AfetTuru { get; set; } = string.Empty;

        /// <summary>Önce / Sırasında / Sonra</summary>
        public string Asama { get; set; } = string.Empty;

        public string Baslik { get; set; } = string.Empty;
        public string Icerik { get; set; } = string.Empty;
        public int Sira { get; set; } = 1;
        public DateTime GuncellemeTarihi { get; set; }
    }
}
