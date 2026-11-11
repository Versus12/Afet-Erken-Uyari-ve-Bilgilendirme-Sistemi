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

---

## Sonraki aşama planı (Hafta 9+)
| Hafta | Plan |
|---|---|
| 9 | Harita (toplanma alanları) ve SMS/e-posta bildirim API'si; anahtar ortam değişkeninde |
| 10 | Yapay zekâ: gelen duyuruyu bölgeye özel sade uyarı metnine özetleme + çıktı doğrulama |
| 11 | En yakın toplanma alanı hesabı (Haversine) ve harita gösterimi |
| 12 | Yönetim panosu: abonelik/uyarı istatistikleri, grafik, CSV dışa aktarma |
| 13 | Birim + entegrasyon testleri (MSTest), hata listesi |
| 14–15 | Dokümantasyon, arayüz iyileştirme, final sürüm `v1.0.0` |
