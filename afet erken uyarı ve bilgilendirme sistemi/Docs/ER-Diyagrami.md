# Afet Erken Uyarı ve Bilgilendirme Sistemi
## ER Diyagramı (Hafta 3 – güncellendi Hafta 8)

```mermaid
erDiagram
    KULLANICILAR ||--o{ ABONELIKLER : "abone olur"
    BOLGELER     ||--o{ ABONELIKLER : "abonesi vardır"
    KULLANICILAR ||--o{ UYARILAR : "yayınlar (yönetici)"
    UYARILAR     ||--|{ UYARI_BOLGELERI : "hedeflenir"
    BOLGELER     ||--o{ UYARI_BOLGELERI : "uyarı alır"
    BOLGELER     ||--o{ TOPLANMA_ALANLARI : "içerir"

    KULLANICILAR {
        int Id PK
        nvarchar KullaniciAdi UK
        nvarchar Eposta UK
        nvarchar SifreHash "BCrypt"
        nvarchar Rol "Vatandas | Yonetici"
        datetime OlusturmaTarihi
        bit AktifMi
    }
    BOLGELER {
        int BolgeId PK
        nvarchar Il
        nvarchar Ilce
        nvarchar BolgeAdi
        tinyint RiskSeviyesi "1-5"
        decimal Enlem
        decimal Boylam
    }
    ABONELIKLER {
        int AbonelikId PK
        int KullaniciId FK
        int BolgeId FK
        nvarchar BildirimKanali "Uygulama | SMS | Eposta"
        tinyint MinSeviye "1-4"
        datetime AboneTarihi
        bit AktifMi
    }
    UYARILAR {
        int UyariId PK
        nvarchar Baslik
        nvarchar Mesaj
        nvarchar AfetTuru
        tinyint Seviye "1 Bilgi - 4 Acil"
        datetime YayinTarihi
        datetime BitisTarihi
        int YayinlayanId FK
        bit AktifMi
    }
    UYARI_BOLGELERI {
        int UyariId PK, FK
        int BolgeId PK, FK
    }
    HAZIRLIK_REHBERI {
        int RehberId PK
        nvarchar AfetTuru
        nvarchar Asama "Önce | Sırasında | Sonra"
        nvarchar Baslik
        nvarchar Icerik
        int Sira
    }
    TOPLANMA_ALANLARI {
        int AlanId PK
        int BolgeId FK
        nvarchar AlanAdi
        nvarchar Adres
        int Kapasite
        decimal Enlem
        decimal Boylam
    }
```

## İlişkiler

| İlişki | Tür | Nasıl modellendi |
|---|---|---|
| Kullanıcı – Bölge (abonelik) | **N-N** | `Abonelikler` ara tablosu; `UNIQUE (KullaniciId, BolgeId)` aynı bölgeye iki kez abone olmayı engeller. Tercihler (kanal, min. seviye) ara tabloda tutulur. |
| Uyarı – Bölge (hedefleme) | **N-N** | `UyariBolgeleri` ara tablosu; birleşik PK `(UyariId, BolgeId)`. Bir uyarı birden çok bölgeye gidebilir. |
| Kullanıcı (yönetici) – Uyarı | 1-N | `Uyarilar.YayinlayanId` → `Kullanicilar.Id` |
| Bölge – Toplanma alanı | 1-N | `ToplanmaAlanlari.BolgeId` → `Bolgeler.BolgeId` |
| Hazırlık rehberi | bağımsız | Afet türüne göre gruplanır (bölgeden bağımsız içerik). |

## Normalizasyon notları
- **1NF:** Her sütun tek değer tutar; uyarının hedef bölgeleri virgüllü metin olarak değil, ayrı tabloda (`UyariBolgeleri`) tutulur.
- **2NF:** Ara tablolardaki tercih alanları (kanal, min. seviye) birleşik anahtarın tamamına bağlıdır.
- **3NF:** Bölge adı abonelik/uyarı tablolarında tekrar edilmez; yalnızca `BolgeId` tutulur.
  (`Bolgeler.BolgeAdi` = "İl / İlçe" görüntüleme kolaylığı için bilinçli olarak türetilmiş bir alandır; iş katmanı her kayıtta yeniden üretir.)

## Kısıtlar (özet)
- CHECK: `Rol`, `RiskSeviyesi (1-5)`, `Seviye (1-4)`, `MinSeviye (1-4)`, `AfetTuru`, `BildirimKanali`, `Kapasite > 0`, `BitisTarihi > YayinTarihi`, enlem/boylam aralıkları.
- UNIQUE: `KullaniciAdi`, `Eposta`, `(Il, Ilce)`, `(KullaniciId, BolgeId)`.
- FK + `ON DELETE CASCADE`: kullanıcı silinirse abonelikleri; uyarı silinirse hedef bölge kayıtları silinir.
- İndeksler: `Abonelikler.BolgeId`, `UyariBolgeleri.BolgeId`, `Uyarilar (AktifMi, YayinTarihi)`, `ToplanmaAlanlari.BolgeId`, `HazirlikRehberi (AfetTuru, Sira)` — "bölgedeki aboneler / kullanıcının uyarıları" sorgularını hızlandırmak için.
