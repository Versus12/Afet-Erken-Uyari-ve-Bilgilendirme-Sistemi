# Kısa Güvenlik Notu (Hafta 5)

| Konu | Uygulama | Kod |
|---|---|---|
| Parola saklama | Parolalar **BCrypt** (maliyet 11) ile hash'lenir. BCrypt her parola için rastgele bir **tuz (salt)** üretir ve hash'in içine gömer; aynı parolanın iki hash'i farklıdır. Düz metin parola hiçbir yerde saklanmaz. | `Business/KimlikDogrulamaManager.cs` → `KayitOl`, `GirisYap` |
| Parola kuralı | En az 8 karakter, büyük harf + küçük harf + rakam. | `SifreKurallariniDogrula` |
| Kaba kuvvet koruması | Aynı kullanıcı adıyla 5 hatalı denemeden sonra hesap 2 dakika kilitlenir. | `MaksimumHataliDeneme`, `KilitSuresi` |
| Kullanıcı adı sızdırmama | "Kullanıcı yok" ve "parola yanlış" için aynı mesaj verilir. | `GirisYap` |
| Oturum yönetimi | Masaüstü uygulama olduğu için web'deki session/JWT yerine bellek içi oturum kullanılır. 30 dk işlem yapılmazsa oturum düşer ve giriş ekranına dönülür. Arka plan yenilemesi süreyi uzatmaz. | `Business/Oturum.cs`, `FrmAnaMenu.tmrYenile_Tick` |
| Rol tabanlı erişim | Roller: `Vatandas`, `Yonetici`. Arayüzde yönetici menüsü gizlenir **ve** iş katmanında her korumalı metot `Oturum.YoneticiGerekli()` çağırır. | tüm `*Manager` sınıfları |
| Sahiplik kontrolü | Vatandaş yalnızca kendi aboneliğini değiştirebilir/silebilir. | `AbonelikManager.SahiplikKontrol` |
| Rol yükseltme | Kayıt ekranı her zaman `Vatandas` rolü atar; yönetici rolü yalnızca veritabanından verilir. DB'de `CK_Kullanicilar_Rol` kısıtı vardır. | `KayitOl`, `01_Sema.sql` |
| SQL Injection | Tüm sorgular **parametreli** (`@parametre`); kullanıcı girdisi SQL metnine eklenmez. | `Data/*Repository.cs` |
| Gizli bilgi | Bağlantı cümlesi koda gömülmez, `App.config` içindedir ve Windows kimlik doğrulaması kullanır (parola yok). | `Data/VeritabaniBaglanti.cs` |
| Hata mesajları | Kullanıcıya sade mesaj gösterilir; teknik ayrıntı `%LOCALAPPDATA%\AfetErkenUyari\hata.log` dosyasına yazılır. | `ArayuzYardimci.HataKaydet` |
| KVKK | Örnek verideki tüm kişiler uydurmadır, e-postalar `example.com` alanındadır. | `Database/02_OrnekVeri.sql` |

## Bilinen sınırlamalar (sonraki haftalar)
- Kilit sayacı bellekte tutulur; uygulama yeniden başlatılınca sıfırlanır (kalıcı kilit için tabloya taşınabilir).
- Parola sıfırlama / değiştirme ekranı yok.
- Hafta 9'da eklenecek API anahtarları ortam değişkeninde tutulacak, depoya eklenmeyecek.
