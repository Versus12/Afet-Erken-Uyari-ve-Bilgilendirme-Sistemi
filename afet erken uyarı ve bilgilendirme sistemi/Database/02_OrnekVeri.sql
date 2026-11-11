/* =====================================================================
   02_OrnekVeri.sql  -  Sentetik örnek veri (Hafta 4)

   KVKK notu: Buradaki tüm kişi/e-posta bilgileri UYDURMADIR.
   Gerçek kişisel veri kullanılmamıştır. E-postalar "example.com"
   (RFC 2606 ile test için ayrılmış alan adı) altındadır.

   Test hesapları (yalnızca geliştirme ortamı içindir):
     yonetici  / Yonetici123!   (Rol: Yonetici)
     vatandas1 / Vatandas123!   (Rol: Vatandas)
     vatandas2 / Vatandas123!   (Rol: Vatandas)

   Script tekrar çalıştırılabilir; kayıt varsa yeniden eklemez.
   ===================================================================== */

USE AfetBilgilendirme;
GO

SET NOCOUNT ON;

/* ---------------- Kullanıcılar (parolalar BCrypt ile hash'li) -------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Kullanicilar WHERE KullaniciAdi = N'yonetici')
    INSERT INTO dbo.Kullanicilar (KullaniciAdi, Eposta, SifreHash, Rol)
    VALUES (N'yonetici', N'yonetici@example.com',
            N'$2a$11$NhOZmA.dYfQHy2XBHdn9Re2P7pslB4JpO15qKV0gUnmuXp9PZ4JwK', N'Yonetici');

IF NOT EXISTS (SELECT 1 FROM dbo.Kullanicilar WHERE KullaniciAdi = N'vatandas1')
    INSERT INTO dbo.Kullanicilar (KullaniciAdi, Eposta, SifreHash, Rol)
    VALUES (N'vatandas1', N'vatandas1@example.com',
            N'$2a$11$XToDZSbabMgjPM9DKRm1HOs/LDZ048uyg2ZOrwCiGdbbzWcqj16Ue', N'Vatandas');

IF NOT EXISTS (SELECT 1 FROM dbo.Kullanicilar WHERE KullaniciAdi = N'vatandas2')
    INSERT INTO dbo.Kullanicilar (KullaniciAdi, Eposta, SifreHash, Rol)
    VALUES (N'vatandas2', N'vatandas2@example.com',
            N'$2a$11$XToDZSbabMgjPM9DKRm1HOs/LDZ048uyg2ZOrwCiGdbbzWcqj16Ue', N'Vatandas');

/* ---------------- Bölgeler ------------------------------------------ */
DECLARE @Bolge TABLE (Il NVARCHAR(50), Ilce NVARCHAR(50), Risk TINYINT, Enlem DECIMAL(9,6), Boylam DECIMAL(9,6));
INSERT INTO @Bolge VALUES
 (N'Bingöl',   N'Merkez',   5, 38.884700, 40.498200),
 (N'Bingöl',   N'Solhan',   4, 38.968300, 41.054400),
 (N'Bingöl',   N'Genç',     4, 38.751600, 40.559500),
 (N'Elazığ',   N'Merkez',   4, 38.674300, 39.222500),
 (N'Erzurum',  N'Yakutiye', 3, 39.904300, 41.267900),
 (N'Rize',     N'Merkez',   3, 41.025500, 40.517700),
 (N'Antalya',  N'Manavgat', 2, 36.786600, 31.442500),
 (N'İstanbul', N'Kadıköy',  4, 40.990900, 29.029400);

INSERT INTO dbo.Bolgeler (Il, Ilce, BolgeAdi, RiskSeviyesi, Enlem, Boylam)
SELECT b.Il, b.Ilce, b.Il + N' / ' + b.Ilce, b.Risk, b.Enlem, b.Boylam
FROM @Bolge b
WHERE NOT EXISTS (SELECT 1 FROM dbo.Bolgeler x WHERE x.Il = b.Il AND x.Ilce = b.Ilce);

/* ---------------- Abonelikler --------------------------------------- */
DECLARE @v1 INT = (SELECT Id FROM dbo.Kullanicilar WHERE KullaniciAdi = N'vatandas1');
DECLARE @v2 INT = (SELECT Id FROM dbo.Kullanicilar WHERE KullaniciAdi = N'vatandas2');
DECLARE @yon INT = (SELECT Id FROM dbo.Kullanicilar WHERE KullaniciAdi = N'yonetici');

INSERT INTO dbo.Abonelikler (KullaniciId, BolgeId, BildirimKanali, MinSeviye)
SELECT a.KullaniciId, b.BolgeId, a.Kanal, a.MinSeviye
FROM (VALUES
        (@v1, N'Bingöl',  N'Merkez',  N'Uygulama', 1),
        (@v1, N'Bingöl',  N'Solhan',  N'SMS',      2),
        (@v2, N'Rize',    N'Merkez',  N'Eposta',   1),
        (@v2, N'Elazığ',  N'Merkez',  N'Uygulama', 3)
     ) a (KullaniciId, Il, Ilce, Kanal, MinSeviye)
JOIN dbo.Bolgeler b ON b.Il = a.Il AND b.Ilce = a.Ilce
WHERE NOT EXISTS (SELECT 1 FROM dbo.Abonelikler x
                  WHERE x.KullaniciId = a.KullaniciId AND x.BolgeId = b.BolgeId);

/* ---------------- Uyarılar + hedef bölgeler ------------------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Uyarilar)
BEGIN
    DECLARE @u INT;

    INSERT INTO dbo.Uyarilar (Baslik, Mesaj, AfetTuru, Seviye, YayinTarihi, BitisTarihi, YayinlayanId)
    VALUES (N'Artçı deprem riski',
            N'Bölgede artçı sarsıntı olasılığı yüksektir. Hasarlı binalara girmeyiniz, toplanma alanlarını öğreniniz.',
            N'Deprem', 3, DATEADD(HOUR, -3, GETDATE()), DATEADD(DAY, 3, GETDATE()), @yon);
    SET @u = SCOPE_IDENTITY();
    INSERT INTO dbo.UyariBolgeleri (UyariId, BolgeId)
    SELECT @u, BolgeId FROM dbo.Bolgeler WHERE Il = N'Bingöl';

    INSERT INTO dbo.Uyarilar (Baslik, Mesaj, AfetTuru, Seviye, YayinTarihi, BitisTarihi, YayinlayanId)
    VALUES (N'Kuvvetli yağış ve sel uyarısı',
            N'Önümüzdeki 24 saat içinde kuvvetli yağış bekleniyor. Dere yataklarından uzak durunuz.',
            N'Sel', 2, DATEADD(HOUR, -1, GETDATE()), DATEADD(DAY, 1, GETDATE()), @yon);
    SET @u = SCOPE_IDENTITY();
    INSERT INTO dbo.UyariBolgeleri (UyariId, BolgeId)
    SELECT @u, BolgeId FROM dbo.Bolgeler WHERE Il = N'Rize';

    INSERT INTO dbo.Uyarilar (Baslik, Mesaj, AfetTuru, Seviye, YayinTarihi, BitisTarihi, YayinlayanId)
    VALUES (N'Orman yangını riski',
            N'Yüksek sıcaklık ve rüzgâr nedeniyle orman yangını riski artmıştır. Açık alanda ateş yakmayınız.',
            N'Yangın', 2, DATEADD(DAY, -1, GETDATE()), DATEADD(DAY, 2, GETDATE()), @yon);
    SET @u = SCOPE_IDENTITY();
    INSERT INTO dbo.UyariBolgeleri (UyariId, BolgeId)
    SELECT @u, BolgeId FROM dbo.Bolgeler WHERE Il = N'Antalya';
END

/* ---------------- Hazırlık rehberi ---------------------------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.HazirlikRehberi)
BEGIN
    INSERT INTO dbo.HazirlikRehberi (AfetTuru, Asama, Baslik, Icerik, Sira) VALUES
    (N'Deprem', N'Önce',      N'Afet çantası hazırlayın',
        N'Su, uzun ömürlü gıda, ilk yardım malzemesi, el feneri, yedek pil, düdük, önemli belgelerin kopyaları ve ilaçlarınızı içeren bir çanta hazırlayın.', 1),
    (N'Deprem', N'Önce',      N'Eşyaları sabitleyin',
        N'Dolap, kitaplık ve ağır eşyaları duvara sabitleyin. Yatak başında devrilebilecek eşya bulundurmayın.', 2),
    (N'Deprem', N'Sırasında', N'Çök - Kapan - Tutun',
        N'Sağlam bir masanın yanına çökün, başınızı ve boynunuzu koruyun, sarsıntı bitene kadar tutunun. Merdiven ve asansör kullanmayın.', 1),
    (N'Deprem', N'Sonra',     N'Güvenli alana geçin',
        N'Gaz, elektrik ve suyu kapatın. Hasarlı binaya girmeyin, en yakın toplanma alanına gidin, resmi duyuruları takip edin.', 1),
    (N'Sel',    N'Önce',      N'Risk durumunu öğrenin',
        N'Evinizin dere yatağı veya taşkın alanında olup olmadığını öğrenin. Önemli eşyaları yüksek yerlere kaldırın.', 1),
    (N'Sel',    N'Sırasında', N'Yüksek yere çıkın',
        N'Akan suya girmeyin, araçla su basmış yoldan geçmeyin. Elektrik tesisatından uzak durun.', 1),
    (N'Sel',    N'Sonra',     N'Hijyene dikkat edin',
        N'Sel suyuyla temas eden gıdaları tüketmeyin, içme suyunun güvenli olduğundan emin olun.', 1),
    (N'Yangın', N'Önce',      N'Yangın söndürücü bulundurun',
        N'Evinizde yangın söndürücü ve duman dedektörü bulundurun, kullanımını öğrenin.', 1),
    (N'Yangın', N'Sırasında', N'Dumandan korunun',
        N'Eğilerek ve ağzınızı ıslak bezle kapatarak ilerleyin. 112''yi arayın.', 1),
    (N'Yangın', N'Sonra',     N'Yetkililerin iznini bekleyin',
        N'Yetkililer güvenli demeden yanmış alana geri dönmeyin.', 1);
END

/* ---------------- Toplanma alanları --------------------------------- */
INSERT INTO dbo.ToplanmaAlanlari (BolgeId, AlanAdi, Adres, Kapasite, Enlem, Boylam)
SELECT b.BolgeId, t.AlanAdi, t.Adres, t.Kapasite, t.Enlem, t.Boylam
FROM (VALUES
        (N'Bingöl',  N'Merkez', N'Şehir Stadyumu Toplanma Alanı', N'Merkez, Stadyum önü (örnek adres)', 5000, 38.886000, 40.495000),
        (N'Bingöl',  N'Merkez', N'Kampüs Toplanma Alanı',         N'Üniversite yerleşkesi (örnek adres)', 3000, 38.900000, 40.520000),
        (N'Bingöl',  N'Solhan', N'İlçe Parkı Toplanma Alanı',     N'Solhan merkez park (örnek adres)', 1200, 38.969000, 41.050000),
        (N'Rize',    N'Merkez', N'Sahil Parkı Toplanma Alanı',    N'Sahil yolu (örnek adres)', 2500, 41.030000, 40.520000),
        (N'Elazığ',  N'Merkez', N'Fuar Alanı',                    N'Fuar alanı (örnek adres)', 4000, 38.670000, 39.220000)
     ) t (Il, Ilce, AlanAdi, Adres, Kapasite, Enlem, Boylam)
JOIN dbo.Bolgeler b ON b.Il = t.Il AND b.Ilce = t.Ilce
WHERE NOT EXISTS (SELECT 1 FROM dbo.ToplanmaAlanlari x WHERE x.AlanAdi = t.AlanAdi AND x.BolgeId = b.BolgeId);

PRINT N'Örnek veri yüklendi.';
GO
