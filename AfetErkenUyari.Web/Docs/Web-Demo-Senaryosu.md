# Vize Demo Senaryosu — Web Sitesi (≈ 5 dakika)

Hazırlık: veritabanı scriptleri çalıştırılmış, site açık (http://localhost:5283).

| # | Adım | Gösterilen |
|---|---|---|
| 1 | Ana sayfa: proje amacı ve modüller. | Tanıtım |
| 2 | **Kayıt Ol** → zayıf şifre (`abc`) → hata; sonra `demo_web` / `Demo1234` ile kayıt. | Doğrulama, BCrypt |
| 3 | Yanlış şifreyle 5 kez giriş → kilit mesajı; `vatandas1` / `Vatandas123!` ile giriş. | Kaba kuvvet koruması |
| 4 | Menüde **Yönetim Paneli** yok. Adres çubuğuna `/Uyari` yaz → "Erişim engellendi". | Rol tabanlı erişim (sunucu tarafında) |
| 5 | **Uyarılarım**: Bingöl'deki "Artçı deprem riski" (turuncu, TEHLİKE). | Bölge bazlı filtreleme |
| 6 | **Aboneliklerim**: Rize / Merkez'e "Dikkat" seviyesiyle abone ol → panelde sel uyarısı belirir. "Aktif" işaretini kaldırıp kaydet → kaybolur. | N-N ilişki, tercih yönetimi |
| 7 | **Hazırlık Rehberi** → Deprem: önce / sırasında / sonra. | Rehber içeriği |
| 8 | **Toplanma Alanları** → "Yalnızca abone olduğum bölgeler". | Modül entegrasyonu |
| 9 | Çıkış → `yonetici` / `Yonetici123!`. Menüde **Yönetim Paneli** görünür. | Rol |
| 10 | **Bölge Yönetimi** → Bingöl / Merkez'i sil → "bağlı kayıt var". Yeni bölge ekle (koordinatı "38,5" yaz). | İş kuralı, CRUD |
| 11 | **Uyarı Yayını** → bölge seçmeden yayınla → hata. Sonra Bingöl / Merkez + Solhan, **Acil** → onay → "X aboneye ulaşacak". | Hedefleme, transaction |
| 12 | Başka bir tarayıcı sekmesinde vatandaş paneli açıkken en geç 60 sn içinde "Yeni uyarı!" bildirimi. | Uygulama içi bildirim |
| 13 | Aynı uyarıyı Windows Forms uygulamasında vatandaş olarak gör. | Ortak veritabanı, entegrasyon |
| 14 | Yönetici "Kaldır" → uyarı vatandaş panelinden kaybolur. | Uyarı yaşam döngüsü |
