using AfetErkenUyari.Web.Business;
using AfetErkenUyari.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfetErkenUyari.Web.Controllers;

/// <summary>Toplanma alanları listesi; yönetici ekler/düzenler. Hafta 9: adresten konum bulma (Nominatim) ve haritada açma.</summary>
[Authorize]
public class ToplanmaAlaniController(ToplanmaAlaniManager manager, BolgeManager bolgeManager, HaritaServisi harita) : TemelController
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

    /// <summary>Hafta 9 - Harita API: formdaki adresi koordinata çevirip formu yeniden gösterir (henüz kaydetmez).</summary>
    [HttpPost, Authorize(Policy = "Yonetici")]
    public IActionResult KonumBul(ToplanmaAlaniFormModel model)
    {
        model.Bolgeler = bolgeManager.Listele();
        var bolge = model.Bolgeler.FirstOrDefault(b => b.BolgeId == model.Alan.BolgeId);
        // Aramayı netleştirmek için bölgenin il/ilçesi adrese eklenir.
        string aranan = (model.Alan.Adres ?? "").Trim();
        if (bolge != null && !aranan.Contains(bolge.Il, StringComparison.CurrentCultureIgnoreCase))
            aranan += $", {bolge.Ilce}, {bolge.Il}";

        try
        {
            var konum = harita.AdresBul(aranan);
            model.Alan.Enlem = konum.Enlem;
            model.Alan.Boylam = konum.Boylam;
            model.BulunanAdres = konum.BulunanAdres;
        }
        catch (IsKuraliHatasi ex)
        {
            Hata(ex.Message);   // TempData aynı istekte de okunur; sayfanın üstünde gösterilir.
        }
        ModelState.Clear();
        return View(nameof(Duzenle), model);
    }

    [HttpPost, Authorize(Policy = "Yonetici")]
    public IActionResult Sil(int id)
    {
        if (Calistir(() => manager.Sil(Kullanici, id), out bool yasak)) Basari("Toplanma alanı silindi.");
        return yasak ? Yasak() : RedirectToAction(nameof(Index));
    }
}
