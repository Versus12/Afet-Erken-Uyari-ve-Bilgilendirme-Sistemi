/* =====================================================================
   03_TestSorgulari.sql  -  Şema ve örnek veriyle çalışan temel sorgular
   (Hafta 4 "Gösterilecek İlerleme" ve vize demosu için)
   ===================================================================== */
USE AfetBilgilendirme;
GO

-- 1) Bölgeler ve risk seviyeleri
SELECT BolgeId, BolgeAdi, RiskSeviyesi FROM dbo.Bolgeler ORDER BY RiskSeviyesi DESC;

-- 2) N-N ilişki: hangi kullanıcı hangi bölgelere abone?
SELECT k.KullaniciAdi, b.BolgeAdi, a.BildirimKanali, a.MinSeviye, a.AktifMi
FROM dbo.Abonelikler a
JOIN dbo.Kullanicilar k ON k.Id = a.KullaniciId
JOIN dbo.Bolgeler b     ON b.BolgeId = a.BolgeId
ORDER BY k.KullaniciAdi, b.BolgeAdi;

-- 3) N-N ilişki: hangi uyarı hangi bölgelere hedeflendi?
SELECT u.UyariId, u.Baslik, u.Seviye, b.BolgeAdi
FROM dbo.Uyarilar u
JOIN dbo.UyariBolgeleri ub ON ub.UyariId = u.UyariId
JOIN dbo.Bolgeler b        ON b.BolgeId = ub.BolgeId
ORDER BY u.UyariId;

-- 4) Bir kullanıcının görmesi gereken güncel uyarılar (uygulamadaki "Uyarılarım" sorgusu)
DECLARE @kid INT = (SELECT Id FROM dbo.Kullanicilar WHERE KullaniciAdi = N'vatandas1');
SELECT DISTINCT u.UyariId, u.Baslik, u.Seviye, u.YayinTarihi
FROM dbo.Uyarilar u
JOIN dbo.UyariBolgeleri ub ON ub.UyariId = u.UyariId
JOIN dbo.Abonelikler a     ON a.BolgeId = ub.BolgeId
WHERE a.KullaniciId = @kid AND a.AktifMi = 1 AND u.AktifMi = 1
  AND u.Seviye >= a.MinSeviye
  AND u.YayinTarihi <= GETDATE()
  AND (u.BitisTarihi IS NULL OR u.BitisTarihi > GETDATE());

-- 5) Bölge başına abone ve toplanma kapasitesi (toplulaştırma örneği)
SELECT b.BolgeAdi,
       (SELECT COUNT(*) FROM dbo.Abonelikler a WHERE a.BolgeId = b.BolgeId)        AS AboneSayisi,
       (SELECT ISNULL(SUM(Kapasite),0) FROM dbo.ToplanmaAlanlari t WHERE t.BolgeId = b.BolgeId) AS ToplamKapasite
FROM dbo.Bolgeler b
ORDER BY AboneSayisi DESC;

-- 6) Kısıt denemeleri (hata vermesi BEKLENİR - yorumu kaldırıp deneyin)
-- INSERT INTO dbo.Bolgeler (Il, Ilce, BolgeAdi, RiskSeviyesi) VALUES (N'Test', N'Test', N'Test', 9);   -- CK_Bolgeler_Risk
-- INSERT INTO dbo.Abonelikler (KullaniciId, BolgeId) VALUES (@kid, 999999);                          -- FK_Abonelikler_Bolgeler
-- INSERT INTO dbo.Kullanicilar (KullaniciAdi, Eposta, SifreHash, Rol) VALUES (N'x', N'x@example.com', N'x', N'Admin'); -- CK_Kullanicilar_Rol
