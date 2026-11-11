using AfetErkenUyari.Web.Entities;
using Microsoft.Data.SqlClient;

namespace AfetErkenUyari.Web.Data;

/// <summary>HazirlikRehberi tablosu için CRUD.</summary>
public class RehberRepository(VeritabaniBaglanti db)
{
    private const string Sutunlar = "RehberId, AfetTuru, Asama, Baslik, Icerik, Sira, GuncellemeTarihi";

    /// <summary>afetTuru null ise tüm kayıtlar döner.</summary>
    public List<HazirlikRehberi> Listele(string? afetTuru)
    {
        var liste = new List<HazirlikRehberi>();
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(
            $@"SELECT {Sutunlar} FROM dbo.HazirlikRehberi
               WHERE (@tur IS NULL OR AfetTuru = @tur)
               ORDER BY AfetTuru,
                        CASE Asama WHEN N'Önce' THEN 1 WHEN N'Sırasında' THEN 2 ELSE 3 END, Sira", con);
        cmd.Parameters.Add("@tur", System.Data.SqlDbType.NVarChar, 30).Value = VeritabaniBaglanti.DbDegeri(afetTuru);
        using var dr = cmd.ExecuteReader();
        while (dr.Read()) liste.Add(Oku(dr));
        return liste;
    }

    public HazirlikRehberi? Getir(int rehberId)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand($"SELECT {Sutunlar} FROM dbo.HazirlikRehberi WHERE RehberId = @id", con);
        cmd.Parameters.AddWithValue("@id", rehberId);
        using var dr = cmd.ExecuteReader();
        return dr.Read() ? Oku(dr) : null;
    }

    public int Ekle(HazirlikRehberi r)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(
            @"INSERT INTO dbo.HazirlikRehberi (AfetTuru, Asama, Baslik, Icerik, Sira)
              VALUES (@tur, @asama, @baslik, @icerik, @sira);
              SELECT CAST(SCOPE_IDENTITY() AS INT);", con);
        Parametreler(cmd, r);
        return (int)cmd.ExecuteScalar()!;
    }

    public void Guncelle(HazirlikRehberi r)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(
            @"UPDATE dbo.HazirlikRehberi
              SET AfetTuru = @tur, Asama = @asama, Baslik = @baslik, Icerik = @icerik,
                  Sira = @sira, GuncellemeTarihi = GETDATE()
              WHERE RehberId = @id", con);
        Parametreler(cmd, r);
        cmd.Parameters.AddWithValue("@id", r.RehberId);
        cmd.ExecuteNonQuery();
    }

    public void Sil(int rehberId)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand("DELETE FROM dbo.HazirlikRehberi WHERE RehberId = @id", con);
        cmd.Parameters.AddWithValue("@id", rehberId);
        cmd.ExecuteNonQuery();
    }

    private static HazirlikRehberi Oku(SqlDataReader dr) => new()
    {
        RehberId = (int)dr["RehberId"],
        AfetTuru = (string)dr["AfetTuru"],
        Asama = (string)dr["Asama"],
        Baslik = (string)dr["Baslik"],
        Icerik = (string)dr["Icerik"],
        Sira = (int)dr["Sira"],
        GuncellemeTarihi = (DateTime)dr["GuncellemeTarihi"]
    };

    private static void Parametreler(SqlCommand cmd, HazirlikRehberi r)
    {
        cmd.Parameters.AddWithValue("@tur", r.AfetTuru);
        cmd.Parameters.AddWithValue("@asama", r.Asama);
        cmd.Parameters.AddWithValue("@baslik", r.Baslik);
        cmd.Parameters.AddWithValue("@icerik", r.Icerik);
        cmd.Parameters.AddWithValue("@sira", r.Sira);
    }
}
