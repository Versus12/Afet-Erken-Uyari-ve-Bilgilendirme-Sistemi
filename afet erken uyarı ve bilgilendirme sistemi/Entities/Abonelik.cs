using System;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities
{
    /// <summary>
    /// Kullanıcı ile bölge arasındaki N-N ilişkiyi tutan ara varlık. Tablo: Abonelikler
    /// Tercihler: bildirim kanalı ve kullanıcının almak istediği en düşük uyarı seviyesi.
    /// </summary>
    public class Abonelik
    {
        public int AbonelikId { get; set; }
        public int KullaniciId { get; set; }
        public int BolgeId { get; set; }
        public string BildirimKanali { get; set; } = "Uygulama";
        public int MinSeviye { get; set; } = 1;
        public DateTime AboneTarihi { get; set; }
        public bool AktifMi { get; set; } = true;

        // Listeleme için (JOIN ile doldurulur)
        public string BolgeAdi { get; set; } = string.Empty;
        public int RiskSeviyesi { get; set; }
    }
}
