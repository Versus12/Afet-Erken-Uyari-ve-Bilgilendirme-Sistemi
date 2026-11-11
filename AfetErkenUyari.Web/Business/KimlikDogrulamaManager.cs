using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using AfetErkenUyari.Web.Data;
using AfetErkenUyari.Web.Entities;

namespace AfetErkenUyari.Web.Business;

/// <summary>
/// Kaba kuvvet koruması için hatalı giriş sayacı (uygulama boyunca tek örnek - singleton).
/// </summary>
public class GirisDenemeTakibi
{
    public const int MaksimumHataliDeneme = 5;
    public static readonly TimeSpan KilitSuresi = TimeSpan.FromMinutes(2);

    private readonly ConcurrentDictionary<string, int> _denemeler = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, DateTime> _kilitler = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Kilitliyse kalan süreyi döndürür, değilse null.</summary>
    public TimeSpan? KalanKilit(string kullaniciAdi)
    {
        if (!_kilitler.TryGetValue(kullaniciAdi, out DateTime bitis)) return null;
        if (bitis > DateTime.Now) return bitis - DateTime.Now;
        _kilitler.TryRemove(kullaniciAdi, out _);
        _denemeler.TryRemove(kullaniciAdi, out _);
        return null;
    }

    /// <summary>Hatalı denemeyi kaydeder; kalan hak sayısını döndürür (0 ise hesap kilitlendi).</summary>
    public int HataliDenemeKaydet(string kullaniciAdi)
    {
        int sayi = _denemeler.AddOrUpdate(kullaniciAdi, 1, (_, eski) => eski + 1);
        if (sayi >= MaksimumHataliDeneme)
        {
            _kilitler[kullaniciAdi] = DateTime.Now.Add(KilitSuresi);
            return 0;
        }
        return MaksimumHataliDeneme - sayi;
    }

    public void Sifirla(string kullaniciAdi) => _denemeler.TryRemove(kullaniciAdi, out _);
}

/// <summary>
/// Kayıt ve giriş iş kuralları (Hafta 5).
/// - Parolalar BCrypt ile tuzlanarak (salt) hash'lenir; düz metin saklanmaz.
/// - "Kullanıcı yok" ve "parola yanlış" için aynı mesaj verilir.
/// - Kayıt ekranı her zaman Vatandaş rolü verir.
/// </summary>
public class KimlikDogrulamaManager(KullaniciRepository repo, GirisDenemeTakibi takip)
{
    private const int BcryptMaliyet = 11;

    public Kullanici KayitOl(string? kullaniciAdi, string? eposta, string? sifre, string? sifreTekrar)
    {
        string ad = (kullaniciAdi ?? "").Trim();
        string mail = (eposta ?? "").Trim().ToLowerInvariant();

        if (ad.Length is < 3 or > 50) throw new IsKuraliHatasi("Kullanıcı adı 3-50 karakter olmalıdır.");
        if (!Regex.IsMatch(ad, @"^[A-Za-z0-9_.]+$"))
            throw new IsKuraliHatasi("Kullanıcı adı yalnızca harf (Türkçe karakter hariç), rakam, '_' ve '.' içerebilir.");
        if (mail.Length > 150 || !Regex.IsMatch(mail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new IsKuraliHatasi("Geçerli bir e-posta adresi giriniz.");

        SifreKurallariniDogrula(sifre);
        if (sifre != sifreTekrar) throw new IsKuraliHatasi("Şifreler eşleşmiyor.");

        if (repo.KullaniciAdiVarMi(ad)) throw new IsKuraliHatasi("Bu kullanıcı adı zaten alınmış.");
        if (repo.EpostaVarMi(mail)) throw new IsKuraliHatasi("Bu e-posta adresi zaten kayıtlı.");

        var yeni = new Kullanici
        {
            KullaniciAdi = ad,
            Eposta = mail,
            SifreHash = BCrypt.Net.BCrypt.HashPassword(sifre, BcryptMaliyet),
            Rol = Roller.Vatandas
        };
        yeni.Id = repo.Ekle(yeni);
        return yeni;
    }

    public Kullanici GirisYap(string? kullaniciAdi, string? sifre)
    {
        string ad = (kullaniciAdi ?? "").Trim();
        if (ad.Length == 0 || string.IsNullOrEmpty(sifre))
            throw new IsKuraliHatasi("Kullanıcı adı ve şifre boş bırakılamaz.");

        TimeSpan? kalan = takip.KalanKilit(ad);
        if (kalan.HasValue)
            throw new IsKuraliHatasi($"Çok fazla hatalı deneme. {Math.Ceiling(kalan.Value.TotalSeconds)} saniye sonra tekrar deneyiniz.");

        Kullanici? k = repo.KullaniciAdiIleGetir(ad);
        bool dogru = k != null && k.AktifMi && SifreDogrulaGuvenli(sifre, k.SifreHash);

        if (!dogru)
        {
            int hak = takip.HataliDenemeKaydet(ad);
            if (hak == 0)
                throw new IsKuraliHatasi($"Çok fazla hatalı deneme. Hesap {GirisDenemeTakibi.KilitSuresi.TotalMinutes} dakika kilitlendi.");
            throw new IsKuraliHatasi($"Kullanıcı adı veya şifre hatalı. Kalan deneme hakkı: {hak}");
        }

        takip.Sifirla(ad);
        return k!;
    }

    public static void SifreKurallariniDogrula(string? sifre)
    {
        if (string.IsNullOrEmpty(sifre) || sifre.Length < 8)
            throw new IsKuraliHatasi("Şifre en az 8 karakter olmalıdır.");
        if (!Regex.IsMatch(sifre, "[A-ZÇĞİÖŞÜ]") || !Regex.IsMatch(sifre, "[a-zçğıöşü]") || !Regex.IsMatch(sifre, "[0-9]"))
            throw new IsKuraliHatasi("Şifre en az bir büyük harf, bir küçük harf ve bir rakam içermelidir.");
    }

    /// <summary>Bozuk veya BCrypt olmayan hash varsa site çökmesin, giriş reddedilsin.</summary>
    private static bool SifreDogrulaGuvenli(string sifre, string hash)
    {
        try { return BCrypt.Net.BCrypt.Verify(sifre, hash); }
        catch (BCrypt.Net.SaltParseException) { return false; }
        catch (ArgumentException) { return false; }
    }
}
