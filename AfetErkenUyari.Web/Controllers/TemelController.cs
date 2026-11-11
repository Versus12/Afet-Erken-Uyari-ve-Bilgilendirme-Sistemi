using System.Security.Claims;
using AfetErkenUyari.Web.Business;
using Microsoft.AspNetCore.Mvc;

namespace AfetErkenUyari.Web.Controllers;

/// <summary>
/// Tüm controller'ların ortak tabanı: oturumdaki kullanıcıyı iş katmanına taşır ve
/// iş kuralı hatalarını sayfada gösterilecek mesaja çevirir.
/// </summary>
public abstract class TemelController : Controller
{
    protected const string BasariAnahtari = "Basari";
    protected const string HataAnahtari = "Hata";

    /// <summary>Çerezdeki kimlik bilgilerinden iş katmanının anladığı kullanıcı nesnesi.</summary>
    protected AktifKullanici Kullanici => new(
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
        User.Identity?.Name ?? "",
        User.FindFirstValue(ClaimTypes.Role) ?? "");

    protected void Basari(string mesaj) => TempData[BasariAnahtari] = mesaj;
    protected void Hata(string mesaj) => TempData[HataAnahtari] = mesaj;

    /// <summary>
    /// Bir iş işlemini çalıştırır. İş kuralı hatasında mesajı TempData'ya yazar ve false döner;
    /// yetki hatasında 403 sayfasına yönlendirmek için <paramref name="yasak"/> true olur.
    /// </summary>
    protected bool Calistir(Action islem, out bool yasak)
    {
        yasak = false;
        try
        {
            islem();
            return true;
        }
        catch (IsKuraliHatasi ex)
        {
            Hata(ex.Message);
        }
        catch (YetkisizErisimHatasi ex)
        {
            Hata(ex.Message);
            yasak = true;
        }
        return false;
    }

    protected IActionResult Yasak() => RedirectToAction("ErisimEngellendi", "Hesap");
}
