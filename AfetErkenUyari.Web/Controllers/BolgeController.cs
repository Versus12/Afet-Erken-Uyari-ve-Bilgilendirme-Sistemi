using AfetErkenUyari.Web.Business;
using AfetErkenUyari.Web.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfetErkenUyari.Web.Controllers;

/// <summary>Yönetim paneli: bölge ekleme / güncelleme / silme (Hafta 6).</summary>
[Authorize(Policy = "Yonetici")]
public class BolgeController(BolgeManager manager) : TemelController
{
    public IActionResult Index() => View(manager.Listele());

    [HttpGet]
    public IActionResult Duzenle(int? id)
    {
        if (id is null) return View(new Bolge());
        var b = manager.Getir(id.Value);
        return b == null ? NotFound() : View(b);
    }

    [HttpPost]
    public IActionResult Duzenle(Bolge model)
    {
        bool yeni = model.BolgeId == 0;
        if (Calistir(() => manager.Kaydet(Kullanici, model), out bool yasak))
        {
            Basari(yeni ? "Bölge eklendi." : "Bölge güncellendi.");
            return RedirectToAction(nameof(Index));
        }
        if (yasak) return Yasak();
        return View(model);
    }

    [HttpPost]
    public IActionResult Sil(int id)
    {
        if (Calistir(() => manager.Sil(Kullanici, id), out bool yasak)) Basari("Bölge silindi.");
        return yasak ? Yasak() : RedirectToAction(nameof(Index));
    }
}
