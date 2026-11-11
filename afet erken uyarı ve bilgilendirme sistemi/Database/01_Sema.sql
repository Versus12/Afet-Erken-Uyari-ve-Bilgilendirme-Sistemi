/* =====================================================================
   Afet Erken Uyarı ve Bilgilendirme Sistemi
   01_Sema.sql  -  Veritabanı şeması (Hafta 4)

   - Script tekrar tekrar çalıştırılabilir (idempotent). Var olan tabloları
     silmez; eksik tabloları/sütunları/kısıtları ekler.
   - Çalıştırma: SSMS'te açıp F5  veya
       sqlcmd -S "HELIN\SQLEXPRESS" -E -C -i 01_Sema.sql
   ===================================================================== */

IF DB_ID(N'AfetBilgilendirme') IS NULL
    CREATE DATABASE AfetBilgilendirme;
GO

USE AfetBilgilendirme;
GO

/* ---------------------------------------------------------------------
   1) Kullanicilar  (Hafta 5 - kimlik doğrulama / rol)
   --------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Kullanicilar', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Kullanicilar
    (
        Id              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Kullanicilar PRIMARY KEY,
        KullaniciAdi    NVARCHAR(100) NOT NULL,
        Eposta          NVARCHAR(150) NOT NULL,
        SifreHash       NVARCHAR(255) NOT NULL,
        Rol             NVARCHAR(50)  NOT NULL CONSTRAINT DF_Kullanicilar_Rol DEFAULT (N'Vatandas'),
        OlusturmaTarihi DATETIME      NOT NULL CONSTRAINT DF_Kullanicilar_Tarih DEFAULT (GETDATE()),
        AktifMi         BIT           NOT NULL CONSTRAINT DF_Kullanicilar_Aktif DEFAULT (1)
    );
END
GO

-- Önceki haftalarda oluşturulmuş tabloda eksik olabilecek sütunlar
IF COL_LENGTH(N'dbo.Kullanicilar', N'OlusturmaTarihi') IS NULL
    ALTER TABLE dbo.Kullanicilar ADD OlusturmaTarihi DATETIME NOT NULL
        CONSTRAINT DF_Kullanicilar_Tarih DEFAULT (GETDATE());
GO
IF COL_LENGTH(N'dbo.Kullanicilar', N'AktifMi') IS NULL
    ALTER TABLE dbo.Kullanicilar ADD AktifMi BIT NOT NULL
        CONSTRAINT DF_Kullanicilar_Aktif DEFAULT (1);
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Kullanicilar_Rol')
    ALTER TABLE dbo.Kullanicilar WITH NOCHECK
        ADD CONSTRAINT CK_Kullanicilar_Rol CHECK (Rol IN (N'Vatandas', N'Yonetici'));
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Kullanicilar_KullaniciAdi')
    CREATE UNIQUE INDEX UX_Kullanicilar_KullaniciAdi ON dbo.Kullanicilar (KullaniciAdi);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Kullanicilar_Eposta')
    CREATE UNIQUE INDEX UX_Kullanicilar_Eposta ON dbo.Kullanicilar (Eposta);
GO

/* ---------------------------------------------------------------------
   2) Bolgeler  (Hafta 6 - bölge modeli)
   --------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Bolgeler', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Bolgeler
    (
        BolgeId      INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Bolgeler PRIMARY KEY,
        Il           NVARCHAR(50)  NOT NULL,
        Ilce         NVARCHAR(50)  NOT NULL,
        BolgeAdi     NVARCHAR(100) NOT NULL,           -- Örn: "Bingöl / Merkez"
        RiskSeviyesi TINYINT       NOT NULL CONSTRAINT DF_Bolgeler_Risk DEFAULT (1),
        Enlem        DECIMAL(9,6)  NULL,
        Boylam       DECIMAL(9,6)  NULL,
        CONSTRAINT CK_Bolgeler_Risk   CHECK (RiskSeviyesi BETWEEN 1 AND 5),
        CONSTRAINT CK_Bolgeler_Enlem  CHECK (Enlem  IS NULL OR Enlem  BETWEEN -90  AND 90),
        CONSTRAINT CK_Bolgeler_Boylam CHECK (Boylam IS NULL OR Boylam BETWEEN -180 AND 180),
        CONSTRAINT UQ_Bolgeler_IlIlce UNIQUE (Il, Ilce)
    );
END
GO

/* ---------------------------------------------------------------------
   3) Abonelikler  (Hafta 6 - Kullanıcı <-> Bölge  N-N ilişkisi)
   Bir kullanıcı birden çok bölgeye abone olabilir; aynı bölgeye iki kez
   abone olamaz (UQ_Abonelikler_KullaniciBolge).
   --------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Abonelikler', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Abonelikler
    (
        AbonelikId     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Abonelikler PRIMARY KEY,
        KullaniciId    INT          NOT NULL,
        BolgeId        INT          NOT NULL,
        BildirimKanali NVARCHAR(20) NOT NULL CONSTRAINT DF_Abonelikler_Kanal DEFAULT (N'Uygulama'),
        MinSeviye      TINYINT      NOT NULL CONSTRAINT DF_Abonelikler_MinSeviye DEFAULT (1),
        AboneTarihi    DATETIME     NOT NULL CONSTRAINT DF_Abonelikler_Tarih DEFAULT (GETDATE()),
        AktifMi        BIT          NOT NULL CONSTRAINT DF_Abonelikler_Aktif DEFAULT (1),
        CONSTRAINT FK_Abonelikler_Kullanicilar FOREIGN KEY (KullaniciId)
            REFERENCES dbo.Kullanicilar (Id) ON DELETE CASCADE,
        CONSTRAINT FK_Abonelikler_Bolgeler FOREIGN KEY (BolgeId)
            REFERENCES dbo.Bolgeler (BolgeId),
        CONSTRAINT UQ_Abonelikler_KullaniciBolge UNIQUE (KullaniciId, BolgeId),
        CONSTRAINT CK_Abonelikler_Kanal CHECK (BildirimKanali IN (N'Uygulama', N'SMS', N'Eposta')),
        CONSTRAINT CK_Abonelikler_MinSeviye CHECK (MinSeviye BETWEEN 1 AND 4)
    );
    CREATE INDEX IX_Abonelikler_BolgeId ON dbo.Abonelikler (BolgeId);
END
GO

/* ---------------------------------------------------------------------
   4) Uyarilar  (Hafta 7 - uyarı yayını)
   Seviye: 1=Bilgi, 2=Dikkat, 3=Tehlike, 4=Acil
   --------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Uyarilar', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Uyarilar
    (
        UyariId          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Uyarilar PRIMARY KEY,
        Baslik           NVARCHAR(150)  NOT NULL,
        Mesaj            NVARCHAR(2000) NOT NULL,
        AfetTuru         NVARCHAR(30)   NOT NULL,
        Seviye           TINYINT        NOT NULL,
        YayinTarihi      DATETIME       NOT NULL CONSTRAINT DF_Uyarilar_Yayin DEFAULT (GETDATE()),
        BitisTarihi      DATETIME       NULL,
        YayinlayanId     INT            NOT NULL,
        AktifMi          BIT            NOT NULL CONSTRAINT DF_Uyarilar_Aktif DEFAULT (1),
        CONSTRAINT FK_Uyarilar_Kullanicilar FOREIGN KEY (YayinlayanId)
            REFERENCES dbo.Kullanicilar (Id),
        CONSTRAINT CK_Uyarilar_Seviye CHECK (Seviye BETWEEN 1 AND 4),
        CONSTRAINT CK_Uyarilar_AfetTuru CHECK (AfetTuru IN
            (N'Deprem', N'Sel', N'Yangın', N'Fırtına', N'Heyelan', N'Çığ', N'Kar', N'Diğer')),
        CONSTRAINT CK_Uyarilar_Tarih CHECK (BitisTarihi IS NULL OR BitisTarihi > YayinTarihi)
    );
    CREATE INDEX IX_Uyarilar_Aktif_Yayin ON dbo.Uyarilar (AktifMi, YayinTarihi DESC);
END
GO

/* ---------------------------------------------------------------------
   5) UyariBolgeleri  (Hafta 7 - bölgeye hedefleme, Uyarı <-> Bölge N-N)
   Bir uyarı birden çok bölgeye hedeflenebilir.
   --------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.UyariBolgeleri', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.UyariBolgeleri
    (
        UyariId INT NOT NULL,
        BolgeId INT NOT NULL,
        CONSTRAINT PK_UyariBolgeleri PRIMARY KEY (UyariId, BolgeId),
        CONSTRAINT FK_UyariBolgeleri_Uyarilar FOREIGN KEY (UyariId)
            REFERENCES dbo.Uyarilar (UyariId) ON DELETE CASCADE,
        CONSTRAINT FK_UyariBolgeleri_Bolgeler FOREIGN KEY (BolgeId)
            REFERENCES dbo.Bolgeler (BolgeId)
    );
    CREATE INDEX IX_UyariBolgeleri_BolgeId ON dbo.UyariBolgeleri (BolgeId);
END
GO

/* ---------------------------------------------------------------------
   6) HazirlikRehberi  (Hafta 7 - rehber içeriği, afet türüne göre)
   --------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.HazirlikRehberi', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.HazirlikRehberi
    (
        RehberId  INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_HazirlikRehberi PRIMARY KEY,
        AfetTuru  NVARCHAR(30)   NOT NULL,
        Asama     NVARCHAR(20)   NOT NULL,   -- Önce / Sırasında / Sonra
        Baslik    NVARCHAR(150)  NOT NULL,
        Icerik    NVARCHAR(MAX)  NOT NULL,
        Sira      INT            NOT NULL CONSTRAINT DF_HazirlikRehberi_Sira DEFAULT (1),
        GuncellemeTarihi DATETIME NOT NULL CONSTRAINT DF_HazirlikRehberi_Tarih DEFAULT (GETDATE()),
        CONSTRAINT CK_HazirlikRehberi_Asama CHECK (Asama IN (N'Önce', N'Sırasında', N'Sonra')),
        CONSTRAINT CK_HazirlikRehberi_AfetTuru CHECK (AfetTuru IN
            (N'Deprem', N'Sel', N'Yangın', N'Fırtına', N'Heyelan', N'Çığ', N'Kar', N'Diğer'))
    );
    CREATE INDEX IX_HazirlikRehberi_AfetTuru ON dbo.HazirlikRehberi (AfetTuru, Sira);
END
GO

/* ---------------------------------------------------------------------
   7) ToplanmaAlanlari  (temel model; harita/en yakın hesap Hafta 11'de)
   --------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.ToplanmaAlanlari', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ToplanmaAlanlari
    (
        AlanId   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ToplanmaAlanlari PRIMARY KEY,
        BolgeId  INT            NOT NULL,
        AlanAdi  NVARCHAR(150)  NOT NULL,
        Adres    NVARCHAR(300)  NOT NULL,
        Kapasite INT            NOT NULL,
        Enlem    DECIMAL(9,6)   NULL,
        Boylam   DECIMAL(9,6)   NULL,
        CONSTRAINT FK_ToplanmaAlanlari_Bolgeler FOREIGN KEY (BolgeId)
            REFERENCES dbo.Bolgeler (BolgeId),
        CONSTRAINT CK_ToplanmaAlanlari_Kapasite CHECK (Kapasite > 0)
    );
    CREATE INDEX IX_ToplanmaAlanlari_BolgeId ON dbo.ToplanmaAlanlari (BolgeId);
END
GO

PRINT N'Şema hazır.';
GO
