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

## Sözlüde açıklamaya hazırlanılacak AI kaynaklı parçalar
1. `UyariRepository.Ekle` — `SqlTransaction`, `Commit`/`Rollback`, `SCOPE_IDENTITY()`.
2. `UyariRepository.KullaniciIcinGecerliUyarilar` — `EXISTS` alt sorgusu, seviye ve tarih filtreleri.
3. `KimlikDogrulamaManager.GirisYap` — `BCrypt.Verify`, deneme sayacı, kilit süresi.
4. `Oturum` — statik sınıf, hareketsizlik süresi, `YoneticiGerekli()`.
5. `ArayuzYardimci.Calistir` — hata türüne göre farklı mesaj (`IsKuraliHatasi` / `YetkisizErisimHatasi` / `SqlException`).
