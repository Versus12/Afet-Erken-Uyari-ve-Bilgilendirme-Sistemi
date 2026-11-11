using AfetErkenUyari.Web.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfetErkenUyari.Web.Controllers;

/// <summary>
/// Kullanıcı paneli — modüllerin uçtan uca birleştiği yer (Hafta 8).
/// Abone olunan bölgelerdeki güncel uyarılar + uygulama içi bildirim.
/// </summary>
[Authorize]
public class PanelController(UyariManager uyariManager, AbonelikManager abonelikManager) : TemelController
{
    public IActionResult Index()
    {
        ViewBag.AbonelikSayisi = abonelikManager.Aboneliklerim(Kullanici).Count;
        return View(uyariManager.Uyarilarim(Kullanici));
    }

    /// <summary>
    /// Sayfadaki betik her 60 sn'de bir bu adresi çağırır; yeni uyarı varsa bildirim gösterir.
    /// Bu istek oturum süresini uzatmaz (Program.cs > OnCheckSlidingExpiration).
    /// </summary>
    [HttpGet]
    public IActionResult UyariOzeti()
    {
        var uyarilar = uyariManager.Uyarilarim(Kullanici);
        return Json(new
        {
            sayi = uyarilar.Count,
            kimlikler = uyarilar.Select(u => u.UyariId).ToArray(),
            enYuksekSeviye = uyarilar.Count == 0 ? 0 : uyarilar.Max(u => u.Seviye)
        });
    }
}
