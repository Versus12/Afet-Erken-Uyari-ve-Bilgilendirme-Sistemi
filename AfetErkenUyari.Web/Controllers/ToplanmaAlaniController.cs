using AfetErkenUyari.Web.Business;
using AfetErkenUyari.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfetErkenUyari.Web.Controllers;

/// <summary>Toplanma alanları listesi; yönetici ekler/düzenler. (Harita Hafta 11 kapsamındadır.)</summary>
[Authorize]
public class ToplanmaAlaniController(ToplanmaAlaniManager manager, BolgeManager bolgeManager) : TemelController
{
    public IActionResult Index(int? bolgeId, bool aboneliklerim = false) => View(new ToplanmaAlaniSayfaModel
    {
        BolgeId = bolgeId,
        Aboneliklerim = aboneliklerim,
        Bolgeler = bolgeManager.Listele(),
        Alanlar = manager.Listele(Kullanici, bolgeId, aboneliklerim)
    });

    [HttpGet, Authorize(Policy = "Yonetici")]
    public IActionResult Duzenle(int? id)
    {
        var model = new ToplanmaAlaniFormModel { Bolgeler = bolgeManager.Listele() };
        if (id is not null)
        {
            var alan = manager.Getir(id.Value);
            if (alan == null) return NotFound();
            model.Alan = alan;
        }
        return View(model);
    }

    [HttpPost, Authorize(Policy = "Yonetici")]
    public IActionResult Duzenle(ToplanmaAlaniFormModel model)
    {
        if (Calistir(() => manager.Kaydet(Kullanici, model.Alan), out bool yasak))
        {
            Basari("Toplanma alanı kaydedildi.");
            return RedirectToAction(nameof(Index));
        }
        if (yasak) return Yasak();
        model.Bolgeler = bolgeManager.Listele();
        return View(model);
    }

    [HttpPost, Authorize(Policy = "Yonetici")]
    public IActionResult Sil(int id)
    {
        if (Calistir(() => manager.Sil(Kullanici, id), out bool yasak)) Basari("Toplanma alanı silindi.");
        return yasak ? Yasak() : RedirectToAction(nameof(Index));
    }
}
