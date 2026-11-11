using System.Collections.Generic;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Data;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Business
{
    /// <summary>Bölge modülü iş kuralları (Hafta 6).</summary>
    public class BolgeManager
    {
        private readonly BolgeRepository _repo = new BolgeRepository();

        /// <summary>Tüm giriş yapmış kullanıcılar bölgeleri görebilir.</summary>
        public List<Bolge> Listele()
        {
            Oturum.GirisGerekli();
            return _repo.Listele();
        }

        public int Ekle(Bolge b)
        {
            Oturum.YoneticiGerekli();
            Dogrula(b);
            return _repo.Ekle(b);
        }

        public void Guncelle(Bolge b)
        {
            Oturum.YoneticiGerekli();
            if (b.BolgeId <= 0) throw new IsKuraliHatasi("Güncellenecek bölgeyi seçiniz.");
            Dogrula(b);
            _repo.Guncelle(b);
        }

        /// <summary>İş kuralı: Aboneliği, uyarısı veya toplanma alanı olan bölge silinemez.</summary>
        public void Sil(int bolgeId)
        {
            Oturum.YoneticiGerekli();
            int bagli = _repo.BagliKayitSayisi(bolgeId);
            if (bagli > 0)
                throw new IsKuraliHatasi("Bu bölgeye bağlı " + bagli +
                    " kayıt (abonelik/uyarı/toplanma alanı) var. Önce bunları kaldırınız.");
            _repo.Sil(bolgeId);
        }

        private void Dogrula(Bolge b)
        {
            b.Il = (b.Il ?? "").Trim();
            b.Ilce = (b.Ilce ?? "").Trim();

            if (b.Il.Length < 2 || b.Il.Length > 50) throw new IsKuraliHatasi("İl adı 2-50 karakter olmalıdır.");
            if (b.Ilce.Length < 2 || b.Ilce.Length > 50) throw new IsKuraliHatasi("İlçe adı 2-50 karakter olmalıdır.");
            if (b.RiskSeviyesi < 1 || b.RiskSeviyesi > 5) throw new IsKuraliHatasi("Risk seviyesi 1 ile 5 arasında olmalıdır.");
            if (b.Enlem.HasValue && (b.Enlem < -90 || b.Enlem > 90)) throw new IsKuraliHatasi("Enlem -90 ile 90 arasında olmalıdır.");
            if (b.Boylam.HasValue && (b.Boylam < -180 || b.Boylam > 180)) throw new IsKuraliHatasi("Boylam -180 ile 180 arasında olmalıdır.");
            if (b.Enlem.HasValue != b.Boylam.HasValue) throw new IsKuraliHatasi("Enlem ve boylam birlikte girilmelidir.");

            if (_repo.AyniBolgeVarMi(b.Il, b.Ilce, b.BolgeId))
                throw new IsKuraliHatasi(b.Il + " / " + b.Ilce + " bölgesi zaten kayıtlı.");

            b.BolgeAdi = b.Il + " / " + b.Ilce;
        }
    }
}
