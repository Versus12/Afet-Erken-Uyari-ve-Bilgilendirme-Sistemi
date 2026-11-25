using System;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities
{
    /// <summary>
    /// Bir uyarının bir kullanıcıya e-postayla gönderim kaydı (Hafta 9).
    /// Kuyruk + günlük görevi görür: önce "Bekliyor" olarak eklenir, gönderimden sonra sonucu yazılır.
    /// </summary>
    public class Bildirim
    {
        public int BildirimId { get; set; }
        public int UyariId { get; set; }
        public int KullaniciId { get; set; }
        public string Kanal { get; set; } = BildirimKanallari.Eposta;
        public string Alici { get; set; } = string.Empty;
        public string Durum { get; set; } = BildirimDurumlari.Bekliyor;
        public int DenemeSayisi { get; set; }
        public string SonHata { get; set; }
        public string SaglayiciMesajId { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public DateTime? GonderimTarihi { get; set; }

        // Listeleme için (JOIN)
        public string KullaniciAdi { get; set; } = string.Empty;
    }

    /// <summary>Gönderim sonucunun özeti (yayın sonrası kullanıcıya gösterilir).</summary>
    public class BildirimSonucu
    {
        public int Toplam { get; set; }
        public int Gonderilen { get; set; }
        public int DenemeModu { get; set; }
        public int Basarisiz { get; set; }
        public string SaglayiciAdi { get; set; } = string.Empty;

        public override string ToString()
        {
            if (Toplam == 0) return "E-posta tercih eden abone yok.";
            string s = Toplam + " e-posta: " + Gonderilen + " gönderildi";
            if (DenemeModu > 0) s += ", " + DenemeModu + " deneme modunda dosyaya yazıldı";
            if (Basarisiz > 0) s += ", " + Basarisiz + " başarısız (yeniden denenebilir)";
            return s + ".";
        }
    }

    /// <summary>Yayın işleminin sonucu: uyarı numarası, kaç aboneye ulaşacağı ve e-posta gönderim özeti.</summary>
    public class YayinSonucu
    {
        public int UyariId { get; set; }
        public int AliciSayisi { get; set; }
        public BildirimSonucu Eposta { get; set; } = new BildirimSonucu();
    }
}
