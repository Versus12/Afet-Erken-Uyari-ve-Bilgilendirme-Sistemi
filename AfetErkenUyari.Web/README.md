# Afet Erken Uyarı ve Bilgilendirme Sistemi — Web Sitesi

P14 projesinin **ASP.NET Core MVC (.NET 9, C#)** ile hazırlanmış web sürümü. Kapsam: **Hafta 3 – 8** (vize ara sürümü).
Masaüstü (Windows Forms) uygulamasıyla **aynı veritabanını** (`AfetBilgilendirme`) kullanır; yönetici web'den
uyarı yayınlarsa vatandaş masaüstü uygulamasında da görür.

## Özellikler
| Modül | Vatandaş | Yönetici | Sayfa |
|---|---|---|---|
| Kayıt / giriş (BCrypt, çerezli oturum, 30 dk zaman aşımı, hatalı giriş kilidi) | ✅ | ✅ | `/Hesap/Kayit`, `/Hesap/Giris` |
| Uyarılarım + uygulama içi bildirim (60 sn'de bir kontrol, yeni uyarıda ses + bildirim) | ✅ | ✅ | `/Panel` |
| Bölge aboneliği + tercih (kanal, minimum seviye, aktif/pasif) | ✅ | ✅ | `/Abonelik` |
| Hazırlık rehberi (afet türü × önce / sırasında / sonra) | okur | okur + düzenler | `/Rehber` |
| Toplanma alanları (bölge ve "abone olduğum bölgeler" filtresi) | okur | okur + düzenler | `/ToplanmaAlani` |
| Bölge yönetimi | – | ✅ | `/Bolge` |
| Uyarı yayını (çoklu bölge hedefleme, geçerlilik süresi, yayından kaldırma) | – | ✅ | `/Uyari` |

## Mimari
```
AfetErkenUyari.Web/
├── Entities/      Varlıklar (Kullanici, Bolge, Abonelik, Uyari, HazirlikRehberi, ToplanmaAlani) + sabit listeler
├── Data/          Veri erişimi: VeritabaniBaglanti + *Repository (ADO.NET, parametreli SQL, transaction)
├── Business/      İş kuralları + yetki: *Manager, KimlikDogrulamaManager, GirisDenemeTakibi, AktifKullanici
├── Controllers/   Sunum katmanı (MVC): Hesap, Panel, Abonelik, Rehber, ToplanmaAlani, Bolge, Uyari
├── Models/        Form / sayfa modelleri, ondalık sayı bağlayıcı
├── Views/         Razor sayfaları (Bootstrap 5)
└── Program.cs     Bağımlılık enjeksiyonu, çerez kimlik doğrulama, CSRF filtresi
```
Akış: **Controller → Manager (doğrulama + yetki) → Repository (SQL) → SQL Server**.
Yetki iki katmanda kontrol edilir: controller'da `[Authorize(Policy = "Yonetici")]` ve iş katmanında
`AktifKullanici.YoneticiOlmali()`. Ayrıntı: [Docs/Web-Mimari-ve-Guvenlik.md](Docs/Web-Mimari-ve-Guvenlik.md)

## Kurulum ve çalıştırma
1. **Veritabanı** (masaüstü uygulamasıyla ortak; bir kez yeterli):
   ```bash
   sqlcmd -S ".\SQLEXPRESS" -E -C -f 65001 -i "afet erken uyarı ve bilgilendirme sistemi/Database/01_Sema.sql"
   sqlcmd -S ".\SQLEXPRESS" -E -C -f 65001 -i "afet erken uyarı ve bilgilendirme sistemi/Database/02_OrnekVeri.sql"
   ```
2. `appsettings.json` içindeki `ConnectionStrings:AfetDB` değerinde `Data Source` kısmını kendi SQL Server adınızla değiştirin
   (veya `ConnectionStrings__AfetDB` ortam değişkenini tanımlayın).
3. **Visual Studio 2022:** çözümü açın, Solution Explorer'da `AfetErkenUyari.Web` projesine sağ tıklayıp
   *Set as Startup Project* seçin, **F5**. (Visual Studio Installer'da "ASP.NET ve web geliştirme" iş yükü kurulu olmalıdır.)
   **Komut satırı:**
   ```bash
   dotnet run --project AfetErkenUyari.Web
   ```
   Site: http://localhost:5283

### Test hesapları (yalnızca geliştirme ortamı, sentetik veri)
| Kullanıcı adı | Şifre | Rol |
|---|---|---|
| `yonetici` | `Yonetici123!` | Yönetici |
| `vatandas1` | `Vatandas123!` | Vatandaş |
| `vatandas2` | `Vatandas123!` | Vatandaş |

## Belgeler
- [Haftalık ilerleme (Hafta 3–8, web)](Docs/Web-Haftalik-Ilerleme.md)
- [Mimari ve güvenlik notu](Docs/Web-Mimari-ve-Guvenlik.md)
- [Vize demo senaryosu (web)](Docs/Web-Demo-Senaryosu.md)
- Ortak: ER diyagramı, veritabanı scriptleri, AI kullanım günlüğü → `../afet erken uyarı ve bilgilendirme sistemi/Docs`
