using AfetErkenUyari.Web.Business;
using AfetErkenUyari.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfetErkenUyari.Web.Controllers;

/// <summary>Bölge aboneliği ve tercih yönetimi (Hafta 6).</summary>
[Authorize]
public class AbonelikController(AbonelikManager abonelikManager, BolgeManager bolgeManager) : TemelController
{
    public IActionResult Index() => View(new AbonelikSayfaModel
    {
        Abonelikler = abonelikManager.Aboneliklerim(Kullanici),
        Bolgeler = bolgeManager.Listele()
    });

    [HttpPost]
    public IActionResult AboneOl(int bolgeId, string? kanal, int minSeviye)
    {
        string bolgeAdi = "";
        if (Calistir(() => bolgeAdi = abonelikManager.AboneOl(Kullanici, bolgeId, kanal, minSeviye), out _))
            Basari($"{bolgeAdi} bölgesine abone oldunuz.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult TercihGuncelle(int abonelikId, string? kanal, int minSeviye, bool aktifMi)
    {
        if (Calistir(() => abonelikManager.TercihGuncelle(Kullanici, abonelikId, kanal, minSeviye, aktifMi), out bool yasak))
            Basari("Tercihleriniz kaydedildi.");
        return yasak ? Yasak() : RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Iptal(int abonelikId)
    {
        if (Calistir(() => abonelikManager.AbonelikIptal(Kullanici, abonelikId), out bool yasak))
            Basari("Abonelik iptal edildi.");
        return yasak ? Yasak() : RedirectToAction(nameof(Index));
    }
}
