using System;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities
{
    /// <summary>Sisteme giriş yapan kişi (vatandaş veya yönetici). Tablo: Kullanicilar</summary>
    public class Kullanici
    {
        public int Id { get; set; }
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Eposta { get; set; } = string.Empty;
        public string SifreHash { get; set; } = string.Empty;
        public string Rol { get; set; } = Roller.Vatandas;
        public DateTime OlusturmaTarihi { get; set; }
        public bool AktifMi { get; set; } = true;

        public bool YoneticiMi
        {
            get { return Rol == Roller.Yonetici; }
        }
    }
}
