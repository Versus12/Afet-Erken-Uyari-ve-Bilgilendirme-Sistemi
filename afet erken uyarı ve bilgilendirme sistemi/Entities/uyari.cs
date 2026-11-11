using System;
using System.Collections.Generic;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities
{
    /// <summary>Yönetici tarafından bir veya birden çok bölgeye yayınlanan uyarı. Tablo: Uyarilar</summary>
    public class Uyari
    {
        public int UyariId { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string Mesaj { get; set; } = string.Empty;
        public string AfetTuru { get; set; } = string.Empty;

        /// <summary>1=Bilgi, 2=Dikkat, 3=Tehlike, 4=Acil</summary>
        public int Seviye { get; set; } = 1;

        public DateTime YayinTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public int YayinlayanId { get; set; }
        public bool AktifMi { get; set; } = true;

        /// <summary>Hedef bölgeler (UyariBolgeleri tablosu)</summary>
        public List<int> BolgeIdleri { get; set; } = new List<int>();

        // Listeleme için
        public string HedefBolgeler { get; set; } = string.Empty;
        public string YayinlayanAdi { get; set; } = string.Empty;

        public string SeviyeAdi
        {
            get { return UyariSeviyeleri.Ad(Seviye); }
        }

        /// <summary>Uyarı şu an yürürlükte mi?</summary>
        public bool GecerliMi(DateTime simdi)
        {
            return AktifMi && YayinTarihi <= simdi && (!BitisTarihi.HasValue || BitisTarihi.Value > simdi);
        }
    }
}
