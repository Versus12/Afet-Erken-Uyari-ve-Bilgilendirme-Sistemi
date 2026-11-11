using AfetErkenUyari.Web.Entities;
using Microsoft.Data.SqlClient;

namespace AfetErkenUyari.Web.Data;

/// <summary>Bolgeler tablosu için CRUD.</summary>
public class BolgeRepository(VeritabaniBaglanti db)
{
    private const string Sutunlar = "BolgeId, Il, Ilce, BolgeAdi, RiskSeviyesi, Enlem, Boylam";

    public List<Bolge> Listele()
    {
        var liste = new List<Bolge>();
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand($"SELECT {Sutunlar} FROM dbo.Bolgeler ORDER BY Il, Ilce", con);
        using var dr = cmd.ExecuteReader();
        while (dr.Read()) liste.Add(Oku(dr));
        return liste;
    }

    public Bolge? Getir(int bolgeId)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand($"SELECT {Sutunlar} FROM dbo.Bolgeler WHERE BolgeId = @id", con);
        cmd.Parameters.AddWithValue("@id", bolgeId);
        using var dr = cmd.ExecuteReader();
        return dr.Read() ? Oku(dr) : null;
    }

    /// <summary>Aynı il/ilçe başka bir kayıtta var mı? (güncellemede kendi kaydı hariç)</summary>
    public bool AyniBolgeVarMi(string il, string ilce, int haricBolgeId)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(
            "SELECT COUNT(*) FROM dbo.Bolgeler WHERE Il = @il AND Ilce = @ilce AND BolgeId <> @id", con);
        cmd.Parameters.AddWithValue("@il", il);
        cmd.Parameters.AddWithValue("@ilce", ilce);
        cmd.Parameters.AddWithValue("@id", haricBolgeId);
        return (int)cmd.ExecuteScalar()! > 0;
    }

    public int Ekle(Bolge b)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(
            @"INSERT INTO dbo.Bolgeler (Il, Ilce, BolgeAdi, RiskSeviyesi, Enlem, Boylam)
              VALUES (@il, @ilce, @ad, @risk, @enlem, @boylam);
              SELECT CAST(SCOPE_IDENTITY() AS INT);", con);
        Parametreler(cmd, b);
        return (int)cmd.ExecuteScalar()!;
    }

    public void Guncelle(Bolge b)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(
            @"UPDATE dbo.Bolgeler SET Il = @il, Ilce = @ilce, BolgeAdi = @ad, RiskSeviyesi = @risk,
                     Enlem = @enlem, Boylam = @boylam
              WHERE BolgeId = @id", con);
        Parametreler(cmd, b);
        cmd.Parameters.AddWithValue("@id", b.BolgeId);
        cmd.ExecuteNonQuery();
    }

    public void Sil(int bolgeId)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand("DELETE FROM dbo.Bolgeler WHERE BolgeId = @id", con);
        cmd.Parameters.AddWithValue("@id", bolgeId);
        cmd.ExecuteNonQuery();
    }

    /// <summary>Bölgeye bağlı abonelik + uyarı + toplanma alanı sayısı (silme kuralı için).</summary>
    public int BagliKayitSayisi(int bolgeId)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(
            @"SELECT (SELECT COUNT(*) FROM dbo.Abonelikler      WHERE BolgeId = @id)
                   + (SELECT COUNT(*) FROM dbo.UyariBolgeleri   WHERE BolgeId = @id)
                   + (SELECT COUNT(*) FROM dbo.ToplanmaAlanlari WHERE BolgeId = @id)", con);
        cmd.Parameters.AddWithValue("@id", bolgeId);
        return (int)cmd.ExecuteScalar()!;
    }

    private static Bolge Oku(SqlDataReader dr) => new()
    {
        BolgeId = (int)dr["BolgeId"],
        Il = (string)dr["Il"],
        Ilce = (string)dr["Ilce"],
        BolgeAdi = (string)dr["BolgeAdi"],
        RiskSeviyesi = dr.Tamsayi("RiskSeviyesi"),
        Enlem = dr.OndalikVeyaNull("Enlem"),
        Boylam = dr.OndalikVeyaNull("Boylam")
    };

    private static void Parametreler(SqlCommand cmd, Bolge b)
    {
        cmd.Parameters.AddWithValue("@il", b.Il);
        cmd.Parameters.AddWithValue("@ilce", b.Ilce);
        cmd.Parameters.AddWithValue("@ad", b.BolgeAdi);
        cmd.Parameters.AddWithValue("@risk", b.RiskSeviyesi);
        cmd.Parameters.AddWithValue("@enlem", VeritabaniBaglanti.DbDegeri(b.Enlem));
        cmd.Parameters.AddWithValue("@boylam", VeritabaniBaglanti.DbDegeri(b.Boylam));
    }
}
