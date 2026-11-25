# Haftalık İlerleme (Hafta 3 – 8)

Kaynak: *P14 — Afet Erken Uyarı ve Bilgilendirme Sistemi* ders paketi.
Teknoloji: C#, .NET Framework 4.7.2, Windows Forms, Microsoft SQL Server, ADO.NET, BCrypt.Net-Next.

---

## Hafta 3 · Kurulum, Mimari ve Veri Modeli ✅
**Teslim:** GitHub deposu (README + .gitignore) · ER diyagramı · Mimari şema

- [x] Visual Studio 2022 + .NET Framework 4.7.2 Windows Forms projesi
- [x] Katmanlı klasör yapısı: `Entities/`, `Data/`, `Business/`, sunum (`Frm*.cs`)
- [x] `.gitignore` (bin/obj/.vs/packages hariç tutulur), `README.md`
- [x] ER diyagramı → [ER-Diyagrami.md](ER-Diyagrami.md)
- [x] Mimari şema → [Mimari-Sema.md](Mimari-Sema.md)

## Hafta 4 · Veritabanı Tasarımı ve Veri Erişim Katmanı ✅
**Teslim:** Şema scriptleri · Örnek veriyle dolu veritabanı · CRUD veri erişim kodu

- [x] `Database/01_Sema.sql` — 7 tablo, PK/FK, CHECK, UNIQUE, indeksler (tekrar çalıştırılabilir)
- [x] `Database/02_OrnekVeri.sql` — KVKK'ya uygun sentetik veri (8 bölge, 3 kullanıcı, 3 uyarı, 10 rehber, 5 toplanma alanı)
- [x] `Database/03_TestSorgulari.sql` — JOIN, N-N, toplulaştırma ve kısıt deneme sorguları
- [x] `Data/*Repository.cs` — parametreli SQL ile CRUD; uyarı ekleme **transaction** içinde

## Hafta 5 · Kullanıcı Yönetimi ve Kimlik Doğrulama ✅
**Teslim:** Çalışan kayıt/giriş · Rol tabanlı erişim · Kısa güvenlik notu

- [x] Kayıt (`FrmKayit`) ve giriş (`FrmGiris`) akışı
- [x] BCrypt parola hash + salt, parola kuralları
- [x] Oturum yönetimi (30 dk hareketsizlik), çıkış yap
- [x] Rol tabanlı erişim: vatandaş / yönetici (arayüz + iş katmanı)
- [x] 5 hatalı denemede geçici kilit
- [x] [Guvenlik-Notu.md](Guvenlik-Notu.md)

## Hafta 6 · Çekirdek Modül A: Bölge ve Abonelik ✅
**Teslim:** Çalışan Bölge ve Abonelik · kod ve kısa açıklama

- [x] Bölge modeli + yönetici CRUD ekranı (`FrmBolgeYonetimi`)
- [x] Abonelik ilişkisi (Kullanıcı–Bölge N-N), `FrmAbonelik`
- [x] Tercih yönetimi: bildirim kanalı, minimum uyarı seviyesi, aktif/durdur
- [x] İş kuralları: aynı bölgeye tekrar abonelik yok, en fazla 10 abonelik, bağlı kaydı olan bölge silinemez, sahiplik kontrolü

## Hafta 7 · Çekirdek Modül B: Uyarı Yayını ve Hazırlık Rehberi ✅
**Teslim:** Çalışan Uyarı Yayını ve Hazırlık Rehberi · kod ve kısa açıklama

- [x] Uyarı modeli (tür, seviye, yayın/bitiş tarihi, aktiflik)
- [x] Bölgeye hedefleme (birden çok bölge, `UyariBolgeleri`), "tümünü seç"
- [x] Yayın öncesi onay + "kaç aboneye ulaşacak" bilgisi, yayından kaldırma
- [x] Hazırlık rehberi: afet türü × aşama (Önce/Sırasında/Sonra); herkes okur, yönetici düzenler
- [x] İş kuralları: en az bir bölge, başlık/mesaj uzunluğu, bitiş > yayın, en fazla 30 gün geçerlilik

## Hafta 8 · VİZE: Ara Sürüm, Entegrasyon ve Sözlü Hazırlık ✅
**Teslim:** Çalışan ara sürüm (etiketli) · Güncel AI kullanım günlüğü · Kısa ilerleme raporu

- [x] Modüllerin uçtan uca entegrasyonu (`FrmAnaMenu`): giriş → abonelik → uyarı → bildirim
- [x] Uygulama içi bildirim: 60 sn'de bir yenileme, yeni uyarıda ses + durum çubuğu
- [x] Toplanma alanları listesi (bölge / "abone olduğum bölgeler" filtresi) — harita Hafta 11'de
- [x] Bilinen hataların giderilmesi (bkz. [Vize-Ilerleme-Raporu.md](Vize-Ilerleme-Raporu.md))
- [x] Uçtan uca entegrasyon testi (25 senaryo + 16 form testi, tamamı geçti)
- [x] Demo senaryosu → [Demo-Senaryosu.md](Demo-Senaryosu.md)
- [x] AI kullanım günlüğü → [AI-Kullanim-Gunlugu.md](AI-Kullanim-Gunlugu.md)
- [ ] **Git etiketi:** `git tag -a v0.8.0 -m "Vize ara sürüm"` ve `git push origin v0.8.0` (commit sonrası öğrenci tarafından)

## Hafta 9 · Harici API Entegrasyonu: Harita / Bildirim ✅
**Teslim:** Çalışan API entegrasyonu · Hata yönetimi

- [x] **Bildirim kanalı sadeleştirildi:** SMS kaldırıldı; kanallar yalnızca *Uygulama içi* ve *E-posta*. E-posta, kullanıcının kayıtlı adresine gider (ayrı bir "mail" adresi yok). Eski SMS abonelikleri `01_Sema.sql` ile E-posta'ya çevrildi; `CK_Abonelikler_Kanal` güncellendi.
- [x] **Bildirim API'si (e-posta):** Brevo REST API (`POST /v3/smtp/email`, `api-key` başlığı) → `BrevoEpostaGonderici`. Anahtar yoksa **deneme modu**: e-posta `.eml` dosyası olarak `%LOCALAPPDATA%\AfetErkenUyari\GidenEpostalar` klasörüne yazılır.
- [x] **Toplu bildirim:** Uyarı yayınlanınca e-posta kuyruğu (`Bildirimler` tablosu) uyarıyla **aynı transaction** içinde oluşturulur; her kişiye bir uyarı için tek e-posta (`UQ_Bildirimler`). `BildirimManager` kuyruğu sırayla gönderir.
- [x] **Hata yönetimi:** 10 sn zaman aşımı; geçici hatada (zaman aşımı, 429, 5xx) en fazla 3 deneme ve artan bekleme; kalıcı hatada (400) hemen "Başarısız"; yanlış anahtarda (401/403) kalan gönderimler durur. E-posta hatası uyarı yayınını geri almaz. Yönetici "E-posta Gönderim Kayıtları" ekranından başarısızları yeniden dener.
- [x] **Hız sınırı (rate limit):** e-postalar arasında en az 250 ms; harita servisine saniyede en fazla 1 istek; 429 yanıtındaki `Retry-After` dikkate alınır.
- [x] **Harita API'si:** OpenStreetMap Nominatim ile **adresten konum bulma** (toplanma alanı formunda "Adresten Konum Bul"); yanıt (JSON) ayrıştırılıp enlem/boylam alanlarına yazılır. "Haritada aç" seçili alanı OpenStreetMap'te gösterir.
- [x] **Anahtar ortam değişkeninde:** `AFET_EPOSTA_API_ANAHTARI`, `AFET_EPOSTA_GONDEREN` (isteğe bağlı `AFET_HARITA_URL`). Depoda anahtar yok.
- [x] Test: masaüstü iş katmanı 24/24, web 16/16 senaryo (gerçek Nominatim ve Brevo çağrıları dahil).

## Hafta 10 · Yapay Zekâ Bileşeni: Duyuru Özetleme ve Sade Uyarı ✅
**Teslim:** Çalışan yapay zekâ bileşeni · AI kullanım günlüğü

- [x] **Metin özetleme + sade dil:** Yönetici kurumun (AFAD, MGM, valilik) teknik duyurusunu yapıştırır; sistem seçili bölgeler için kısa, sade, bölgeye özel bir uyarı taslağı üretir (başlık, mesaj, önerilen seviye ve afet türü).
- [x] **Model/LLM API:** Anthropic'in resmî C# SDK'sı (`Anthropic` 13.8.0) ile Claude (`claude-opus-5-5`). Yapılandırılmış çıktı (JSON şeması) kullanılır; güvenlik reddinde sunucu tarafı yedek model (`fallbacks: default`) devreye girer. Anahtar ortam değişkeninde: `ANTHROPIC_API_KEY`.
- [x] **İstem tasarımı:** Sabit sistem istemi (9 kural: yalnızca kaynaktaki bilgi, sayı/adres uydurmama, bölge adını geçirme, en fazla 4 kısa cümle, son cümle eylem, seviye ölçütleri). Kaynak metin `<kaynak_duyuru>` etiketleri içinde **veri** olarak verilir (istem enjeksiyonuna karşı).
- [x] **Çıktı doğrulama (`CiktiDogrulayici`):** boş/bozuk JSON, `uygun=false`, uzunluklar, seviye 1-4, afet türü listede, **kaynakta olmayan sayı** (uydurma bilgi), kaynakta olmayan internet adresi, hedef bölgenin anılmaması → HATA; 25 kelimeyi aşan cümle, 4'ten fazla cümle → UYARI.
- [x] **Hatalı/boş çıktı yönetimi:** AI hata verirse, anahtar yoksa ya da çıktı doğrulanamazsa **kural tabanlı özetleyici** devreye girer (anahtar kelimeyle seviye/tür, kaynaktan kısa cümleler, sabit eylem cümlesi); neden kural tabanlı taslak gösterildiği ekranda yazar.
- [x] **İnsan onayı:** Taslak asla otomatik yayınlanmaz; geçerliyse yayın formuna aktarılır, yönetici okuyup düzeltip yayınlar.
- [x] **AI günlüğü:** Her denemenin istemi, ham çıktısı, doğrulama notları, süresi `YapayZekaKayitlari` tablosunda; "Yapay Zekâ Günlüğü" ekranından okunur.
- [x] Proje paket yapısı `packages.config` → `PackageReference`'a taşındı (SDK bağımlılıkları için); C# dil sürümü `latest`.
- [x] Test: masaüstü 23/23 + 9 form, web 12/12 (anahtarsız) ve 12/12 (geçersiz anahtarla gerçek Claude çağrısı → 401 → kural tabanlı yedek).

---

## Sonraki aşama planı (Hafta 11+)
| Hafta | Plan |
|---|---|
| 11 | En yakın toplanma alanı hesabı (Haversine) ve harita gösterimi |
| 12 | Yönetim panosu: abonelik/uyarı istatistikleri, grafik, CSV dışa aktarma |
| 13 | Birim + entegrasyon testleri (MSTest), hata listesi |
| 14–15 | Dokümantasyon, arayüz iyileştirme, final sürüm `v1.0.0` |
