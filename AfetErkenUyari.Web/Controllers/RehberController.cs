using AfetErkenUyari.Web.Business;
using AfetErkenUyari.Web.Entities;
using AfetErkenUyari.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfetErkenUyari.Web.Controllers;

/// <summary>Hazırlık rehberi (Hafta 7): herkes okur, yönetici düzenler.</summary>
[Authorize]
public class RehberController(RehberManager manager) : TemelController
{
    public IActionResult Index(string? afetTuru) => View(new RehberSayfaModel
    {
        AfetTuru = afetTuru,
        Kayitlar = manager.Listele(afetTuru)
    });

    [HttpGet, Authorize(Policy = "Yonetici")]
    public IActionResult Duzenle(int? id)
    {
        if (id is null) return View(new HazirlikRehberi { AfetTuru = AfetTurleri.Tumu[0], Asama = RehberAsamalari.Tumu[0] });
        var kayit = manager.Getir(id.Value);
        return kayit == null ? NotFound() : View(kayit);
    }

    [HttpPost, Authorize(Policy = "Yonetici")]
    public IActionResult Duzenle(HazirlikRehberi model)
    {
        if (Calistir(() => manager.Kaydet(Kullanici, model), out bool yasak))
        {
            Basari("Rehber kaydedildi.");
            return RedirectToAction(nameof(Index), new { afetTuru = model.AfetTuru });
        }
        if (yasak) return Yasak();
        return View(model);
    }

    [HttpPost, Authorize(Policy = "Yonetici")]
    public IActionResult Sil(int id)
    {
        if (Calistir(() => manager.Sil(Kullanici, id), out bool yasak)) Basari("Rehber kaydı silindi.");
        return yasak ? Yasak() : RedirectToAction(nameof(Index));
    }
}
