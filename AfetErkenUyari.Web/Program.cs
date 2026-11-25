using AfetErkenUyari.Web.Business;
using AfetErkenUyari.Web.Data;
using System.Globalization;
using AfetErkenUyari.Web.Entities;
using AfetErkenUyari.Web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------- Sunum katmanı (MVC)
builder.Services.AddControllersWithViews(o =>
{
    // Tüm POST formlarında CSRF (anti-forgery) doğrulaması
    o.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    // Enlem/boylam: "38,88" ve "38.88" yazımlarının ikisini de kabul et
    o.ModelBinderProviders.Insert(0, new OndalikModelBinderSaglayici());
});

// Tarih ve sayılar Türkçe biçimde gösterilir (12.11.2026 14:30)
var turkce = new CultureInfo("tr-TR");
CultureInfo.DefaultThreadCurrentCulture = turkce;
CultureInfo.DefaultThreadCurrentUICulture = turkce;

// ---------------------------------------------------------------- Kimlik doğrulama (Hafta 5)
// Web'de oturum: şifrelenmiş, HttpOnly kimlik çerezi. 30 dk işlem yapılmazsa oturum düşer.
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Hesap/Giris";
        o.LogoutPath = "/Hesap/Cikis";
        o.AccessDeniedPath = "/Hesap/ErisimEngellendi";
        o.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        o.SlidingExpiration = true;
        o.Cookie.Name = "AfetUyari.Oturum";
        o.Cookie.HttpOnly = true;
        o.Cookie.SameSite = SameSiteMode.Lax;
        o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        o.Events.OnCheckSlidingExpiration = ctx =>
        {
            // Panelin arka plandaki otomatik yenileme isteği oturum süresini uzatmaz.
            if (ctx.Request.Path.StartsWithSegments("/Panel/UyariOzeti")) ctx.ShouldRenew = false;
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("Yonetici", p => p.RequireRole(Roller.Yonetici));
});

// ---------------------------------------------------------------- Veri ve iş katmanı
builder.Services.AddSingleton<VeritabaniBaglanti>();
builder.Services.AddSingleton<GirisDenemeTakibi>();
builder.Services.AddScoped<KullaniciRepository>();
builder.Services.AddScoped<BolgeRepository>();
builder.Services.AddScoped<AbonelikRepository>();
builder.Services.AddScoped<UyariRepository>();
builder.Services.AddScoped<RehberRepository>();
builder.Services.AddScoped<ToplanmaAlaniRepository>();
builder.Services.AddScoped<KimlikDogrulamaManager>();
builder.Services.AddScoped<BolgeManager>();
builder.Services.AddScoped<AbonelikManager>();
builder.Services.AddScoped<UyariManager>();
builder.Services.AddScoped<RehberManager>();
builder.Services.AddScoped<ToplanmaAlaniManager>();

// ---------------------------------------------------------------- Hafta 9: harici API'ler
// E-posta API'si (Brevo) ve harita API'si (OpenStreetMap Nominatim) için HttpClient'lar: 10 sn zaman aşımı.
builder.Services.AddHttpClient("eposta", c => c.Timeout = TimeSpan.FromSeconds(10));
builder.Services.AddHttpClient("harita", c =>
{
    c.Timeout = TimeSpan.FromSeconds(10);
    c.DefaultRequestHeaders.UserAgent.ParseAdd("AfetErkenUyari-P14/0.9 (ogrenci projesi)");
    c.DefaultRequestHeaders.AcceptLanguage.ParseAdd("tr");
});
// API anahtarı ortam değişkeninde (AFET_EPOSTA_API_ANAHTARI) varsa gerçek gönderim, yoksa deneme modu.
builder.Services.AddScoped<IEpostaGonderici>(EpostaGondericiSecici.Olustur);
builder.Services.AddScoped<BildirimRepository>();
builder.Services.AddScoped<BildirimManager>();
builder.Services.AddScoped<HaritaServisi>();

// ---------------------------------------------------------------- Hafta 10: yapay zekâ bileşeni
// ANTHROPIC_API_KEY ortam değişkeni varsa Claude kullanılır; yoksa kural tabanlı taslak üretilir.
builder.Services.AddSingleton(sp => new YapayZekaSecimi(ClaudeIstemcisi.Olustur(sp.GetRequiredService<IConfiguration>())));
builder.Services.AddScoped<YapayZekaRepository>();
builder.Services.AddScoped<SadeUyariManager>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Hata");
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/Home/Durum/{0}");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
