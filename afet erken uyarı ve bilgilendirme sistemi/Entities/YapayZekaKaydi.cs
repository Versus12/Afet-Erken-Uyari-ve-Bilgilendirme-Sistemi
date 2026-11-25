using System;
using System.Collections.Generic;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities
{
    /// <summary>Tablo: YapayZekaKayitlari — AI günlüğü (Hafta 10). Her taslak üretme denemesi kaydedilir.</summary>
    public class YapayZekaKaydi
    {
        public int KayitId { get; set; }
        public int KullaniciId { get; set; }
        public DateTime Tarih { get; set; }
        public string Saglayici { get; set; } = string.Empty;
        public string HedefBolgeler { get; set; } = string.Empty;
        public string KaynakMetin { get; set; } = string.Empty;
        public string Istem { get; set; } = string.Empty;
        public string HamCikti { get; set; }
        public string Baslik { get; set; }
        public string Mesaj { get; set; }
        public int? OnerilenSeviye { get; set; }
        public bool Gecerli { get; set; }
        public string DogrulamaNotlari { get; set; }
        public int? SureMs { get; set; }

        // Listeleme için (JOIN)
        public string KullaniciAdi { get; set; } = string.Empty;
    }

    /// <summary>
    /// Duyurudan üretilen sade uyarı taslağı. Yayınlanmaz; yönetici kontrol edip yayın formuna aktarır.
    /// </summary>
    public class SadeUyariTaslagi
    {
        public string Baslik { get; set; } = string.Empty;
        public string Mesaj { get; set; } = string.Empty;
        public int Seviye { get; set; } = 2;
        public string AfetTuru { get; set; } = string.Empty;

        /// <summary>"Claude (claude-opus-5-5)" veya "Kural tabanlı".</summary>
        public string Saglayici { get; set; } = string.Empty;
        public bool YapayZekaKullanildi { get; set; }

        /// <summary>Doğrulamadan geçti mi? Geçmediyse yayın formuna aktarılmaz.</summary>
        public bool Gecerli { get; set; }
        public List<string> Hatalar { get; set; } = new List<string>();
        public List<string> Uyarilar { get; set; } = new List<string>();

        /// <summary>Kullanıcıya gösterilecek bilgi (örn. neden kural tabanlı taslak gösterildiği).</summary>
        public string Not { get; set; } = string.Empty;
    }
}
