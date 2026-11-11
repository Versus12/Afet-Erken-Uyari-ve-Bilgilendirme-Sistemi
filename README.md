# Afet Erken Uyarı ve Bilgilendirme Sistemi

Vatandaşların bölge bazlı afet uyarılarını ve hazırlık yönergelerini tek bir yerden almasını sağlayan,
yerel yönetimlerin de uyarı yayınlayabildiği **Windows Forms (.NET Framework 4.7.2)** masaüstü uygulaması.

> Bilgisayar Uygulamaları I – Proje Tabanlı Ders · **P14** · Sürüm **v0.8.0 (Vize ara sürümü, Hafta 3–8)**

## Özellikler
| Modül | Vatandaş | Yönetici |
|---|---|---|
| Kayıt / giriş (BCrypt, rol, oturum zaman aşımı, hatalı giriş kilidi) | ✅ | ✅ |
| Bölge aboneliği + tercih (kanal, minimum seviye, durdur/başlat) | ✅ | ✅ |
| Uyarılarım + uygulama içi bildirim (60 sn'de bir yenileme, sesli uyarı) | ✅ | ✅ |
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

### Test hesapları (yalnızca geliştirme ortamı, sentetik veri)
| Kullanıcı adı | Şifre | Rol |
|---|---|---|
| `yonetici` | `Yonetici123!` | Yönetici |
| `vatandas1` | `Vatandas123!` | Vatandaş (Bingöl/Merkez, Bingöl/Solhan abonesi) |
| `vatandas2` | `Vatandas123!` | Vatandaş (Rize/Merkez, Elazığ/Merkez abonesi) |

## Veritabanı
Veritabanı adı: **AfetBilgilendirme** — Tablolar: `Kullanicilar`, `Bolgeler`, `Abonelikler` (N-N), `Uyarilar`,
`UyariBolgeleri` (N-N), `HazirlikRehberi`, `ToplanmaAlanlari`.

## Proje belgeleri
- [Haftalık ilerleme (Hafta 3–8)](afet%20erken%20uyar%C4%B1%20ve%20bilgilendirme%20sistemi/Docs/Haftalik-Ilerleme.md)
- [Güvenlik notu](afet%20erken%20uyar%C4%B1%20ve%20bilgilendirme%20sistemi/Docs/Guvenlik-Notu.md)
- [Vize ilerleme raporu](afet%20erken%20uyar%C4%B1%20ve%20bilgilendirme%20sistemi/Docs/Vize-Ilerleme-Raporu.md)
- [Demo senaryosu](afet%20erken%20uyar%C4%B1%20ve%20bilgilendirme%20sistemi/Docs/Demo-Senaryosu.md)
- [AI kullanım günlüğü](afet%20erken%20uyar%C4%B1%20ve%20bilgilendirme%20sistemi/Docs/AI-Kullanim-Gunlugu.md)
- [Sözlü sınav hazırlık](afet%20erken%20uyar%C4%B1%20ve%20bilgilendirme%20sistemi/Docs/Sozlu-Sinav-Hazirlik.md)

## Geliştirme durumu
Hafta 3–8 tamamlandı (vize ara sürümü `v0.8.0`). Sıradaki: harita/bildirim API (H9), yapay zekâ ile sade uyarı metni (H10),
en yakın toplanma alanı (H11), yönetim panosu (H12).
