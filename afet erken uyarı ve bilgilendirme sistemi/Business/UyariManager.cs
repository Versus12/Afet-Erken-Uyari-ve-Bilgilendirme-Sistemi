using System;
using System.Collections.Generic;
using System.Linq;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Data;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Business
{
    /// <summary>
    /// Uyarı yayını iş kuralları (Hafta 7).
    /// - Yalnızca yönetici uyarı yayınlayabilir / yayından kaldırabilir.
    /// - Uyarı en az bir bölgeye hedeflenmelidir.
    /// - Bitiş tarihi yayın tarihinden sonra olmalı; uyarı en fazla 30 gün geçerli olabilir.
    /// </summary>
    public class UyariManager
    {
        public const int MaksimumGecerlilikGun = 30;

        private readonly UyariRepository _repo = new UyariRepository();
        private readonly AbonelikRepository _abonelikRepo = new AbonelikRepository();

        /// <summary>Uyarıyı yayınlar ve ulaşacağı abone sayısını döndürür.</summary>
        public int Yayinla(Uyari u, out int aliciSayisi)
        {
            Kullanici yonetici = Oturum.YoneticiGerekli();

            u.Baslik = (u.Baslik ?? "").Trim();
            u.Mesaj = (u.Mesaj ?? "").Trim();
            u.BolgeIdleri = (u.BolgeIdleri ?? new List<int>()).Distinct().ToList();

            if (u.Baslik.Length < 5 || u.Baslik.Length > 150)
                throw new IsKuraliHatasi("Başlık 5-150 karakter olmalıdır.");
            if (u.Mesaj.Length < 10 || u.Mesaj.Length > 2000)
                throw new IsKuraliHatasi("Mesaj 10-2000 karakter olmalıdır.");
            if (Array.IndexOf(AfetTurleri.Tumu, u.AfetTuru) < 0)
                throw new IsKuraliHatasi("Geçerli bir afet türü seçiniz.");
            if (u.Seviye < UyariSeviyeleri.EnDusuk || u.Seviye > UyariSeviyeleri.EnYuksek)
                throw new IsKuraliHatasi("Geçerli bir uyarı seviyesi seçiniz.");
            if (u.BolgeIdleri.Count == 0)
                throw new IsKuraliHatasi("Uyarı en az bir bölgeye hedeflenmelidir.");

            if (u.YayinTarihi == default(DateTime)) u.YayinTarihi = DateTime.Now;
            if (u.BitisTarihi.HasValue)
            {
                if (u.BitisTarihi.Value <= u.YayinTarihi)
                    throw new IsKuraliHatasi("Bitiş tarihi yayın tarihinden sonra olmalıdır.");
                if ((u.BitisTarihi.Value - u.YayinTarihi).TotalDays > MaksimumGecerlilikGun)
                    throw new IsKuraliHatasi("Bir uyarı en fazla " + MaksimumGecerlilikGun + " gün geçerli olabilir.");
            }

            u.YayinlayanId = yonetici.Id;
            u.AktifMi = true;
            u.UyariId = _repo.Ekle(u);

            aliciSayisi = _abonelikRepo.UyariAliciSayisi(u.UyariId);
            return u.UyariId;
        }

        public List<Uyari> TumUyarilar()
        {
            Oturum.YoneticiGerekli();
            return _repo.TumunuListele();
        }

        /// <summary>Giriş yapan kullanıcının abone olduğu bölgelerdeki güncel uyarılar (bildirim kutusu).</summary>
        /// <param name="arkaPlan">Zamanlayıcıyla otomatik yenilemede true; oturum süresini uzatmaz.</param>
        public List<Uyari> Uyarilarim(bool arkaPlan = false)
        {
            Kullanici k = Oturum.GirisGerekli(!arkaPlan);
            return _repo.KullaniciIcinGecerliUyarilar(k.Id);
        }

        public void YayindanKaldir(int uyariId)
        {
            Oturum.YoneticiGerekli();
            Uyari u = _repo.Getir(uyariId);
            if (u == null) throw new IsKuraliHatasi("Uyarı bulunamadı.");
            if (!u.AktifMi) throw new IsKuraliHatasi("Bu uyarı zaten yayından kaldırılmış.");
            _repo.YayindanKaldir(uyariId);
        }
    }
}
