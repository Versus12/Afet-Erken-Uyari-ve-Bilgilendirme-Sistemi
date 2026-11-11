using System.Security.Claims;
using AfetErkenUyari.Web.Business;
using AfetErkenUyari.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfetErkenUyari.Web.Controllers;

/// <summary>Kayıt, giriş, çıkış (Hafta 5).</summary>
public class HesapController(KimlikDogrulamaManager kimlik) : TemelController
{
    [HttpGet]
    public IActionResult Giris(string? returnUrl, string? kullanici) =>
        View(new GirisModel { DonusAdresi = returnUrl, KullaniciAdi = kullanici });

    [HttpPost]
    public async Task<IActionResult> Giris(GirisModel model)
    {
        if (!ModelState.IsValid) return View(model);
        try
        {
            var k = kimlik.GirisYap(model.KullaniciAdi, model.Sifre);
            var kimlikBilgileri = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, k.Id.ToString()),
                new(ClaimTypes.Name, k.KullaniciAdi),
                new(ClaimTypes.Role, k.Rol)
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(kimlikBilgileri, CookieAuthenticationDefaults.AuthenticationScheme));
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Açık yönlendirme (open redirect) açığını önlemek için yalnızca site içi adreslere dönülür.
            if (!string.IsNullOrEmpty(model.DonusAdresi) && Url.IsLocalUrl(model.DonusAdresi))
                return Redirect(model.DonusAdresi);
            return RedirectToAction("Index", "Panel");
        }
        catch (IsKuraliHatasi ex)
        {
            ModelState.AddModelError("", ex.Message);
            model.Sifre = null;
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Kayit() => View(new KayitModel());

    [HttpPost]
    public IActionResult Kayit(KayitModel model)
    {
        if (!ModelState.IsValid) return View(model);
        try
        {
            var yeni = kimlik.KayitOl(model.KullaniciAdi, model.Eposta, model.Sifre, model.SifreTekrar);
            Basari("Hesabınız oluşturuldu. Şimdi giriş yapabilirsiniz.");
            return RedirectToAction(nameof(Giris), new { kullanici = yeni.KullaniciAdi });
        }
        catch (IsKuraliHatasi ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    [HttpPost, Authorize]
    public async Task<IActionResult> Cikis()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Basari("Oturumunuz kapatıldı.");
        return RedirectToAction(nameof(Giris));
    }

    public IActionResult ErisimEngellendi() => View();
}
