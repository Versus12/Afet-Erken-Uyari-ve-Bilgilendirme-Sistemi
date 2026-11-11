# Vize Demo Senaryosu (≈ 5 dakika)

Hazırlık: `01_Sema.sql` ve `02_OrnekVeri.sql` çalıştırılmış, uygulama `F5` ile açılmış olmalı.

| # | Adım | Ne gösterilir? | İlgili hafta |
|---|---|---|---|
| 1 | Giriş ekranında "Veritabanı bağlantısı hazır" yazısını göster. | App.config'ten bağlantı | 4 |
| 2 | **Kayıt ol** → zayıf şifre dene (`abc`) → hata. Sonra `demo_vatandas` / `Demo1234` ile kayıt ol. | Doğrulama, BCrypt | 5 |
| 3 | SSMS'te `SELECT KullaniciAdi, SifreHash FROM Kullanicilar` → şifrenin hash'li olduğunu göster. | Düz metin yok | 5 |
| 4 | Yanlış şifreyle 5 kez dene → kilit mesajı. Doğru şifreyle 2 dk sonra (veya `vatandas1`) giriş. | Kaba kuvvet koruması | 5 |
| 5 | Vatandaş ana menüsü: yönetim menüsünün **görünmediğini** göster. | Rol tabanlı arayüz | 5 |
| 6 | **Bölge Aboneliklerim** → *Bingöl / Merkez*'e "Uygulama", min. seviye "Bilgi" ile abone ol. Aynı bölgeye tekrar abone olmayı dene → hata. | N-N ilişki, iş kuralı | 6 |
| 7 | Ana menüde Bingöl'deki "Artçı deprem riski" uyarısının (turuncu satır) göründüğünü göster. | Bölge bazlı filtreleme | 6–7 |
| 8 | **Hazırlık Rehberi** → "Deprem" filtrele; Önce/Sırasında/Sonra adımları. | Rehber içeriği | 7 |
| 9 | **Toplanma Alanları** → "Yalnızca abone olduğum bölgeler" işaretle. | Modül entegrasyonu | 8 |
| 10 | Çıkış yap → `yonetici` / `Yonetici123!` ile giriş. Yönetim paneli görünür. | Rol | 5 |
| 11 | **Bölge Yönetimi** → *Bingöl / Merkez*'i silmeyi dene → "bağlı kayıt var" hatası. Yeni bölge ekle (örn. *Muş / Merkez*). | İş kuralı, CRUD | 6 |
| 12 | **Uyarı Yayını** → Başlık "Deprem tatbikatı", seviye *Acil*, hedef: Bingöl / Merkez + Solhan → Yayınla. "X aboneye ulaşacak" mesajı. | Hedefleme, transaction | 7 |
| 13 | Bölgesiz yayınlamayı dene → hata. | Doğrulama | 7 |
| 14 | Çıkış → vatandaş olarak tekrar giriş: yeni **Acil** (kırmızı) uyarı en üstte. | Uçtan uca akış | 8 |
| 15 | Yönetici olarak uyarıyı "Yayından kaldır" → vatandaş listesinden kaybolur. | Uyarı yaşam döngüsü | 7–8 |

**Kapanış cümlesi:** "Kullanıcı + veritabanı + bölge/abonelik + uyarı/rehber modülleri uçtan uca çalışıyor;
sürüm `v0.8.0` olarak etiketlendi. Sonraki adım harita/bildirim API'si ve yapay zekâ ile sade uyarı üretimi."
