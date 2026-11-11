using System;
using System.Data.SqlClient;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Data
{
    /// <summary>Kullanicilar tablosu için CRUD işlemleri (yalnızca SQL; iş kuralı içermez).</summary>
    public class KullaniciRepository
    {
        private const string Sutunlar = "Id, KullaniciAdi, Eposta, SifreHash, Rol, OlusturmaTarihi, AktifMi";

        public Kullanici KullaniciAdiIleGetir(string kullaniciAdi)
        {
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(
                "SELECT " + Sutunlar + " FROM dbo.Kullanicilar WHERE KullaniciAdi = @ad", con))
            {
                cmd.Parameters.AddWithValue("@ad", kullaniciAdi);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    return dr.Read() ? Oku(dr) : null;
                }
            }
        }

        public bool KullaniciAdiVarMi(string kullaniciAdi)
        {
            return VarMi("SELECT COUNT(*) FROM dbo.Kullanicilar WHERE KullaniciAdi = @deger", kullaniciAdi);
        }

        public bool EpostaVarMi(string eposta)
        {
            return VarMi("SELECT COUNT(*) FROM dbo.Kullanicilar WHERE Eposta = @deger", eposta);
        }

        public int Ekle(Kullanici k)
        {
            const string sql = @"INSERT INTO dbo.Kullanicilar (KullaniciAdi, Eposta, SifreHash, Rol)
                                 VALUES (@ad, @eposta, @hash, @rol);
                                 SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@ad", k.KullaniciAdi);
                cmd.Parameters.AddWithValue("@eposta", k.Eposta);
                cmd.Parameters.AddWithValue("@hash", k.SifreHash);
                cmd.Parameters.AddWithValue("@rol", k.Rol);
                return (int)cmd.ExecuteScalar();
            }
        }

        private static bool VarMi(string sql, string deger)
        {
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@deger", deger);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private static Kullanici Oku(SqlDataReader dr)
        {
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
    }
}
