# Sözlü Sınav Hazırlık Notları (Hafta 3 – 8)

Bunlar **ezber cevap değil**, kendi kodunuza bakarak anlatabilmeniz için yol gösterici notlardır.

## Hafta 3
1. **Neden katmanlı mimari?** Sorumluluk ayrımı, yetkinin iş katmanında kontrolü, form olmadan test edilebilme → [Mimari-Sema.md](Mimari-Sema.md)
2. **1-1 / 1-N / N-N örnekleri:** Bölge–Toplanma alanı 1-N; Kullanıcı–Bölge (abonelik) N-N; Uyarı–Bölge N-N. (1-1 örneği: bir kullanıcının tek profil kaydı olsaydı.)
3. **PK taşıyan varlıklar:** Her tablo; ara tablo `UyariBolgeleri`'de birleşik PK `(UyariId, BolgeId)`.
4. **Klasör yapısı:** `Entities/` → `Data/` → `Business/` → `Frm*.cs`; her katman yalnızca altındakini çağırır.
5. **.gitignore:** `bin/`, `obj/`, `.vs/`, `packages/`, `*.user` — derleme çıktıları ve kişisel ayarlar; gizli anahtarlar asla eklenmez.

## Hafta 4
1. **İndeks:** `Abonelikler.BolgeId` — "bu bölgenin aboneleri" sorgusu sık; `Uyarilar(AktifMi, YayinTarihi)` — güncel uyarılar.
2. **FK ne işe yarar?** Var olmayan bölgeye abonelik eklenemez (referans bütünlüğü). `03_TestSorgulari.sql` 6. bölümde deneyin.
3. **N-N:** Ara tablo + iki FK + UNIQUE/PK.
4. **Migration:** Şemanın sürümlü, tekrar çalıştırılabilir scriptlerle değişmesi — `01_Sema.sql` `IF OBJECT_ID ... IS NULL` ile idempotent.
5. **Bir CRUD SQL'i:** `BolgeRepository.Guncelle` → `UPDATE ... WHERE BolgeId = @id` (parametreli).

## Hafta 5
1. **Hash + salt:** BCrypt her parolaya rastgele tuz ekler, hash'in içinde saklar; `Verify` aynı tuzu kullanarak karşılaştırır.
2. **Oturum vs token:** Web'de sunucu oturumu ya da JWT; bizde masaüstü olduğu için bellek içi `Oturum` sınıfı + 30 dk zaman aşımı.
3. **Rol tabanlı erişim:** Arayüzde buton gizleme + `Oturum.YoneticiGerekli()`.
4. **Başarısız giriş:** Genel mesaj + kalan hak; 5'te 2 dk kilit.
5. **Yetkisiz kullanıcı korumalı işleme giderse:** `YetkisizErisimHatasi` → "Bu işlem için yönetici yetkisi gereklidir."

## Hafta 6
1. **Abonelik–bölge ilişkisi:** `Abonelikler (KullaniciId, BolgeId)` + UNIQUE.
2. **Birden çok bölgeye abone olunabilir mi?** Evet, en fazla 10 (iş kuralı).
3. **İş kuralları:** tekrar abonelik yok; sahiplik kontrolü; bağlı kaydı olan bölge silinemez; il/ilçe tekil.
4. **Entegrasyon:** Abonelikler, uyarı filtrelemenin ve toplanma alanı filtresinin girdisidir.

## Hafta 7
1. **Doğru bölgeye hedefleme:** `UyariBolgeleri` ara tablosu; kullanıcı sorgusu abonelik ile JOIN yapar.
2. **Rehber yapısı:** Afet türü × aşama (Önce/Sırasında/Sonra) × sıra.
3. **İş kuralları:** en az bir bölge, bitiş > yayın, en fazla 30 gün, başlık/mesaj uzunluğu.

## Hafta 8
1. **Mimari özet:** 4 katman, 7 tablo, 8 form, 6 manager.
2. **Veri akışı:** → [Vize-Ilerleme-Raporu.md](Vize-Ilerleme-Raporu.md) bölüm 2.
3. **En zor entegrasyon:** İki farklı ad alanı + `Email`/`Eposta` sütun uyuşmazlığı → tek ad alanı ve tek repository.
4. **AI kodunu satır satır:** → [AI-Kullanim-Gunlugu.md](AI-Kullanim-Gunlugu.md) listesindeki 5 parça.
5. **Sonraki plan:** Harita/bildirim API → AI ile sade uyarı → en yakın toplanma alanı → istatistik panosu.
