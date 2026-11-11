using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Data;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Business
{
    /// <summary>
    /// Kayıt ve giriş iş kuralları (Hafta 5).
    /// - Parolalar BCrypt ile tuzlanarak (salt) hash'lenir, düz metin saklanmaz.
    /// - Art arda başarısız girişlerde hesap geçici olarak kilitlenir (kaba kuvvet koruması).
    /// - Kullanıcı yok / parola yanlış durumunda aynı genel mesaj verilir (kullanıcı adı tahminini zorlaştırır).
    /// </summary>
    public class KimlikDogrulamaManager
    {
        public const int MaksimumHataliDeneme = 5;
        public static readonly TimeSpan KilitSuresi = TimeSpan.FromMinutes(2);
        private const int BcryptMaliyet = 11;

        // Kullanıcı adı -> (hatalı deneme sayısı, kilit bitiş zamanı)
        private static readonly Dictionary<string, int> HataliDenemeler =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, DateTime> Kilitler =
            new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

        private readonly KullaniciRepository _repo = new KullaniciRepository();

        public Kullanici KayitOl(string kullaniciAdi, string eposta, string sifre, string sifreTekrar)
        {
            kullaniciAdi = (kullaniciAdi ?? "").Trim();
            eposta = (eposta ?? "").Trim().ToLowerInvariant();

            if (kullaniciAdi.Length < 3 || kullaniciAdi.Length > 50)
                throw new IsKuraliHatasi("Kullanıcı adı 3-50 karakter olmalıdır.");
            if (!Regex.IsMatch(kullaniciAdi, @"^[A-Za-z0-9_.]+$"))
                throw new IsKuraliHatasi("Kullanıcı adı yalnızca harf (Türkçe karakter hariç), rakam, '_' ve '.' içerebilir.");
            if (!Regex.IsMatch(eposta, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new IsKuraliHatasi("Geçerli bir e-posta adresi giriniz.");

            SifreKurallariniDogrula(sifre);
            if (sifre != sifreTekrar)
                throw new IsKuraliHatasi("Şifreler eşleşmiyor.");

            if (_repo.KullaniciAdiVarMi(kullaniciAdi))
                throw new IsKuraliHatasi("Bu kullanıcı adı zaten alınmış.");
            if (_repo.EpostaVarMi(eposta))
                throw new IsKuraliHatasi("Bu e-posta adresi zaten kayıtlı.");

            Kullanici yeni = new Kullanici
            {
                KullaniciAdi = kullaniciAdi,
                Eposta = eposta,
                SifreHash = BCrypt.Net.BCrypt.HashPassword(sifre, BcryptMaliyet),
                // Kayıt ekranından her zaman Vatandaş rolü atanır; yönetici rolü yalnızca veritabanından verilir.
                Rol = Roller.Vatandas
            };
            yeni.Id = _repo.Ekle(yeni);
            return yeni;
        }

        public Kullanici GirisYap(string kullaniciAdi, string sifre)
        {
            kullaniciAdi = (kullaniciAdi ?? "").Trim();
            if (kullaniciAdi.Length == 0 || string.IsNullOrEmpty(sifre))
                throw new IsKuraliHatasi("Kullanıcı adı ve şifre boş bırakılamaz.");

            DateTime kilitBitis;
            if (Kilitler.TryGetValue(kullaniciAdi, out kilitBitis))
            {
                if (kilitBitis > DateTime.Now)
                {
                    int kalanSn = (int)Math.Ceiling((kilitBitis - DateTime.Now).TotalSeconds);
                    throw new IsKuraliHatasi("Çok fazla hatalı deneme. " + kalanSn + " saniye sonra tekrar deneyiniz.");
                }
                Kilitler.Remove(kullaniciAdi);
                HataliDenemeler.Remove(kullaniciAdi);
            }

            Kullanici k = _repo.KullaniciAdiIleGetir(kullaniciAdi);
            bool dogru = k != null && k.AktifMi && SifreDogrulaGuvenli(sifre, k.SifreHash);

            if (!dogru)
            {
                int sayi;
                HataliDenemeler.TryGetValue(kullaniciAdi, out sayi);
                sayi++;
                HataliDenemeler[kullaniciAdi] = sayi;

                if (sayi >= MaksimumHataliDeneme)
                {
                    Kilitler[kullaniciAdi] = DateTime.Now.Add(KilitSuresi);
                    throw new IsKuraliHatasi("Çok fazla hatalı deneme. Hesap " + KilitSuresi.TotalMinutes + " dakika kilitlendi.");
                }
                throw new IsKuraliHatasi("Kullanıcı adı veya şifre hatalı. Kalan deneme hakkı: "
                                         + (MaksimumHataliDeneme - sayi));
            }

            HataliDenemeler.Remove(kullaniciAdi);
            Oturum.Baslat(k);
            return k;
        }

        public void CikisYap()
        {
            Oturum.Kapat();
        }

        public static void SifreKurallariniDogrula(string sifre)
        {
            if (string.IsNullOrEmpty(sifre) || sifre.Length < 8)
                throw new IsKuraliHatasi("Şifre en az 8 karakter olmalıdır.");
            if (!Regex.IsMatch(sifre, "[A-ZÇĞİÖŞÜ]") || !Regex.IsMatch(sifre, "[a-zçğıöşü]") || !Regex.IsMatch(sifre, "[0-9]"))
                throw new IsKuraliHatasi("Şifre en az bir büyük harf, bir küçük harf ve bir rakam içermelidir.");
        }

        /// <summary>Bozuk/eski formatta hash varsa uygulama çökmesin, giriş reddedilsin.</summary>
        private static bool SifreDogrulaGuvenli(string sifre, string hash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(sifre, hash);
            }
            catch (BCrypt.Net.SaltParseException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false; // boş veya BCrypt biçiminde olmayan hash
            }
        }
    }
}
