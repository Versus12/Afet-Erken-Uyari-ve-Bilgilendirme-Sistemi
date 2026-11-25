# Web Sitesi — Haftalık İlerleme (Hafta 3 – 11)

Kaynak: *P14 — Afet Erken Uyarı ve Bilgilendirme Sistemi* ders paketi. Hafta 12 ve sonrası bu sürümün kapsamı dışındadır.

## Hafta 3 · Kurulum, Mimari ve Veri Modeli ✅
- [x] Visual Studio 2022 + .NET 9 SDK, ASP.NET Core MVC proje iskeleti (`AfetErkenUyari.Web`), çözüme eklendi
- [x] Katmanlı klasör yapısı: `Entities/`, `Data/`, `Business/`, `Controllers/` + `Views/` (sunum)
- [x] GitHub deposu ve `.gitignore` (ortak), web README'si
- [x] ER diyagramı (ortak veritabanı) → `../../afet erken uyarı ve bilgilendirme sistemi/Docs/ER-Diyagrami.md`
- [x] Mimari şema → [Web-Mimari-ve-Guvenlik.md](Web-Mimari-ve-Guvenlik.md)

## Hafta 4 · Veritabanı Tasarımı ve Veri Erişim Katmanı ✅
- [x] Şema, ilişkiler, kısıtlar, indeksler ve sentetik veri: ortak `Database/01_Sema.sql`, `02_OrnekVeri.sql`, `03_TestSorgulari.sql`
- [x] `Data/*Repository.cs`: parametreli SQL ile CRUD (Microsoft.Data.SqlClient)
- [x] Uyarı + hedef bölgeler tek **transaction** içinde yazılır (`UyariRepository.Ekle`)
- [x] Bağlantı cümlesi koda gömülmez: `appsettings.json` / ortam değişkeni

## Hafta 5 · Kullanıcı Yönetimi ve Kimlik Doğrulama ✅
- [x] Kayıt ve giriş sayfaları; parola **BCrypt** ile hash + salt
- [x] Oturum: şifreli, `HttpOnly` kimlik çerezi; 30 dk hareketsizlikte düşer (kayan süre)
- [x] Rol tabanlı erişim (Vatandaş / Yönetici): `[Authorize(Policy = "Yonetici")]` **ve** iş katmanında `YoneticiOlmali()`
- [x] 5 hatalı denemede 2 dk kilit; "kullanıcı yok" ve "şifre yanlış" için aynı mesaj
- [x] CSRF koruması, açık yönlendirme (open redirect) koruması
- [x] Güvenlik notu → [Web-Mimari-ve-Guvenlik.md](Web-Mimari-ve-Guvenlik.md)

## Hafta 6 · Çekirdek Modül A: Bölge ve Abonelik ✅
- [x] Bölge modeli + yönetici CRUD sayfası (`/Bolge`)
- [x] Abonelik ilişkisi (Kullanıcı–Bölge N-N) ve tercih yönetimi (`/Abonelik`)
- [x] İş kuralları: aynı bölgeye tekrar abonelik yok, en fazla 10 abonelik, sahiplik kontrolü,
      bağlı kaydı olan bölge silinemez, il/ilçe tekil, koordinat aralıkları

## Hafta 7 · Çekirdek Modül B: Uyarı Yayını ve Hazırlık Rehberi ✅
- [x] Uyarı modeli; bir veya birden çok bölgeye hedefleme, "tümünü seç" (`/Uyari`)
- [x] Yayın öncesi onay, yayın sonrası "kaç aboneye ulaşacak" bilgisi, yayından kaldırma
- [x] Hazırlık rehberi: afet türüne göre filtre; önce / sırasında / sonra sütunları; yönetici düzenler (`/Rehber`)
- [x] İş kuralları: en az bir bölge, başlık/mesaj uzunluğu, bitiş > yayın, en fazla 30 gün geçerlilik

## Hafta 8 · VİZE: Ara Sürüm, Entegrasyon ve Sözlü Hazırlık ✅
- [x] Uçtan uca entegrasyon: giriş → abonelik → panelde bölge bazlı uyarılar → rehber ve toplanma alanı bağlantıları
- [x] Uygulama içi bildirim: panel 60 sn'de bir `/Panel/UyariOzeti` adresini kontrol eder (bu istek oturumu uzatmaz)
- [x] Toplanma alanları listesi (harita Hafta 11'de)
- [x] Masaüstü uygulamasıyla ortak veritabanı: iki istemci aynı veriyi görür
- [x] Tarayıcıda uçtan uca deneme (bkz. aşağıdaki test listesi)
- [x] Demo senaryosu → [Web-Demo-Senaryosu.md](Web-Demo-Senaryosu.md)
- [ ] Git etiketi: commit sonrası `v0.8.0-web`

## Hafta 9 · Harici API Entegrasyonu: Harita / Bildirim ✅
- [x] SMS kanalı kaldırıldı; kanallar *Uygulama içi* ve *E-posta*. E-posta hesabın kayıtlı adresine gider; Aboneliklerim sayfası bu adresi gösterir
- [x] E-posta bildirimi: Brevo REST API (`BrevoEpostaGonderici`, `"eposta"` HttpClient'ı, 10 sn zaman aşımı); anahtar yoksa deneme modu (`.eml` dosyası)
- [x] Toplu bildirim kuyruğu (`Bildirimler` tablosu, uyarıyla aynı transaction), 3 deneme + artan bekleme, 250 ms hız sınırı, 401'de durdurma
- [x] Yönetici: `/Uyari/Bildirimler/{id}` gönderim kayıtları ve "Başarısızları yeniden dene"
- [x] Harita API: toplanma alanı formunda "Adresten konum bul" (OpenStreetMap Nominatim, `"harita"` HttpClient'ı, User-Agent, saniyede 1 istek); listede "Haritada aç"
- [x] Anahtarlar ortam değişkeninde: `AFET_EPOSTA_API_ANAHTARI`, `AFET_EPOSTA_GONDEREN` (isteğe bağlı `AFET_HARITA_URL`)
- [x] HTTP üzerinden 16 senaryo testi geçti (SMS reddi, yayın → e-posta özeti, kayıt sayfası, yeniden deneme, adresten konum, bulunamayan adres)

## Hafta 10 · Yapay Zekâ Bileşeni: Duyuru Özetleme ve Sade Uyarı ✅
- [x] `/Uyari` formunda "Yapay zekâ ile sade uyarı taslağı": kurum duyurusu + seçili bölgeler → başlık, mesaj, seviye, afet türü alanlarına yazılan taslak (yayınlanmaz)
- [x] Claude (`claude-opus-5-5`, Anthropic C# SDK 13.8.0), JSON şemalı yapılandırılmış çıktı, sunucu tarafı yedek model; anahtar `ANTHROPIC_API_KEY`
- [x] `CiktiDogrulayici` (uydurma sayı / adres, bölge adı, uzunluk, seviye, tür; uzun cümle uyarısı) ve kural tabanlı yedek özetleyici
- [x] AI günlüğü: `/Uyari/YapayZekaGunlugu` (istem, ham çıktı, doğrulama notları)
- [x] Hata düzeltmesi: "Taslak üret" düğmesi yayın onay penceresini açıyordu; onay artık yalnızca "Uyarıyı Yayınla" için soruluyor
- [x] HTTP testleri: anahtarsız 12/12, geçersiz anahtarla (gerçek Claude çağrısı → 401 → kural tabanlı yedek) 12/12

## Hafta 11 · İleri Modül: Toplanma Alanı Haritası ✅
- [x] `/ToplanmaAlani/Harita`: Leaflet 1.9.4 (SRI bütünlük kontrolüyle) + OpenStreetMap haritası, en yakın 3 alan listesi (mesafe, yürüme süresi)
- [x] Konum: bölge merkezi, adres (Nominatim), "Konumumu kullan" (tarayıcı konumu; https veya localhost), haritaya tıklama
- [x] `KonumHesap` (Haversine) ve `ToplanmaAlaniManager.EnYakinlar`; koordinat zorunlu ve bölgeye en fazla 50 km kuralı, 10 km uyarısı
- [x] Harita verisi sayfaya JSON olarak gömülür; açılır pencere metinleri `textContent` ile eklenir (XSS'e karşı)
- [x] HTTP testleri 14/14 + tarayıcıda harita ve tıklama

### Vize öncesi tarayıcıda denenen senaryolar (hepsi beklendiği gibi)
| Senaryo | Sonuç |
|---|---|
| Vatandaş girişi → panelde abone olunan bölgedeki uyarı | ✅ görünür |
| Vatandaş `/Bolge`, `/Uyari`, `/Rehber/Duzenle` adreslerine gider | ✅ "Erişim engellendi" |
| Vatandaş doğrudan `POST /Uyari/Yayinla` gönderir | ✅ reddedilir |
| Başka kullanıcının aboneliğini iptal etme / değiştirme | ✅ reddedilir (sahiplik kontrolü) |
| Aynı bölgeye ikinci abonelik | ✅ "Bu bölgeye zaten abonesiniz." |
| CSRF anahtarı olmayan POST isteği | ✅ 400 Bad Request |
| Abonelik ekle → min. seviye tercihine uyan uyarı panelde belirir; aboneliği pasif yapınca kaybolur | ✅ |
| Yönetici: bölgesiz uyarı | ✅ "Uyarı en az bir bölgeye hedeflenmelidir." |
| Yönetici: acil uyarı yayınla → vatandaş panelinde en üstte, kırmızı | ✅ |
| Yönetici: bağlı kaydı olan bölgeyi silme | ✅ reddedilir |
| Koordinat girişi "38,5" ve "40.25" | ✅ ikisi de doğru kaydedilir; 95 enlemi reddedilir |
| Oturum kapalıyken `/Panel` | ✅ giriş sayfasına yönlendirir |
| Olmayan sayfa | ✅ Türkçe 404 sayfası |
