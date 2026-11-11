using AfetErkenUyari.Web.Entities;
using Microsoft.Data.SqlClient;

namespace AfetErkenUyari.Web.Data;

/// <summary>Kullanicilar tablosu (yalnızca SQL; iş kuralı içermez).</summary>
public class KullaniciRepository(VeritabaniBaglanti db)
{
    public Kullanici? KullaniciAdiIleGetir(string kullaniciAdi)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(
            @"SELECT Id, KullaniciAdi, Eposta, SifreHash, Rol, OlusturmaTarihi, AktifMi
              FROM dbo.Kullanicilar WHERE KullaniciAdi = @ad", con);
        cmd.Parameters.AddWithValue("@ad", kullaniciAdi);
        using var dr = cmd.ExecuteReader();
        if (!dr.Read()) return null;
        return new Kullanici
        {
            Id = (int)dr["Id"],
            KullaniciAdi = (string)dr["KullaniciAdi"],
            Eposta = (string)dr["Eposta"],
            SifreHash = (string)dr["SifreHash"],
            Rol = (string)dr["Rol"],
            OlusturmaTarihi = (DateTime)dr["OlusturmaTarihi"],
            AktifMi = (bool)dr["AktifMi"]
        };
    }

    public bool KullaniciAdiVarMi(string kullaniciAdi) =>
        VarMi("SELECT COUNT(*) FROM dbo.Kullanicilar WHERE KullaniciAdi = @deger", kullaniciAdi);

    public bool EpostaVarMi(string eposta) =>
        VarMi("SELECT COUNT(*) FROM dbo.Kullanicilar WHERE Eposta = @deger", eposta);

    public int Ekle(Kullanici k)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(
            @"INSERT INTO dbo.Kullanicilar (KullaniciAdi, Eposta, SifreHash, Rol)
              VALUES (@ad, @eposta, @hash, @rol);
              SELECT CAST(SCOPE_IDENTITY() AS INT);", con);
        cmd.Parameters.AddWithValue("@ad", k.KullaniciAdi);
        cmd.Parameters.AddWithValue("@eposta", k.Eposta);
        cmd.Parameters.AddWithValue("@hash", k.SifreHash);
        cmd.Parameters.AddWithValue("@rol", k.Rol);
        return (int)cmd.ExecuteScalar()!;
    }

    private bool VarMi(string sql, string deger)
    {
        using var con = db.BaglantiAc();
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@deger", deger);
        return (int)cmd.ExecuteScalar()! > 0;
    }
}
