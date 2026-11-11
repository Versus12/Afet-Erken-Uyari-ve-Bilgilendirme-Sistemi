using AfetErkenUyari.Web.Entities;
using Microsoft.Data.SqlClient;

namespace AfetErkenUyari.Web.Data;

/// <summary>Uyarilar ve UyariBolgeleri tabloları.</summary>
public class UyariRepository(VeritabaniBaglanti db)
{
    private const string Secim = @"
        SELECT u.UyariId, u.Baslik, u.Mesaj, u.AfetTuru, u.Seviye, u.YayinTarihi, u.BitisTarihi,
               u.YayinlayanId, u.AktifMi, k.KullaniciAdi AS YayinlayanAdi,
               STUFF((SELECT ', ' + b.BolgeAdi
                      FROM dbo.UyariBolgeleri ub JOIN dbo.Bolgeler b ON b.BolgeId = ub.BolgeId
                      WHERE ub.UyariId = u.UyariId ORDER BY b.BolgeAdi
                      FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS HedefBolgeler
        FROM dbo.Uyarilar u
        JOIN dbo.Kullanicilar k ON k.Id = u.YayinlayanId";

    /// <summary>Uyarıyı ve hedef bölgelerini TEK transaction içinde ekler (ya hepsi ya hiçbiri).</summary>
    public int Ekle(Uyari u)
    {
        using var con = db.BaglantiAc();
        using var tr = con.BeginTransaction();
        try
        {
            int yeniId;
            using (var cmd = new SqlCommand(
                @"INSERT INTO dbo.Uyarilar (Baslik, Mesaj, AfetTuru, Seviye, YayinTarihi, BitisTarihi, YayinlayanId)
                  VALUES (@baslik, @mesaj, @tur, @seviye, @yayin, @bitis, @yayinlayan);
                  SELECT CAST(SCOPE_IDENTITY() AS INT);", con, tr))
            {
                cmd.Parameters.AddWithValue("@baslik", u.Baslik);
                cmd.Parameters.AddWithValue("@mesaj", u.Mesaj);
                cmd.Parameters.AddWithValue("@tur", u.AfetTuru);
                cmd.Parameters.AddWithValue("@seviye", u.Seviye);
                cmd.Parameters.AddWithValue("@yayin", u.YayinTarihi);
                cmd.Parameters.AddWithValue("@bitis", VeritabaniBaglanti.DbDegeri(u.BitisTarihi));
                cmd.Parameters.AddWithValue("@yayinlayan", u.YayinlayanId);
                yeniId = (int)cmd.ExecuteScalar()!;
            }

            foreach (int bolgeId in u.BolgeIdleri)
            {
                using var cmd = new SqlCommand(
                    "INSERT INTO dbo.UyariBolgeleri (UyariId, BolgeId) VALUES (@uid, @bid)", con, tr);
                cmd.Parameters.AddWithValue("@uid", yeniId);
                cmd.Parameters.AddWithValue("@bid", bolgeId);
                cmd.ExecuteNonQuery();
            }

            tr.Commit();
            return yeniId;
        }
        catch
        {
            tr.Rollback();
            throw;
        }
    }

    public List<Uyari> TumunuListele() => Sorgula(Secim + " ORDER BY u.YayinTarihi DESC", null);

    /// <summary>
    /// Kullanıcının aktif aboneliği olan bölgelere hedeflenmiş, yürürlükteki ve
    /// seviye tercihini karşılayan uyarılar (bölge bazlı filtreleme).
    /// </summary>
    public List<Uyari> KullaniciIcinGecerliUyarilar(int kullaniciId) => Sorgula(Secim + @"
        WHERE u.AktifMi = 1
          AND u.YayinTarihi <= GETDATE()
          AND (u.BitisTarihi IS NULL OR u.BitisTarihi > GETDATE())
          AND EXISTS (SELECT 1 FROM dbo.UyariBolgeleri ub
                      JOIN dbo.Abonelikler a ON a.BolgeId = ub.BolgeId
                      WHERE ub.UyariId = u.UyariId AND a.KullaniciId = @kid
                        AND a.AktifMi = 1 AND u.Seviye >= a.MinSeviye)
        ORDER BY u.Seviye DESC, u.YayinTarihi DESC", kullaniciId);

    public Uyari? Getir(int uyariId)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(Secim + " WHERE u.UyariId = @id", con);
        cmd.Parameters.AddWithValue("@id", uyariId);
        using var dr = cmd.ExecuteReader();
        return dr.Read() ? Oku(dr) : null;
    }

    /// <summary>Silmek yerine yayından kaldırır (geçmiş kaydı korunur).</summary>
    public void YayindanKaldir(int uyariId)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand("UPDATE dbo.Uyarilar SET AktifMi = 0 WHERE UyariId = @id", con);
        cmd.Parameters.AddWithValue("@id", uyariId);
        cmd.ExecuteNonQuery();
    }

    private List<Uyari> Sorgula(string sql, int? kullaniciId)
    {
        var liste = new List<Uyari>();
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(sql, con);
        if (kullaniciId.HasValue) cmd.Parameters.AddWithValue("@kid", kullaniciId.Value);
        using var dr = cmd.ExecuteReader();
        while (dr.Read()) liste.Add(Oku(dr));
        return liste;
    }

    private static Uyari Oku(SqlDataReader dr) => new()
    {
        UyariId = (int)dr["UyariId"],
        Baslik = (string)dr["Baslik"],
        Mesaj = (string)dr["Mesaj"],
        AfetTuru = (string)dr["AfetTuru"],
        Seviye = dr.Tamsayi("Seviye"),
        YayinTarihi = (DateTime)dr["YayinTarihi"],
        BitisTarihi = dr.TarihVeyaNull("BitisTarihi"),
        YayinlayanId = (int)dr["YayinlayanId"],
        AktifMi = (bool)dr["AktifMi"],
        YayinlayanAdi = (string)dr["YayinlayanAdi"],
        HedefBolgeler = dr["HedefBolgeler"] as string ?? ""
    };
}
