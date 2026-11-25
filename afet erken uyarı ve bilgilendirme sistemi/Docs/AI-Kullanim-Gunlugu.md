# Yapay Zekâ Kullanım Günlüğü

> Ders kuralı: AI'dan alınan her kod parçası öğrenci tarafından okunur, test edilir ve sözlüde **satır satır açıklanabilir** olmalıdır.
> Aşağıdaki "Benim doğrulamam / değişikliğim" sütununu kendi cümlelerinizle doldurun.

| Tarih | Hafta | Araç | Amaç / İstem (özet) | AI çıktısı (özet) | Benim doğrulamam / değişikliğim |
|---|---|---|---|---|---|
| | 3 | | Katmanlı mimari ve klasör yapısı alternatifleri | Sunum/İş/Veri/Entities önerisi | |
| | 3 | | ER diyagramı için varlık/ilişki önerisi | Bölge, Abonelik, Uyarı, Toplanma alanı | Kullanıcı–Bölge ve Uyarı–Bölge ilişkilerinin N-N olduğunu ben belirledim |
| | 4 | | SQL şeması, CHECK/UNIQUE kısıtları, indeks önerileri | `01_Sema.sql` taslağı | |
| | 5 | | Güvenli parola saklama (BCrypt), kaba kuvvet koruması | `KimlikDogrulamaManager` | |
| | 6 | | Abonelik iş kuralları ve sahiplik kontrolü | `AbonelikManager` | |
| | 7 | | Uyarı + hedef bölgeleri transaction ile ekleme | `UyariRepository.Ekle` | |
| | 8 | | Entegrasyon hatalarının ayıklanması (ad alanı, sütun adı, bağlantı cümlesi) | Hata listesi + düzeltmeler | |
| | 8 | | Uçtan uca test senaryoları | 21 senaryo | |
| | 9 | | SMS kanalını kaldırma, e-postayı kayıtlı adrese bağlama | Kanal sabitleri, `CK_Abonelikler_Kanal` güncellemesi | |
| | 9 | | Brevo e-posta API dokümantasyonunu çözümleme (istek/yanıt, hata kodları) | `BrevoEpostaGonderici`, `BildirimGonderimHatasi` | |
| | 9 | | Toplu bildirim, yeniden deneme ve hız sınırı tasarımı | `BildirimManager`, `Bildirimler` tablosu, `HizSinirlayici` | |
| | 9 | | Nominatim (OpenStreetMap) ile adresten konum bulma, JSON ayrıştırma | `HaritaServisi.AdresBul` / `Ayristir` | |
| | 10 | | Sade uyarı için sistem istemi (prompt) ve JSON şeması tasarımı | `SadeUyariIstemi` | |
| | 10 | | Anthropic C# SDK ile Claude çağrısı, hata türleri, yedek model | `ClaudeIstemcisi` | |
| | 10 | | AI çıktısını doğrulama kuralları (uydurma sayı, bölge adı, cümle uzunluğu) | `CiktiDogrulayici` | |
| | 11 | | Haversine formülü ve en yakın alan sıralaması | `KonumHesap`, `ToplanmaAlaniManager.EnYakinlar` | |
| | 11 | | Leaflet + OpenStreetMap harita sayfası, WebView2 ile masaüstüne gömme | `HaritaSayfasi`, `FrmToplanmaHaritasi`, `Harita.cshtml` | |
| | 10 | | AI yokken çalışan kural tabanlı özetleyici | `KuralTabanliOzetleyici` (kurum adındaki "Acil" kelimesi hatası testte bulunup düzeltildi) | |

## Sözlüde açıklamaya hazırlanılacak AI kaynaklı parçalar
1. `UyariRepository.Ekle` — `SqlTransaction`, `Commit`/`Rollback`, `SCOPE_IDENTITY()`.
2. `UyariRepository.KullaniciIcinGecerliUyarilar` — `EXISTS` alt sorgusu, seviye ve tarih filtreleri.
3. `KimlikDogrulamaManager.GirisYap` — `BCrypt.Verify`, deneme sayacı, kilit süresi.
4. `Oturum` — statik sınıf, hareketsizlik süresi, `YoneticiGerekli()`.
5. `ArayuzYardimci.Calistir` — hata türüne göre farklı mesaj (`IsKuraliHatasi` / `YetkisizErisimHatasi` / `SqlException`).
6. `BildirimManager.BekleyenleriGonder` — deneme döngüsü, artan bekleme, kalıcı/geçici hata ayrımı (Hafta 9).
7. `BrevoEpostaGonderici.Gonder` — HTTP isteği, `api-key` başlığı, durum koduna göre hata türü, `messageId` ayrıştırma (Hafta 9).
8. `HaritaServisi.AdresBul` — URL kodlama, User-Agent, zaman aşımı, 429 yönetimi, JSON yanıtını okuma (Hafta 9).
9. `SadeUyariIstemi.Sistem` — her kuralın neden konduğu; kaynağın neden etiket içinde verildiği (Hafta 10).
10. `CiktiDogrulayici.IcerigiDogrula` — uydurma sayı kontrolü, bölge adı kontrolü, uyarı/hata ayrımı (Hafta 10).
11. `SadeUyariManager.TaslakUret` — AI → doğrulama → kural tabanlı yedek → günlük akışı (Hafta 10).
12. `KonumHesap.MesafeKm` — Haversine formülünün her satırı (radyan dönüşümü, a, atan2) (Hafta 11).
13. `ToplanmaAlaniManager.Kaydet` — koordinat zorunluluğu ve 50 km kuralı (Hafta 11).

> Hafta 10'un uygulama içindeki AI günlüğü (istem + ham çıktı + doğrulama) `YapayZekaKayitlari` tablosunda tutulur ve
> masaüstünde *Uyarı Yayını → Yapay Zekâ Günlüğü*, web'de `/Uyari/YapayZekaGunlugu` sayfasından okunur.
