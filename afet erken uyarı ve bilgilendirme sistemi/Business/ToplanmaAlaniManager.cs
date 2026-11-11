using System.Collections.Generic;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Data;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Business
{
    /// <summary>
    /// Toplanma alanı iş kuralları. (Temel liste/CRUD; harita ve en yakın alan hesabı Hafta 11'de eklenecek.)
    /// </summary>
    public class ToplanmaAlaniManager
    {
        private readonly ToplanmaAlaniRepository _repo = new ToplanmaAlaniRepository();

        public List<ToplanmaAlani> Listele(int? bolgeId)
        {
            Oturum.GirisGerekli();
            return _repo.Listele(bolgeId);
        }

        public void Kaydet(ToplanmaAlani t)
        {
            Oturum.YoneticiGerekli();

            t.AlanAdi = (t.AlanAdi ?? "").Trim();
            t.Adres = (t.Adres ?? "").Trim();

            if (t.BolgeId <= 0) throw new IsKuraliHatasi("Bölge seçiniz.");
            if (t.AlanAdi.Length < 3 || t.AlanAdi.Length > 150) throw new IsKuraliHatasi("Alan adı 3-150 karakter olmalıdır.");
            if (t.Adres.Length < 5 || t.Adres.Length > 300) throw new IsKuraliHatasi("Adres 5-300 karakter olmalıdır.");
            if (t.Kapasite <= 0) throw new IsKuraliHatasi("Kapasite 0'dan büyük olmalıdır.");
            if (t.Enlem.HasValue != t.Boylam.HasValue) throw new IsKuraliHatasi("Enlem ve boylam birlikte girilmelidir.");
            if (t.Enlem.HasValue && (t.Enlem < -90 || t.Enlem > 90)) throw new IsKuraliHatasi("Enlem -90 ile 90 arasında olmalıdır.");
            if (t.Boylam.HasValue && (t.Boylam < -180 || t.Boylam > 180)) throw new IsKuraliHatasi("Boylam -180 ile 180 arasında olmalıdır.");

            if (t.AlanId > 0) _repo.Guncelle(t);
            else t.AlanId = _repo.Ekle(t);
        }

        public void Sil(int alanId)
        {
            Oturum.YoneticiGerekli();
            _repo.Sil(alanId);
        }
    }
}
