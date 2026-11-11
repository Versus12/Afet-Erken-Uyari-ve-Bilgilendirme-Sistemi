using System;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Business
{
    /// <summary>
    /// Masaüstü uygulama için oturum yönetimi (Hafta 5).
    /// Web'deki session/JWT yerine, giriş yapan kullanıcı bellekte tutulur ve
    /// belirli bir süre işlem yapılmazsa oturum sona erer.
    /// </summary>
    public static class Oturum
    {
        /// <summary>Bu süre boyunca işlem yapılmazsa oturum düşer.</summary>
        public static readonly TimeSpan HareketsizlikSuresi = TimeSpan.FromMinutes(30);

        private static Kullanici _aktifKullanici;
        private static DateTime _sonIslemZamani;

        public static Kullanici AktifKullanici
        {
            get { return OturumAcikMi ? _aktifKullanici : null; }
        }

        public static bool OturumAcikMi
        {
            get
            {
                return _aktifKullanici != null
                       && DateTime.Now - _sonIslemZamani < HareketsizlikSuresi;
            }
        }

        public static void Baslat(Kullanici kullanici)
        {
            _aktifKullanici = kullanici;
            _sonIslemZamani = DateTime.Now;
        }

        public static void Kapat()
        {
            _aktifKullanici = null;
        }

        /// <summary>
        /// İş katmanındaki her korumalı işlemin başında çağrılır.
        /// Yetkilendirme yalnızca arayüzde (butonu gizleyerek) yapılmaz; burada da kontrol edilir.
        /// </summary>
        /// <param name="sureyiYenile">
        /// false verilirse (örn. zamanlayıcıyla arka planda yenileme) hareketsizlik süresi uzatılmaz.
        /// </param>
        public static Kullanici GirisGerekli(bool sureyiYenile = true)
        {
            if (!OturumAcikMi)
            {
                _aktifKullanici = null;
                throw new YetkisizErisimHatasi("Oturumunuz sona erdi. Lütfen tekrar giriş yapınız.");
            }
            if (sureyiYenile) _sonIslemZamani = DateTime.Now;
            return _aktifKullanici;
        }

        public static Kullanici YoneticiGerekli()
        {
            Kullanici k = GirisGerekli();
            if (!k.YoneticiMi)
                throw new YetkisizErisimHatasi("Bu işlem için yönetici yetkisi gereklidir.");
            return k;
        }
    }
}
