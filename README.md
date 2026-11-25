# Afet Erken Uyarı ve Bilgilendirme Sistemi

Vatandaşların bölge bazlı afet uyarılarını ve hazırlık yönergelerini tek bir yerden almasını sağlayan,
yerel yönetimlerin de uyarı yayınlayabildiği **Windows Forms (.NET Framework 4.7.2)** masaüstü uygulaması.

> Bilgisayar Uygulamaları I – Proje Tabanlı Ders · **P14** · Sürüm **v0.10.0 (Hafta 3–10)**

Çözümde iki istemci vardır; ikisi de aynı SQL Server veritabanını (`AfetBilgilendirme`) kullanır:

| Proje | Teknoloji | Klasör |
|---|---|---|
| Masaüstü uygulaması | Windows Forms, .NET Framework 4.7.2 | `afet erken uyarı ve bilgilendirme sistemi/` |
| Web sitesi | ASP.NET Core MVC, .NET 9 | [`AfetErkenUyari.Web/`](AfetErkenUyari.Web/README.md) |

Web sitesinin kurulumu ve sayfaları için: [AfetErkenUyari.Web/README.md](AfetErkenUyari.Web/README.md)

## Özellikler
| Modül | Vatandaş | Yönetici |
|---|---|---|
| Kayıt / giriş (BCrypt, rol, oturum zaman aşımı, hatalı giriş kilidi) | ✅ | ✅ |
| Bölge aboneliği + tercih (kanal: Uygulama içi / E-posta, minimum seviye, durdur/başlat) | ✅ | ✅ |
| Uyarılarım + uygulama içi bildirim (60 sn'de bir yenileme, sesli uyarı) | ✅ | ✅ |
| E-posta bildirimi (kayıtlı adrese; Brevo API veya deneme modu) — Hafta 9 | alır | gönderir + kayıtları izler |
| Harita API: adresten konum bulma (OpenStreetMap Nominatim), haritada aç — Hafta 9 | haritada açar | ✅ |
| Yapay zekâ: kurum duyurusundan bölgeye özel sade uyarı taslağı (Claude) + doğrulama + AI günlüğü — Hafta 10 | – | ✅ |
| Hazırlık rehberi (afet türü × önce/sırasında/sonra) | okur | okur + düzenler |
| Toplanma alanları (bölge filtresi, kapasite) | okur | okur + düzenler |
| Bölge yönetimi | – | ✅ |
| Uyarı yayını (çoklu bölge hedefleme, geçerlilik süresi, yayından kaldırma) | – | ✅ |

## Kullanılan teknolojiler
C# · .NET Framework 4.7.2 · Windows Forms · Microsoft SQL Server (Express) · ADO.NET (`System.Data.SqlClient`) · BCrypt.Net-Next · Visual Studio 2022

## Proje mimarisi
```
afet erken uyarı ve bilgilendirme sistemi/
├── Entities/     Veri modelleri (Kullanici, Bolge, Abonelik, Uyari, HazirlikRehberi, ToplanmaAlani, Sabitler)
├── Data/         Veri erişimi (VeritabaniBaglanti + *Repository, parametreli SQL)
├── Business/     İş kuralları + yetki (Oturum, KimlikDogrulamaManager, *Manager)
├── Frm*.cs       Sunum katmanı (Windows Forms ekranları) + ArayuzYardimci
├── Database/     01_Sema.sql, 02_OrnekVeri.sql, 03_TestSorgulari.sql
└── Docs/         ER diyagramı, mimari şema, güvenlik notu, haftalık ilerleme, demo, vize raporu, AI günlüğü
```
Ayrıntı: [Docs/Mimari-Sema.md](afet%20erken%20uyar%C4%B1%20ve%20bilgilendirme%20sistemi/Docs/Mimari-Sema.md) ·
[Docs/ER-Diyagrami.md](afet%20erken%20uyar%C4%B1%20ve%20bilgilendirme%20sistemi/Docs/ER-Diyagrami.md)

## Kurulum
1. **SQL Server** (Express yeterli) ve **Visual Studio 2022** (".NET masaüstü geliştirme" iş yükü) kurulu olmalı.
2. Veritabanını oluşturun (SSMS'te dosyaları açıp `F5`, ya da komut satırından):
   ```bash
   sqlcmd -S ".\SQLEXPRESS" -E -C -f 65001 -i "afet erken uyarı ve bilgilendirme sistemi/Database/01_Sema.sql"
   sqlcmd -S ".\SQLEXPRESS" -E -C -f 65001 -i "afet erken uyarı ve bilgilendirme sistemi/Database/02_OrnekVeri.sql"
   ```
3. `App.config` içindeki `AfetDB` bağlantısında `Data Source` değerini kendi SQL Server örneğinizle değiştirin.
4. `afet erken uyarı ve bilgilendirme sistemi.sln` dosyasını açın; NuGet paketleri otomatik geri yüklenir. `F5` ile çalıştırın.

### Hafta 9 · Harici API ayarları (ortam değişkenleri)
API anahtarları koda ve depoya yazılmaz. Tanımlı değilse uygulama yine çalışır: e-postalar **deneme modunda**
`%LOCALAPPDATA%\AfetErkenUyari\GidenEpostalar` klasörüne `.eml` dosyası olarak yazılır.

| Değişken | Anlamı |
|---|---|
| `AFET_EPOSTA_API_ANAHTARI` | Brevo API anahtarı (gerçek e-posta gönderimi için) |
| `AFET_EPOSTA_GONDEREN` | Brevo'da doğrulanmış gönderen e-posta adresi |
| `AFET_HARITA_URL` | (İsteğe bağlı) farklı bir Nominatim sunucusu; varsayılan `https://nominatim.openstreetmap.org` |
| `ANTHROPIC_API_KEY` | (Hafta 10) Claude API anahtarı. Yoksa sade uyarı taslağı **kural tabanlı** yöntemle üretilir. |

```bash
setx AFET_EPOSTA_API_ANAHTARI "xkeysib-..."
setx AFET_EPOSTA_GONDEREN "adiniz@ornek.com"
```

### Test hesapları (yalnızca geliştirme ortamı, sentetik veri)
| Kullanıcı adı | Şifre | Rol |
|---|---|---|
| `yonetici` | `Yonetici123!` | Yönetici |
| `vatandas1` | `Vatandas123!` | Vatandaş (Bingöl/Merkez, Bingöl/Solhan abonesi) |
| `vatandas2` | `Vatandas123!` | Vatandaş (Rize/Merkez, Elazığ/Merkez abonesi) |

## Veritabanı
Veritabanı adı: **AfetBilgilendirme** — Tablolar: `Kullanicilar`, `Bolgeler`, `Abonelikler` (N-N), `Uyarilar`,
`UyariBolgeleri` (N-N), `HazirlikRehberi`, `ToplanmaAlanlari`, `Bildirimler` (Hafta 9 — e-posta kuyruğu ve gönderim kaydı),
`YapayZekaKayitlari` (Hafta 10 — AI günlüğü: istem, ham çıktı, doğrulama).

## Proje belgeleri
- [Haftalık ilerleme (Hafta 3–10)](afet%20erken%20uyar%C4%B1%20ve%20bilgilendirme%20sistemi/Docs/Haftalik-Ilerleme.md)
- [Güvenlik notu](afet%20erken%20uyar%C4%B1%20ve%20bilgilendirme%20sistemi/Docs/Guvenlik-Notu.md)
- [Vize ilerleme raporu](afet%20erken%20uyar%C4%B1%20ve%20bilgilendirme%20sistemi/Docs/Vize-Ilerleme-Raporu.md)
- [Demo senaryosu](afet%20erken%20uyar%C4%B1%20ve%20bilgilendirme%20sistemi/Docs/Demo-Senaryosu.md)
- [AI kullanım günlüğü](afet%20erken%20uyar%C4%B1%20ve%20bilgilendirme%20sistemi/Docs/AI-Kullanim-Gunlugu.md)
- [Sözlü sınav hazırlık](afet%20erken%20uyar%C4%B1%20ve%20bilgilendirme%20sistemi/Docs/Sozlu-Sinav-Hazirlik.md)

## Geliştirme durumu
Hafta 3–10 tamamlandı (vize ara sürümü `v0.8.0`, Hafta 9 `v0.9.0`, Hafta 10 `v0.10.0`). Sıradaki:
en yakın toplanma alanı (H11), yönetim panosu (H12).
