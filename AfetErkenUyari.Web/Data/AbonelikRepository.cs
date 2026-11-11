using AfetErkenUyari.Web.Entities;
using Microsoft.Data.SqlClient;

namespace AfetErkenUyari.Web.Data;

/// <summary>Abonelikler tablosu (Kullanıcı - Bölge N-N ara tablosu).</summary>
public class AbonelikRepository(VeritabaniBaglanti db)
{
    public List<Abonelik> KullaniciAbonelikleri(int kullaniciId)
    {
        var liste = new List<Abonelik>();
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(
            @"SELECT a.AbonelikId, a.KullaniciId, a.BolgeId, a.BildirimKanali, a.MinSeviye,
                     a.AboneTarihi, a.AktifMi, b.BolgeAdi, b.RiskSeviyesi
              FROM dbo.Abonelikler a
              JOIN dbo.Bolgeler b ON b.BolgeId = a.BolgeId
              WHERE a.KullaniciId = @kid
              ORDER BY b.BolgeAdi", con);
        cmd.Parameters.AddWithValue("@kid", kullaniciId);
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            liste.Add(new Abonelik
            {
                AbonelikId = (int)dr["AbonelikId"],
                KullaniciId = (int)dr["KullaniciId"],
                BolgeId = (int)dr["BolgeId"],
                BildirimKanali = (string)dr["BildirimKanali"],
                MinSeviye = dr.Tamsayi("MinSeviye"),
                AboneTarihi = (DateTime)dr["AboneTarihi"],
                AktifMi = (bool)dr["AktifMi"],
                BolgeAdi = (string)dr["BolgeAdi"],
                RiskSeviyesi = dr.Tamsayi("RiskSeviyesi")
            });
        }
        return liste;
    }

    /// <summary>Sahiplik kontrolü için aboneliğin kime ait olduğunu döndürür (yoksa null).</summary>
    public int? SahibiniGetir(int abonelikId)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand("SELECT KullaniciId FROM dbo.Abonelikler WHERE AbonelikId = @id", con);
        cmd.Parameters.AddWithValue("@id", abonelikId);
        object? sonuc = cmd.ExecuteScalar();
        return sonuc == null ? null : (int)sonuc;
    }

    public bool AbonelikVarMi(int kullaniciId, int bolgeId)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(
            "SELECT COUNT(*) FROM dbo.Abonelikler WHERE KullaniciId = @kid AND BolgeId = @bid", con);
        cmd.Parameters.AddWithValue("@kid", kullaniciId);
        cmd.Parameters.AddWithValue("@bid", bolgeId);
        return (int)cmd.ExecuteScalar()! > 0;
    }

    public int AbonelikSayisi(int kullaniciId)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Abonelikler WHERE KullaniciId = @kid", con);
        cmd.Parameters.AddWithValue("@kid", kullaniciId);
        return (int)cmd.ExecuteScalar()!;
    }

    public int Ekle(Abonelik a)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(
            @"INSERT INTO dbo.Abonelikler (KullaniciId, BolgeId, BildirimKanali, MinSeviye)
              VALUES (@kid, @bid, @kanal, @min);
              SELECT CAST(SCOPE_IDENTITY() AS INT);", con);
        cmd.Parameters.AddWithValue("@kid", a.KullaniciId);
        cmd.Parameters.AddWithValue("@bid", a.BolgeId);
        cmd.Parameters.AddWithValue("@kanal", a.BildirimKanali);
        cmd.Parameters.AddWithValue("@min", a.MinSeviye);
        return (int)cmd.ExecuteScalar()!;
    }

    public void TercihGuncelle(int abonelikId, string kanal, int minSeviye, bool aktifMi)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(
            @"UPDATE dbo.Abonelikler SET BildirimKanali = @kanal, MinSeviye = @min, AktifMi = @aktif
              WHERE AbonelikId = @id", con);
        cmd.Parameters.AddWithValue("@kanal", kanal);
        cmd.Parameters.AddWithValue("@min", minSeviye);
        cmd.Parameters.AddWithValue("@aktif", aktifMi);
        cmd.Parameters.AddWithValue("@id", abonelikId);
        cmd.ExecuteNonQuery();
    }

    public void Sil(int abonelikId)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand("DELETE FROM dbo.Abonelikler WHERE AbonelikId = @id", con);
        cmd.Parameters.AddWithValue("@id", abonelikId);
        cmd.ExecuteNonQuery();
    }

    /// <summary>Uyarının ulaşacağı aktif abone sayısı (yayın sonrası bilgi).</summary>
    public int UyariAliciSayisi(int uyariId)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(
            @"SELECT COUNT(DISTINCT a.KullaniciId)
              FROM dbo.Abonelikler a
              JOIN dbo.UyariBolgeleri ub ON ub.BolgeId = a.BolgeId
              JOIN dbo.Uyarilar u ON u.UyariId = ub.UyariId
              WHERE ub.UyariId = @uid AND a.AktifMi = 1 AND u.Seviye >= a.MinSeviye", con);
        cmd.Parameters.AddWithValue("@uid", uyariId);
        return (int)cmd.ExecuteScalar()!;
    }
}
