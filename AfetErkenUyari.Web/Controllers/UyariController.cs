using AfetErkenUyari.Web.Business;
using AfetErkenUyari.Web.Entities;
using AfetErkenUyari.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfetErkenUyari.Web.Controllers;

/// <summary>Yönetim paneli: bölgelere uyarı yayını ve yayından kaldırma (Hafta 7).</summary>
[Authorize(Policy = "Yonetici")]
public class UyariController(UyariManager uyariManager, BolgeManager bolgeManager) : TemelController
{
    public IActionResult Index() => View(Hazirla(new UyariFormModel()));

    [HttpPost]
    public IActionResult Yayinla(UyariFormModel form)
    {
        var uyari = new Uyari
        {
            Baslik = form.Baslik ?? "",
            Mesaj = form.Mesaj ?? "",
            AfetTuru = form.AfetTuru,
            Seviye = form.Seviye,
            BolgeIdleri = form.BolgeIdleri,
            BitisTarihi = form.BitisVar ? form.BitisTarihi : null
        };

        int alici = 0;
        if (Calistir(() => alici = uyariManager.Yayinla(Kullanici, uyari), out bool yasak))
        {
            Basari($"\"{uyari.Baslik}\" uyarısı {uyari.BolgeIdleri.Count} bölgeye yayınlandı. Ulaşacağı abone sayısı: {alici}.");
            return RedirectToAction(nameof(Index));
        }
        if (yasak) return Yasak();

        // Hata varsa form, girilen değerlerle tekrar gösterilir (mesaj TempData ile sayfanın üstünde).
        return View("Index", Hazirla(form));
    }

    [HttpPost]
    public IActionResult Kaldir(int id)
    {
        if (Calistir(() => uyariManager.YayindanKaldir(Kullanici, id), out bool yasak)) Basari("Uyarı yayından kaldırıldı.");
        return yasak ? Yasak() : RedirectToAction(nameof(Index));
    }

    private UyariYonetimModel Hazirla(UyariFormModel form)
    {
        form.Bolgeler = bolgeManager.Listele();
        return new UyariYonetimModel { Form = form, Uyarilar = uyariManager.TumUyarilar(Kullanici) };
    }
}
