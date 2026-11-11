using System;
using System.Collections.Generic;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Data;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Business
{
    /// <summary>
    /// Hazırlık rehberi iş kuralları (Hafta 7).
    /// Herkes okuyabilir; yalnızca yönetici ekleyip düzenleyebilir.
    /// </summary>
    public class RehberManager
    {
        private readonly RehberRepository _repo = new RehberRepository();

        public List<HazirlikRehberi> Listele(string afetTuru)
        {
            Oturum.GirisGerekli();
            return _repo.Listele(string.IsNullOrEmpty(afetTuru) ? null : afetTuru);
        }

        public void Kaydet(HazirlikRehberi r)
        {
            Oturum.YoneticiGerekli();

            r.Baslik = (r.Baslik ?? "").Trim();
            r.Icerik = (r.Icerik ?? "").Trim();

            if (Array.IndexOf(AfetTurleri.Tumu, r.AfetTuru) < 0) throw new IsKuraliHatasi("Geçerli bir afet türü seçiniz.");
            if (Array.IndexOf(RehberAsamalari.Tumu, r.Asama) < 0) throw new IsKuraliHatasi("Geçerli bir aşama seçiniz.");
            if (r.Baslik.Length < 3 || r.Baslik.Length > 150) throw new IsKuraliHatasi("Başlık 3-150 karakter olmalıdır.");
            if (r.Icerik.Length < 10) throw new IsKuraliHatasi("İçerik en az 10 karakter olmalıdır.");
            if (r.Sira < 1 || r.Sira > 100) throw new IsKuraliHatasi("Sıra 1-100 arasında olmalıdır.");

            if (r.RehberId > 0) _repo.Guncelle(r);
            else r.RehberId = _repo.Ekle(r);
        }

        public void Sil(int rehberId)
        {
            Oturum.YoneticiGerekli();
            _repo.Sil(rehberId);
        }
    }
}
