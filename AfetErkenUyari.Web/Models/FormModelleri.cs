using System.ComponentModel.DataAnnotations;
using AfetErkenUyari.Web.Entities;

namespace AfetErkenUyari.Web.Models;

// Formların taşıdığı veriler. Asıl doğrulama iş katmanındadır; buradaki öznitelikler
// yalnızca tarayıcıda erken uyarı vermek içindir.

public class GirisModel
{
    [Required(ErrorMessage = "Kullanıcı adı giriniz."), Display(Name = "Kullanıcı adı")]
    public string? KullaniciAdi { get; set; }

    [Required(ErrorMessage = "Şifre giriniz."), DataType(DataType.Password), Display(Name = "Şifre")]
    public string? Sifre { get; set; }

    public string? DonusAdresi { get; set; }
}

public class KayitModel
{
    [Required(ErrorMessage = "Kullanıcı adı giriniz."), StringLength(50, MinimumLength = 3), Display(Name = "Kullanıcı adı")]
    public string? KullaniciAdi { get; set; }

    [Required(ErrorMessage = "E-posta giriniz."), EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz."), Display(Name = "E-posta")]
    public string? Eposta { get; set; }

    [Required(ErrorMessage = "Şifre giriniz."), DataType(DataType.Password), MinLength(8, ErrorMessage = "Şifre en az 8 karakter olmalıdır."), Display(Name = "Şifre")]
    public string? Sifre { get; set; }

    [Required(ErrorMessage = "Şifreyi tekrar giriniz."), DataType(DataType.Password), Compare(nameof(Sifre), ErrorMessage = "Şifreler eşleşmiyor."), Display(Name = "Şifre (tekrar)")]
    public string? SifreTekrar { get; set; }
}

public class AbonelikSayfaModel
{
    public List<Abonelik> Abonelikler { get; set; } = new();
    public List<Bolge> Bolgeler { get; set; } = new();
}

public class ToplanmaAlaniSayfaModel
{
    public List<ToplanmaAlani> Alanlar { get; set; } = new();
    public List<Bolge> Bolgeler { get; set; } = new();
    public int? BolgeId { get; set; }
    public bool Aboneliklerim { get; set; }
}

public class ToplanmaAlaniFormModel
{
    public ToplanmaAlani Alan { get; set; } = new() { Kapasite = 100 };
    public List<Bolge> Bolgeler { get; set; } = new();
}

public class RehberSayfaModel
{
    public List<HazirlikRehberi> Kayitlar { get; set; } = new();
    public string? AfetTuru { get; set; }
}

public class UyariFormModel
{
    public string? Baslik { get; set; }
    public string? Mesaj { get; set; }
    public string AfetTuru { get; set; } = "Deprem";
    public int Seviye { get; set; } = 2;
    public List<int> BolgeIdleri { get; set; } = new();
    public bool BitisVar { get; set; } = true;
    public DateTime BitisTarihi { get; set; } = DateTime.Now.AddDays(1);
    public List<Bolge> Bolgeler { get; set; } = new();
}

public class UyariYonetimModel
{
    public List<Uyari> Uyarilar { get; set; } = new();
    public UyariFormModel Form { get; set; } = new();
}

public class HataModel
{
    public int DurumKodu { get; set; }
    public string? IstekKimligi { get; set; }
}
