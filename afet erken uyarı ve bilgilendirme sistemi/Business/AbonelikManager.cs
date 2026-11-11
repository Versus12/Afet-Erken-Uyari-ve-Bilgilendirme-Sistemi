using System;
using System.Collections.Generic;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Data;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Business
{
    /// <summary>
    /// Abonelik modülü iş kuralları (Hafta 6).
    /// - Kullanıcı birden çok bölgeye abone olabilir (en fazla <see cref="MaksimumAbonelik"/>).
    /// - Aynı bölgeye ikinci kez abone olunamaz.
    /// - Kullanıcı yalnızca KENDİ aboneliklerini değiştirebilir/silebilir (sahiplik kontrolü).
    /// </summary>
    public class AbonelikManager
    {
        public const int MaksimumAbonelik = 10;

        private readonly AbonelikRepository _repo = new AbonelikRepository();

        public List<Abonelik> Aboneliklerim()
        {
            Kullanici k = Oturum.GirisGerekli();
            return _repo.KullaniciAbonelikleri(k.Id);
        }

        public int AboneOl(int bolgeId, string kanal, int minSeviye)
        {
            Kullanici k = Oturum.GirisGerekli();

            if (bolgeId <= 0) throw new IsKuraliHatasi("Lütfen bir bölge seçiniz.");
            TercihDogrula(kanal, minSeviye);

            if (_repo.AbonelikVarMi(k.Id, bolgeId))
                throw new IsKuraliHatasi("Bu bölgeye zaten abonesiniz.");
            if (_repo.AbonelikSayisi(k.Id) >= MaksimumAbonelik)
                throw new IsKuraliHatasi("En fazla " + MaksimumAbonelik + " bölgeye abone olabilirsiniz.");

            return _repo.Ekle(new Abonelik
            {
                KullaniciId = k.Id,
                BolgeId = bolgeId,
                BildirimKanali = kanal,
                MinSeviye = minSeviye
            });
        }

        public void TercihGuncelle(int abonelikId, string kanal, int minSeviye, bool aktifMi)
        {
            SahiplikKontrol(abonelikId);
            TercihDogrula(kanal, minSeviye);
            _repo.TercihGuncelle(abonelikId, kanal, minSeviye, aktifMi);
        }

        public void AbonelikIptal(int abonelikId)
        {
            SahiplikKontrol(abonelikId);
            _repo.Sil(abonelikId);
        }

        private void SahiplikKontrol(int abonelikId)
        {
            Kullanici k = Oturum.GirisGerekli();
            Abonelik a = _repo.Getir(abonelikId);
            if (a == null) throw new IsKuraliHatasi("Abonelik bulunamadı.");
            if (a.KullaniciId != k.Id)
                throw new YetkisizErisimHatasi("Başka bir kullanıcının aboneliğini değiştiremezsiniz.");
        }

        private static void TercihDogrula(string kanal, int minSeviye)
        {
            if (Array.IndexOf(BildirimKanallari.Tumu, kanal) < 0)
                throw new IsKuraliHatasi("Geçersiz bildirim kanalı.");
            if (minSeviye < UyariSeviyeleri.EnDusuk || minSeviye > UyariSeviyeleri.EnYuksek)
                throw new IsKuraliHatasi("Geçersiz uyarı seviyesi tercihi.");
        }
    }
}
