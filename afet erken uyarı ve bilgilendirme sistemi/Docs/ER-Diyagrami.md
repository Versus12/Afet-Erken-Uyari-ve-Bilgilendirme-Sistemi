
# Afet Erken Uyarý ve Bilgilendirme Sistemi
## ER Diyagramý

```mermaid
erDiagram

    BOLGE ||--o{ ABONELIK : "sahiptir"
    BOLGE ||--o{ TOPLANMAALANI : "icerir"
    BOLGE ||--o{ UYARI : "alir"

    BOLGE {
        int BolgeId PK
        string BolgeAdi
    }

    ABONELIK {
        int AbonelikId PK
        string AboneAdi
        string Telefon
        int BolgeId FK
    }

    TOPLANMAALANI {
        int ToplanmaAlaniId PK
        string AlanAdi
        string Adres
        int Kapasite
        int BolgeId FK
    }

    UYARI {
        int UyariId PK
        string UyariBasligi
        string UyariMesaji
        string UyariSeviyesi
        datetime UyariTarihi
        int BolgeId FK
    }
```
