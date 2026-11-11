# Vize Ara Sürüm İlerleme Raporu — v0.8.0

**Proje:** P14 · Afet Erken Uyarı ve Bilgilendirme Sistemi
**Kapsam:** Hafta 3 – 8
**Sürüm:** `v0.8.0` (semantic versioning: 0 = henüz kararlı değil, 8 = vize haftası, 0 = yama)

## 1. Tamamlanan modüller
| Modül | Durum | Ekran | İş sınıfı |
|---|---|---|---|
| Kimlik doğrulama / rol | ✅ | FrmGiris, FrmKayit | KimlikDogrulamaManager, Oturum |
| Bölge | ✅ | FrmBolgeYonetimi | BolgeManager |
| Abonelik + tercih | ✅ | FrmAbonelik | AbonelikManager |
| Uyarı yayını + hedefleme | ✅ | FrmUyariYayini | UyariManager |
| Hazırlık rehberi | ✅ | FrmHazirlikRehberi | RehberManager |
| Uygulama içi bildirim | ✅ (temel) | FrmAnaMenu | UyariManager.Uyarilarim |
| Toplanma alanı | ⏳ liste/CRUD hazır, harita Hafta 11 | FrmToplanmaAlanlari | ToplanmaAlaniManager |

## 2. Hangi modül hangi veriyi nereden alıyor?
- **Ana menü** → `UyariManager.Uyarilarim()` → `Uyarilar ⋈ UyariBolgeleri ⋈ Abonelikler` (kullanıcının aktif aboneliği + seviye ≥ tercih + süresi dolmamış).
- **Abonelik** → `Bolgeler` (seçim listesi) + `Abonelikler` (kendi kayıtları).
- **Uyarı yayını** → `Bolgeler` (hedef liste), yazarken `Uyarilar` + `UyariBolgeleri` (transaction), sonra `Abonelikler` ile alıcı sayısı.
- **Toplanma alanları** → `ToplanmaAlanlari ⋈ Bolgeler`, isteğe bağlı olarak abonelik modülünden gelen bölge kimlikleriyle filtre.

## 3. Giderilen hatalar (Hafta 3–7 kodundan)
| # | Sorun | Çözüm |
|---|---|---|
| 1 | Projede iki farklı ad alanı vardı (`afet_erken_uyarı…` ve `afet_erken_uyari…`); sınıflar birbirini göremiyordu. | Tüm kod tek kök ad alanı + `.Entities/.Data/.Business` alt ad alanlarına taşındı. |
| 2 | Kayıt formu `Eposta`, veri sınıfı `Email` sütununa yazıyordu; DB'de yalnızca `Eposta` vardı. | Tek `KullaniciRepository`, tek sütun adı (`Eposta`). |
| 3 | Bağlantı cümlesi üç farklı yerde koda gömülüydü. | Yalnızca `App.config > AfetDB`; `VeritabaniBaglanti` tek erişim noktası. |
| 4 | Giriş başarılı olunca hiçbir ekran açılmıyordu. | `Program.cs` akışı: Giriş → Ana Menü → Çıkışta tekrar Giriş. |
| 5 | Yönetici yetkisi hiç kontrol edilmiyordu. | `Oturum.YoneticiGerekli()` iş katmanında. |
| 6 | Manager sınıfları boş gövdeliydi. | Tüm CRUD + doğrulama yazıldı. |
| 7 | Kullanılmayan ~30 NuGet paketi (JWT, Caching, SqlClient 7…) vardı. | Yalnızca BCrypt ve bağımlılıkları bırakıldı. |
| 8 | Arka plan yenilemesi oturum süresini sonsuza uzatıyordu. | `Oturum.GirisGerekli(sureyiYenile:false)` |
| 9 | Oturum düşünce ana menü kapanıp uygulama tamamen çıkıyordu; bağlantı kopunca her dakika hata penceresi açılıyordu. | Giriş ekranına dönüş + arka plan hatası durum çubuğuna yazılır. |
| 10 | Boş şablon formlar (Form1/2/3), içi boş sınıflar (`App`, `Application_Configuration_File`), kopya README klasörü ve kullanılmayan metotlar vardı. | Projeden kaldırıldı. |

## 4. Test sonucu
İş katmanı form olmadan, gerçek veritabanına karşı 25 senaryoyla denendi — **25/25 geçti**:
hatalı şifre, oturumsuz erişim, zayıf şifre, BCrypt hash, tekrar kayıt, tekrar abonelik, vatandaşın yönetici işlemi yapamaması,
min. seviye filtresi, bağlı bölge silme, bölgesiz uyarı, alıcı sayısı, pasif abonelik, iptal, rehber/alan listeleri, hesap kilidi,
30 dk oturum zaman aşımı, arka plan yenilemesinin oturumu uzatmaması, bozuk şifre hash'i.
Ayrıca 8 form hem yönetici hem vatandaş hesabıyla açılıp kapatıldı (16/16).

## 5. Bilinen sınırlamalar
- Bildirim yalnızca uygulama içi; SMS/e-posta kanalı tercih olarak kaydediliyor, gönderim Hafta 9'da.
- Hatalı giriş kilidi bellekte; uygulama kapanınca sıfırlanır.
- Otomatik birim test projesi Hafta 13'te eklenecek.

## 6. Sonraki aşama
Hafta 9: harita + bildirim API · Hafta 10: AI ile sade uyarı metni · Hafta 11: en yakın toplanma alanı · Hafta 12: istatistik panosu.
