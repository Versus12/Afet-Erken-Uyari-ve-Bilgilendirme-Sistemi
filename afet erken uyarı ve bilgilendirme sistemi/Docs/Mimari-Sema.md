# Afet Erken Uyarı ve Bilgilendirme Sistemi
## Katmanlı Mimari Şeması

```mermaid
flowchart TD
    subgraph P["SUNUM (Presentation) — Windows Forms"]
        G[FrmGiris / FrmKayit]
        A[FrmAnaMenu<br/>Uyarılarım + bildirim]
        M1[FrmAbonelik]
        M2[FrmUyariYayini]
        M3[FrmBolgeYonetimi]
        M4[FrmHazirlikRehberi]
        M5[FrmToplanmaAlanlari]
        Y[ArayuzYardimci<br/>hata yönetimi, ızgara]
    end
    subgraph B["İŞ (Business) — kurallar + yetki"]
        O[Oturum<br/>oturum & rol kontrolü]
        K[KimlikDogrulamaManager]
        BM[BolgeManager]
        AM[AbonelikManager]
        UM[UyariManager]
        RM[RehberManager]
        TM[ToplanmaAlaniManager]
    end
    subgraph D["VERİ (Data) — ADO.NET / SQL"]
        VB[VeritabaniBaglanti<br/>App.config: AfetDB]
        R[*Repository sınıfları]
    end
    E["ENTITIES<br/>Kullanici, Bolge, Abonelik, Uyari,<br/>HazirlikRehberi, ToplanmaAlani, Sabitler"]
    DB[("SQL Server<br/>AfetBilgilendirme")]

    P --> B --> D --> DB
    E -.-> P
    E -.-> B
    E -.-> D
```

## Katmanların sorumlulukları

| Katman | Klasör | Sorumluluk | Bilmediği şey |
|---|---|---|---|
| Sunum | proje kökü (`Frm*.cs`, `ArayuzYardimci.cs`) | Kullanıcıdan veri almak, sonucu göstermek, hataları mesaj kutusuna çevirmek | SQL, iş kuralı |
| İş | `Business/` | Doğrulama, iş kuralları, **yetki kontrolü** (`Oturum.YoneticiGerekli()`), sahiplik kontrolü | Form/ekran, SQL metni |
| Veri | `Data/` | Parametreli SQL ile CRUD, transaction | İş kuralı, kullanıcı rolü |
| Varlıklar | `Entities/` | Tablolara karşılık gelen sade sınıflar ve sabit listeler | — |

## Neden katmanlı mimari?
- **Tek sorumluluk:** Uyarı yayınlama kuralı değişirse yalnızca `UyariManager` değişir; form ve SQL etkilenmez.
- **Güvenlik:** Yetki kontrolü arayüzde *ve* iş katmanında yapılır. Buton gizlense bile başka bir yoldan çağrılan
  `UyariManager.Yayinla()` vatandaş oturumunda `YetkisizErisimHatasi` fırlatır.
- **Test edilebilirlik:** İş katmanı form olmadan çağrılabilir (vize öncesi uçtan uca testler bu şekilde yapıldı).
- **Değiştirilebilirlik:** Hafta 9'da harita/bildirim API'si eklendiğinde yalnızca yeni bir servis sınıfı eklenecek.

## Örnek akış: "Yönetici uyarı yayınlar → vatandaş görür"
1. `FrmUyariYayini` formu `Uyari` nesnesini doldurur, `UyariManager.Yayinla()` çağırır.
2. `UyariManager` oturum + yönetici rolünü kontrol eder, başlık/mesaj/seviye/bölge/tarih kurallarını doğrular.
3. `UyariRepository.Ekle()` uyarıyı ve hedef bölgeleri **tek transaction** içinde yazar.
4. `AbonelikRepository.UyariAliciSayisi()` kaç aboneye ulaşacağını hesaplar, yöneticiye gösterilir.
5. Vatandaşın `FrmAnaMenu` ekranındaki zamanlayıcı (60 sn) `UyariManager.Uyarilarim()` ile yeni uyarıyı çeker,
   sesli uyarı verir ve uyarıyı seviyesine göre renklendirir.
