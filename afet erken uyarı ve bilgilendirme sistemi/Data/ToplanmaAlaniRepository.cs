using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Data
{
    /// <summary>ToplanmaAlanlari tablosu için CRUD işlemleri.</summary>
    public class ToplanmaAlaniRepository
    {
        /// <summary>bolgeId null ise tüm alanlar döner.</summary>
        public List<ToplanmaAlani> Listele(int? bolgeId)
        {
            const string sql = @"SELECT t.AlanId, t.BolgeId, t.AlanAdi, t.Adres, t.Kapasite, t.Enlem, t.Boylam,
                                        b.BolgeAdi
                                 FROM dbo.ToplanmaAlanlari t
                                 JOIN dbo.Bolgeler b ON b.BolgeId = t.BolgeId
                                 WHERE (@bid IS NULL OR t.BolgeId = @bid)
                                 ORDER BY b.BolgeAdi, t.AlanAdi";
            List<ToplanmaAlani> liste = new List<ToplanmaAlani>();
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@bid", VeritabaniBaglanti.DbDegeri(bolgeId));
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        liste.Add(new ToplanmaAlani
                        {
                            AlanId = (int)dr["AlanId"],
                            BolgeId = (int)dr["BolgeId"],
                            AlanAdi = (string)dr["AlanAdi"],
                            Adres = (string)dr["Adres"],
                            Kapasite = (int)dr["Kapasite"],
                            Enlem = dr["Enlem"] == DBNull.Value ? (decimal?)null : (decimal)dr["Enlem"],
                            Boylam = dr["Boylam"] == DBNull.Value ? (decimal?)null : (decimal)dr["Boylam"],
                            BolgeAdi = (string)dr["BolgeAdi"]
                        });
                    }
                }
            }
            return liste;
        }

        public int Ekle(ToplanmaAlani t)
        {
            const string sql = @"INSERT INTO dbo.ToplanmaAlanlari (BolgeId, AlanAdi, Adres, Kapasite, Enlem, Boylam)
                                 VALUES (@bid, @ad, @adres, @kapasite, @enlem, @boylam);
                                 SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                Parametreler(cmd, t);
                return (int)cmd.ExecuteScalar();
            }
        }

        public void Guncelle(ToplanmaAlani t)
        {
            const string sql = @"UPDATE dbo.ToplanmaAlanlari
                                 SET BolgeId = @bid, AlanAdi = @ad, Adres = @adres, Kapasite = @kapasite,
                                     Enlem = @enlem, Boylam = @boylam
                                 WHERE AlanId = @id";
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                Parametreler(cmd, t);
                cmd.Parameters.AddWithValue("@id", t.AlanId);
                cmd.ExecuteNonQuery();
            }
        }

        public void Sil(int alanId)
        {
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand("DELETE FROM dbo.ToplanmaAlanlari WHERE AlanId = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", alanId);
                cmd.ExecuteNonQuery();
            }
        }

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
}
