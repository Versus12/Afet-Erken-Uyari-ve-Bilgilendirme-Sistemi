using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using AfetErkenUyari.Web.Data;
using AfetErkenUyari.Web.Entities;

namespace AfetErkenUyari.Web.Business;

// =====================================================================================
//  Hafta 9 · Harici API entegrasyonu: E-posta bildirimi + Harita (adresten konum)
// =====================================================================================

/// <summary>Gönderilecek tek bir e-posta.</summary>
public record EpostaMesaji(string Alici, string Konu, string Govde);

/// <summary>
/// E-posta gönderim hatası. TekrarDenenebilir: zaman aşımı, 429, 5xx, ağ hatası (birkaç kez daha denenir).
/// YapilandirmaHatasi: API anahtarı yanlış (401/403) — kalan gönderimler durdurulur.
/// </summary>
public class BildirimGonderimHatasi(string mesaj, bool tekrarDenenebilir, bool yapilandirmaHatasi = false,
                                    TimeSpan? bekleme = null) : Exception(mesaj)
{
    public bool TekrarDenenebilir { get; } = tekrarDenenebilir;
    public bool YapilandirmaHatasi { get; } = yapilandirmaHatasi;
    public TimeSpan? Bekleme { get; } = bekleme;
}

/// <summary>E-posta gönderen servislerin ortak arayüzü (gerçek API veya deneme modu).</summary>
public interface IEpostaGonderici
{
    string Ad { get; }
    bool GercekGonderim { get; }

    /// <summary>E-postayı gönderir; servisin verdiği mesaj numarasını döndürür.</summary>
    string Gonder(EpostaMesaji mesaj);
}

/// <summary>
/// Ortam değişkenine bakarak göndericiyi seçer. API anahtarı koda ve depoya yazılmaz.
///   AFET_EPOSTA_API_ANAHTARI : Brevo API anahtarı (yoksa deneme modu)
///   AFET_EPOSTA_GONDEREN     : Brevo'da doğrulanmış gönderen adresi
/// </summary>
public static class EpostaGondericiSecici
{
    public const string AnahtarDegiskeni = "AFET_EPOSTA_API_ANAHTARI";
    public const string GonderenDegiskeni = "AFET_EPOSTA_GONDEREN";

    public static IEpostaGonderici Olustur(IServiceProvider sp)
    {
        var ayar = sp.GetRequiredService<IConfiguration>();
        string? anahtar = Deger(ayar, AnahtarDegiskeni);
        string? gonderen = Deger(ayar, GonderenDegiskeni);
        if (anahtar != null && gonderen != null)
            return new BrevoEpostaGonderici(sp.GetRequiredService<IHttpClientFactory>().CreateClient("eposta"), anahtar, gonderen);
        return new DenemeModuEpostaGonderici();
    }

    /// <summary>Önce yapılandırma (işlem ortam değişkenleri dahil), sonra kullanıcı düzeyindeki ortam değişkeni.</summary>
    private static string? Deger(IConfiguration ayar, string ad)
    {
        string? d = ayar[ad];
        if (string.IsNullOrWhiteSpace(d) && OperatingSystem.IsWindows())
            d = Environment.GetEnvironmentVariable(ad, EnvironmentVariableTarget.User);
        return string.IsNullOrWhiteSpace(d) ? null : d.Trim();
    }
}

/// <summary>
/// Brevo REST API ile gerçek e-posta gönderimi.
/// POST https://api.brevo.com/v3/smtp/email  ("api-key" başlığıyla kimlik doğrulama)
/// Başarılı yanıt: 201 {"messageId":"..."}
/// </summary>
public class BrevoEpostaGonderici(HttpClient http, string anahtar, string gonderen) : IEpostaGonderici
{
    private const string ApiAdresi = "https://api.brevo.com/v3/smtp/email";

    public string Ad => "Brevo e-posta API";
    public bool GercekGonderim => true;

    public string Gonder(EpostaMesaji mesaj)
    {
        string govde = JsonSerializer.Serialize(new
        {
            sender = new { name = "Afet Erken Uyarı", email = gonderen },
            to = new[] { new { email = mesaj.Alici } },
            subject = mesaj.Konu,
            textContent = mesaj.Govde
        });
        using var istek = new HttpRequestMessage(HttpMethod.Post, ApiAdresi)
        {
            Content = new StringContent(govde, Encoding.UTF8, "application/json")
        };
        istek.Headers.Add("api-key", anahtar);
        istek.Headers.Add("Accept", "application/json");

        HttpResponseMessage yanit;
        try
        {
            yanit = http.Send(istek);
        }
        catch (OperationCanceledException)
        {
            throw new BildirimGonderimHatasi("E-posta servisi 10 saniye içinde yanıt vermedi (zaman aşımı).", true);
        }
        catch (HttpRequestException ex)
        {
            throw new BildirimGonderimHatasi("E-posta servisine bağlanılamadı: " + ex.Message, true);
        }

        using (yanit)
        {
            string icerik = new StreamReader(yanit.Content.ReadAsStream()).ReadToEnd();
            int kod = (int)yanit.StatusCode;
            if (yanit.IsSuccessStatusCode)
            {
                // Yanıtı ayrıştır: {"messageId":"<...>"}
                using var belge = JsonDocument.Parse(string.IsNullOrWhiteSpace(icerik) ? "{}" : icerik);
                return belge.RootElement.TryGetProperty("messageId", out var id) ? id.GetString() ?? "" : "(numara yok)";
            }
            if (kod is 401 or 403)
                throw new BildirimGonderimHatasi($"API anahtarı geçersiz veya yetkisiz ({kod}).", false, true);
            if (kod == 429)
                throw new BildirimGonderimHatasi("Hız sınırı aşıldı (429).", true, false, yanit.Headers.RetryAfter?.Delta);
            if (kod >= 500)
                throw new BildirimGonderimHatasi($"E-posta servisi hata verdi ({kod}).", true);
            throw new BildirimGonderimHatasi($"Gönderim reddedildi ({kod}): " + (icerik.Length > 200 ? icerik[..200] : icerik), false);
        }
    }
}

/// <summary>
/// API anahtarı yokken deneme modu: e-posta gönderilmez, .eml dosyası olarak
/// %LOCALAPPDATA%\AfetErkenUyari\GidenEpostalar klasörüne yazılır (masaüstü uygulamasıyla aynı klasör).
/// </summary>
public class DenemeModuEpostaGonderici : IEpostaGonderici
{
    public static string Klasor => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AfetErkenUyari", "GidenEpostalar");

    public string Ad => "Deneme modu - dosyaya yazılır";
    public bool GercekGonderim => false;

    public string Gonder(EpostaMesaji mesaj)
    {
        try
        {
            Directory.CreateDirectory(Klasor);
            DateTime once = DateTime.Now.AddSeconds(-1);
            using (var m = new MailMessage("bildirim@afet-uyari.local", mesaj.Alici, mesaj.Konu, mesaj.Govde))
            using (var smtp = new SmtpClient())
            {
                m.BodyEncoding = Encoding.UTF8;
                m.SubjectEncoding = Encoding.UTF8;
                smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                smtp.PickupDirectoryLocation = Klasor;
                smtp.Send(m);
            }
            string? dosya = new DirectoryInfo(Klasor).GetFiles("*.eml")
                .Where(f => f.CreationTime >= once).OrderByDescending(f => f.CreationTime).FirstOrDefault()?.Name;
            return dosya is null ? "deneme" : "dosya:" + dosya;
        }
        catch (FormatException)
        {
            throw new BildirimGonderimHatasi("Alıcı e-posta adresi geçersiz.", false);
        }
        catch (Exception ex) when (ex is IOException or SmtpException or UnauthorizedAccessException)
        {
            throw new BildirimGonderimHatasi("Deneme e-postası dosyaya yazılamadı: " + ex.Message, false);
        }
    }
}

/// <summary>Aynı servise art arda giden istekler arasında en az belirli bir süre bırakır (rate limit).</summary>
public class HizSinirlayici(TimeSpan enAzAralik)
{
    private readonly object _kilit = new();
    private DateTime _sonIstek = DateTime.MinValue;

    public void Bekle()
    {
        lock (_kilit)
        {
            TimeSpan gecen = DateTime.UtcNow - _sonIstek;
            if (gecen < enAzAralik) Thread.Sleep(enAzAralik - gecen);
            _sonIstek = DateTime.UtcNow;
        }
    }
}

/// <summary>
/// Toplu e-posta bildirimi (Hafta 9).
/// - Uyarı yayınlanınca kuyruğa alınan e-postaları sırayla gönderir; iki gönderim arasında en az 250 ms.
/// - Geçici hatada en fazla 3 deneme (artan bekleme); kalıcı hatada hemen "Başarısız".
/// - API anahtarı yanlışsa kalan gönderimler durdurulur.
/// - E-posta hatası uyarının yayınını geri almaz; uyarı uygulama içinde yine görünür.
/// </summary>
public class BildirimManager(BildirimRepository repo, UyariRepository uyariRepo, IEpostaGonderici gonderici)
{
    public const int MaksimumDeneme = 3;
    private static readonly HizSinirlayici Sinirlayici = new(TimeSpan.FromMilliseconds(250));

    public string SaglayiciAdi => gonderici.Ad;

    public BildirimSonucu UyariIcinGonder(AktifKullanici kullanici, int uyariId)
    {
        kullanici.YoneticiOlmali();
        return BekleyenleriGonder(uyariId);
    }

    public BildirimSonucu BasarisizlariYenidenDene(AktifKullanici kullanici, int uyariId)
    {
        kullanici.YoneticiOlmali();
        repo.BasarisizlariKuyrugaAl(uyariId);
        return BekleyenleriGonder(uyariId);
    }

    public List<Bildirim> Kayitlar(AktifKullanici kullanici, int uyariId)
    {
        kullanici.YoneticiOlmali();
        return repo.Listele(uyariId);
    }

    private BildirimSonucu BekleyenleriGonder(int uyariId)
    {
        var sonuc = new BildirimSonucu { SaglayiciAdi = gonderici.Ad };
        var bekleyenler = repo.Bekleyenler(uyariId);
        if (bekleyenler.Count == 0) return sonuc;
        Uyari uyari = uyariRepo.Getir(uyariId) ?? throw new IsKuraliHatasi("Uyarı bulunamadı.");

        string? durdurmaNedeni = null;
        foreach (var b in bekleyenler)
        {
            sonuc.Toplam++;
            if (durdurmaNedeni != null)
            {
                repo.SonucYaz(b.BildirimId, BildirimDurumlari.Basarisiz, b.DenemeSayisi, durdurmaNedeni, null);
                sonuc.Basarisiz++;
                continue;
            }

            var mesaj = MesajOlustur(uyari, b);
            int deneme = b.DenemeSayisi;
            for (int i = 1; ; i++)
            {
                deneme++;
                try
                {
                    Sinirlayici.Bekle();
                    string mesajId = gonderici.Gonder(mesaj);
                    repo.SonucYaz(b.BildirimId,
                        gonderici.GercekGonderim ? BildirimDurumlari.Gonderildi : BildirimDurumlari.DenemeModu, deneme, null, mesajId);
                    if (gonderici.GercekGonderim) sonuc.Gonderilen++; else sonuc.DenemeModu++;
                    break;
                }
                catch (BildirimGonderimHatasi h)
                {
                    if (h.TekrarDenenebilir && i < MaksimumDeneme)
                    {
                        // Artan bekleme: 1 sn, 2 sn ... (servis Retry-After verdiyse o kadar, en fazla 10 sn)
                        var bekle = h.Bekleme ?? TimeSpan.FromSeconds(i);
                        Thread.Sleep(bekle > TimeSpan.FromSeconds(10) ? TimeSpan.FromSeconds(10) : bekle);
                        continue;
                    }
                    repo.SonucYaz(b.BildirimId, BildirimDurumlari.Basarisiz, deneme, h.Message, null);
                    sonuc.Basarisiz++;
                    if (h.YapilandirmaHatasi) durdurmaNedeni = "Gönderim durduruldu: " + h.Message;
                    break;
                }
            }
        }
        return sonuc;
    }

    /// <summary>Uyarıdan sade bir e-posta metni üretir.</summary>
    public static EpostaMesaji MesajOlustur(Uyari u, Bildirim b)
    {
        string seviye = UyariSeviyeleri.Ad(u.Seviye).ToUpper(new CultureInfo("tr-TR"));
        var sb = new StringBuilder();
        sb.AppendLine($"Merhaba {b.KullaniciAdi},");
        sb.AppendLine();
        sb.AppendLine($"Abone olduğunuz bölge için yeni bir {seviye} seviyesinde uyarı yayınlandı.");
        sb.AppendLine();
        sb.AppendLine("Başlık   : " + u.Baslik);
        sb.AppendLine("Afet türü: " + u.AfetTuru);
        sb.AppendLine("Bölgeler : " + u.HedefBolgeler);
        sb.AppendLine("Yayın    : " + u.YayinTarihi.ToString("dd.MM.yyyy HH:mm"));
        sb.AppendLine("Geçerlilik: " + (u.BitisTarihi?.ToString("dd.MM.yyyy HH:mm") ?? "Süresiz"));
        sb.AppendLine();
        sb.AppendLine(u.Mesaj);
        sb.AppendLine();
        sb.AppendLine("Hazırlık rehberi ve toplanma alanları için uygulamaya giriş yapabilirsiniz.");
        sb.AppendLine("Bu e-postayı bildirim kanalı olarak E-posta seçtiğiniz için aldınız; tercihinizi Aboneliklerim sayfasından değiştirebilirsiniz.");
        return new EpostaMesaji(b.Alici, $"[{seviye}] {u.Baslik} - Afet Erken Uyarı", sb.ToString());
    }
}

/// <summary>
/// Harita API entegrasyonu: OpenStreetMap Nominatim ile adresten koordinat bulma (geocoding).
/// GET {sunucu}/search?format=json&amp;limit=1&amp;countrycodes=tr&amp;q=...
/// - Anahtar gerektirmez; kullanım kuralı gereği uygulamayı tanıtan User-Agent gönderilir ("harita" HttpClient'ı).
/// - Hız sınırı: saniyede en fazla 1 istek. Zaman aşımı 10 sn.
/// - Farklı bir sunucu AFET_HARITA_URL ile verilebilir.
/// </summary>
public class HaritaServisi(IHttpClientFactory httpFabrikasi, IConfiguration ayar)
{
    private static readonly HizSinirlayici Sinirlayici = new(TimeSpan.FromMilliseconds(1100));

    public KonumSonucu AdresBul(string? adres)
    {
        adres = (adres ?? "").Trim();
        if (adres.Length < 5) throw new IsKuraliHatasi("Konum aramak için önce adresi (en az 5 karakter) yazınız.");

        string sunucu = (ayar["AFET_HARITA_URL"] is { Length: > 0 } s ? s : "https://nominatim.openstreetmap.org").TrimEnd('/');
        string url = $"{sunucu}/search?format=json&limit=1&countrycodes=tr&q={Uri.EscapeDataString(adres)}";

        Sinirlayici.Bekle();
        HttpResponseMessage yanit;
        try
        {
            yanit = httpFabrikasi.CreateClient("harita").Send(new HttpRequestMessage(HttpMethod.Get, url));
        }
        catch (OperationCanceledException)
        {
            throw new IsKuraliHatasi("Harita servisi 10 saniye içinde yanıt vermedi. Daha sonra tekrar deneyiniz.");
        }
        catch (HttpRequestException)
        {
            throw new IsKuraliHatasi("Harita servisine ulaşılamadı. İnternet bağlantınızı kontrol ediniz.");
        }

        using (yanit)
        {
            if ((int)yanit.StatusCode == 429)
                throw new IsKuraliHatasi("Harita servisine çok sık istek gönderildi. Birkaç saniye bekleyip tekrar deneyiniz.");
            if (!yanit.IsSuccessStatusCode)
                throw new IsKuraliHatasi($"Harita servisi hata verdi ({(int)yanit.StatusCode}).");
            return Ayristir(new StreamReader(yanit.Content.ReadAsStream()).ReadToEnd(), adres);
        }
    }

    /// <summary>Örnek yanıt: [{"lat":"38.8847","lon":"40.4939","display_name":"Bingöl, Türkiye", ...}]</summary>
    public static KonumSonucu Ayristir(string json, string aranan)
    {
        try
        {
            using var belge = JsonDocument.Parse(json);
            if (belge.RootElement.ValueKind != JsonValueKind.Array || belge.RootElement.GetArrayLength() == 0)
                throw new IsKuraliHatasi($"\"{aranan}\" için konum bulunamadı. Adresi il/ilçe ekleyerek tekrar deneyiniz.");

            var ilk = belge.RootElement[0];
            if (!ilk.TryGetProperty("lat", out var lat) || !ilk.TryGetProperty("lon", out var lon) ||
                !decimal.TryParse(lat.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal enlem) ||
                !decimal.TryParse(lon.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal boylam))
                throw new IsKuraliHatasi("Harita servisinin yanıtında koordinat bulunamadı.");

            string bulunan = ilk.TryGetProperty("display_name", out var ad) ? ad.GetString() ?? aranan : aranan;
            return new KonumSonucu(Math.Round(enlem, 6), Math.Round(boylam, 6), bulunan);
        }
        catch (JsonException)
        {
            throw new IsKuraliHatasi("Harita servisinden beklenmeyen bir yanıt geldi.");
        }
    }

    /// <summary>Koordinatı OpenStreetMap'te işaretli gösteren bağlantı.</summary>
    public static string HaritaBaglantisi(decimal enlem, decimal boylam)
    {
        string e = enlem.ToString("0.######", CultureInfo.InvariantCulture);
        string b = boylam.ToString("0.######", CultureInfo.InvariantCulture);
        return $"https://www.openstreetmap.org/?mlat={e}&mlon={b}#map=17/{e}/{b}";
    }
}
