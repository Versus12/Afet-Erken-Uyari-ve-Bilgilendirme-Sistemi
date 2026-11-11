using AfetErkenUyari.Web.Entities;

namespace AfetErkenUyari.Web.Business;

/// <summary>İş kuralı / doğrulama ihlali. Mesajı doğrudan kullanıcıya gösterilebilir.</summary>
public class IsKuraliHatasi(string mesaj) : Exception(mesaj);

/// <summary>Rol yetersizse veya kayıt başka bir kullanıcıya aitse fırlatılır.</summary>
public class YetkisizErisimHatasi(string mesaj) : Exception(mesaj);

/// <summary>
/// Oturumu açık kullanıcının iş katmanına taşınan bilgisi (çerezdeki kimlik bilgilerinden üretilir).
/// İş katmanı HTTP'yi bilmez; yalnızca bu nesneyi görür.
/// </summary>
public record AktifKullanici(int Id, string KullaniciAdi, string Rol)
{
    public bool YoneticiMi => Rol == Roller.Yonetici;

    /// <summary>
    /// Yetkilendirme yalnızca arayüzde ([Authorize] / menü gizleme) yapılmaz;
    /// her korumalı iş metodu bunu da çağırır.
    /// </summary>
    public void YoneticiOlmali()
    {
        if (!YoneticiMi) throw new YetkisizErisimHatasi("Bu işlem için yönetici yetkisi gereklidir.");
    }
}

/// <summary>Formlardan gelen metinleri doğrulamak için ortak yardımcılar.</summary>
internal static class Dogrula
{
    public static string Metin(string? deger, string alan, int min, int max)
    {
        string s = (deger ?? "").Trim();
        if (s.Length < min || s.Length > max)
            throw new IsKuraliHatasi($"{alan} {min}-{max} karakter olmalıdır.");
        return s;
    }

    public static void Listede(string? deger, string[] liste, string alan)
    {
        if (deger == null || Array.IndexOf(liste, deger) < 0)
            throw new IsKuraliHatasi($"Geçerli bir {alan} seçiniz.");
    }

    public static void Koordinat(decimal? enlem, decimal? boylam)
    {
        if (enlem.HasValue != boylam.HasValue) throw new IsKuraliHatasi("Enlem ve boylam birlikte girilmelidir.");
        if (enlem is < -90 or > 90) throw new IsKuraliHatasi("Enlem -90 ile 90 arasında olmalıdır.");
        if (boylam is < -180 or > 180) throw new IsKuraliHatasi("Boylam -180 ile 180 arasında olmalıdır.");
    }
}
