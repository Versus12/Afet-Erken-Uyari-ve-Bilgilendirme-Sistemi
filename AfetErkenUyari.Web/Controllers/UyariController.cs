using AfetErkenUyari.Web.Business;
using AfetErkenUyari.Web.Entities;
using AfetErkenUyari.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfetErkenUyari.Web.Controllers;

/// <summary>Yönetim paneli: bölgelere uyarı yayını ve yayından kaldırma (Hafta 7).</summary>
[Authorize(Policy = "Yonetici")]
public class UyariController(UyariManager uyariManager, BolgeManager bolgeManager, BildirimManager bildirimManager,
                             SadeUyariManager sadeUyariManager) : TemelController
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

        YayinSonucu? sonuc = null;
        if (Calistir(() => sonuc = uyariManager.Yayinla(Kullanici, uyari), out bool yasak))
        {
            Basari($"\"{uyari.Baslik}\" uyarısı {uyari.BolgeIdleri.Count} bölgeye yayınlandı. Ulaşacağı abone sayısı: {sonuc!.AliciSayisi}. " +
                   $"E-posta ({sonuc.Eposta.SaglayiciAdi}): {sonuc.Eposta}");
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

    /// <summary>
    /// Hafta 10: formdaki kurum duyurusundan seçili bölgeler için sade uyarı taslağı üretir.
    /// Geçerli taslak başlık/mesaj/seviye/tür alanlarına yazılır; uyarı YAYINLANMAZ, yönetici kontrol edip yayınlar.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SadeUyariUret(UyariFormModel form)
    {
        SadeUyariTaslagi? taslak = null;
        try
        {
            taslak = await sadeUyariManager.TaslakUretAsync(Kullanici, form.KaynakMetin, form.BolgeIdleri, form.AfetTuru);
            if (taslak.Gecerli)
            {
                form.Baslik = taslak.Baslik;
                form.Mesaj = taslak.Mesaj;
                form.Seviye = taslak.Seviye;
                form.AfetTuru = taslak.AfetTuru;
            }
        }
        catch (IsKuraliHatasi ex) { Hata(ex.Message); }
        catch (YetkisizErisimHatasi) { return Yasak(); }

        ModelState.Clear();
        var model = Hazirla(form);
        model.Taslak = taslak;
        return View(nameof(Index), model);
    }

    /// <summary>Hafta 10: AI günlüğü (istem, ham çıktı, doğrulama notları).</summary>
    public IActionResult YapayZekaGunlugu() => View(sadeUyariManager.Gunluk(Kullanici, 100));

    /// <summary>Hafta 9: uyarının e-posta gönderim kayıtları.</summary>
    public IActionResult Bildirimler(int id)
    {
        var uyari = uyariManager.TumUyarilar(Kullanici).FirstOrDefault(u => u.UyariId == id);
        if (uyari == null) return NotFound();
        return View(new BildirimKayitModel
        {
            Uyari = uyari,
            SaglayiciAdi = bildirimManager.SaglayiciAdi,
            Kayitlar = bildirimManager.Kayitlar(Kullanici, id)
        });
    }

    [HttpPost]
    public IActionResult YenidenDene(int id)
    {
        BildirimSonucu? sonuc = null;
        if (Calistir(() => sonuc = bildirimManager.BasarisizlariYenidenDene(Kullanici, id), out bool yasak))
            Basari(sonuc!.Toplam == 0 ? "Yeniden denenecek kayıt yok." : sonuc.ToString());
        return yasak ? Yasak() : RedirectToAction(nameof(Bildirimler), new { id });
    }

    private UyariYonetimModel Hazirla(UyariFormModel form)
    {
        form.Bolgeler = bolgeManager.Listele();
        return new UyariYonetimModel
        {
            Form = form,
            Uyarilar = uyariManager.TumUyarilar(Kullanici),
            YapayZekaYontemi = sadeUyariManager.YontemAdi
        };
    }
}
