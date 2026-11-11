using AfetErkenUyari.Web.Entities;
using Microsoft.Data.SqlClient;

namespace AfetErkenUyari.Web.Data;

/// <summary>ToplanmaAlanlari tablosu için CRUD.</summary>
public class ToplanmaAlaniRepository(VeritabaniBaglanti db)
{
    private const string Secim = @"
        SELECT t.AlanId, t.BolgeId, t.AlanAdi, t.Adres, t.Kapasite, t.Enlem, t.Boylam, b.BolgeAdi
        FROM dbo.ToplanmaAlanlari t
        JOIN dbo.Bolgeler b ON b.BolgeId = t.BolgeId";

    /// <summary>bolgeId null ise tüm alanlar döner.</summary>
    public List<ToplanmaAlani> Listele(int? bolgeId)
    {
        var liste = new List<ToplanmaAlani>();
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(Secim + " WHERE (@bid IS NULL OR t.BolgeId = @bid) ORDER BY b.BolgeAdi, t.AlanAdi", con);
        cmd.Parameters.Add("@bid", System.Data.SqlDbType.Int).Value = VeritabaniBaglanti.DbDegeri(bolgeId);
        using var dr = cmd.ExecuteReader();
        while (dr.Read()) liste.Add(Oku(dr));
        return liste;
    }

    public ToplanmaAlani? Getir(int alanId)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(Secim + " WHERE t.AlanId = @id", con);
        cmd.Parameters.AddWithValue("@id", alanId);
        using var dr = cmd.ExecuteReader();
        return dr.Read() ? Oku(dr) : null;
    }

    public int Ekle(ToplanmaAlani t)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(
            @"INSERT INTO dbo.ToplanmaAlanlari (BolgeId, AlanAdi, Adres, Kapasite, Enlem, Boylam)
              VALUES (@bid, @ad, @adres, @kapasite, @enlem, @boylam);
              SELECT CAST(SCOPE_IDENTITY() AS INT);", con);
        Parametreler(cmd, t);
        return (int)cmd.ExecuteScalar()!;
    }

    public void Guncelle(ToplanmaAlani t)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(
            @"UPDATE dbo.ToplanmaAlanlari
              SET BolgeId = @bid, AlanAdi = @ad, Adres = @adres, Kapasite = @kapasite, Enlem = @enlem, Boylam = @boylam
              WHERE AlanId = @id", con);
        Parametreler(cmd, t);
        cmd.Parameters.AddWithValue("@id", t.AlanId);
        cmd.ExecuteNonQuery();
    }

    public void Sil(int alanId)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand("DELETE FROM dbo.ToplanmaAlanlari WHERE AlanId = @id", con);
        cmd.Parameters.AddWithValue("@id", alanId);
        cmd.ExecuteNonQuery();
    }

    private static ToplanmaAlani Oku(SqlDataReader dr) => new()
    {
        AlanId = (int)dr["AlanId"],
        BolgeId = (int)dr["BolgeId"],
        AlanAdi = (string)dr["AlanAdi"],
        Adres = (string)dr["Adres"],
        Kapasite = (int)dr["Kapasite"],
        Enlem = dr.OndalikVeyaNull("Enlem"),
        Boylam = dr.OndalikVeyaNull("Boylam"),
        BolgeAdi = (string)dr["BolgeAdi"]
    };

    private static void Parametreler(SqlCommand cmd, ToplanmaAlani t)
    {
        cmd.Parameters.AddWithValue("@bid", t.BolgeId);
        cmd.Parameters.AddWithValue("@ad", t.AlanAdi);
        cmd.Parameters.AddWithValue("@adres", t.Adres);
        cmd.Parameters.AddWithValue("@kapasite", t.Kapasite);
        cmd.Parameters.AddWithValue("@enlem", VeritabaniBaglanti.DbDegeri(t.Enlem));
        cmd.Parameters.AddWithValue("@boylam", VeritabaniBaglanti.DbDegeri(t.Boylam));
    }
}
