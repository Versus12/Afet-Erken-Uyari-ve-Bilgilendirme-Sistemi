using System.Diagnostics;
using AfetErkenUyari.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace AfetErkenUyari.Web.Controllers;

/// <summary>Herkese açık tanıtım sayfası ve hata sayfaları.</summary>
public class HomeController : TemelController
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Panel");
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Hata() =>
        View("Hata", new HataModel { DurumKodu = 500, IstekKimligi = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    public IActionResult Durum(int id) =>
        View("Hata", new HataModel { DurumKodu = id, IstekKimligi = HttpContext.TraceIdentifier });
}
