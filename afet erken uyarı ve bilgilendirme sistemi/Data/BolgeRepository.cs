using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Data
{
    /// <summary>Bolgeler tablosu için CRUD işlemleri.</summary>
    public class BolgeRepository
    {
        public List<Bolge> Listele()
        {
            List<Bolge> liste = new List<Bolge>();
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(
                "SELECT BolgeId, Il, Ilce, BolgeAdi, RiskSeviyesi, Enlem, Boylam FROM dbo.Bolgeler ORDER BY Il, Ilce", con))
            using (SqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    liste.Add(new Bolge
                    {
                        BolgeId = (int)dr["BolgeId"],
                        Il = (string)dr["Il"],
                        Ilce = (string)dr["Ilce"],
                        BolgeAdi = (string)dr["BolgeAdi"],
                        RiskSeviyesi = Convert.ToInt32(dr["RiskSeviyesi"]),
                        Enlem = dr["Enlem"] == DBNull.Value ? (decimal?)null : (decimal)dr["Enlem"],
                        Boylam = dr["Boylam"] == DBNull.Value ? (decimal?)null : (decimal)dr["Boylam"]
                    });
                }
            }
            return liste;
        }

        /// <summary>Aynı il/ilçe çifti başka bir kayıtta var mı? (güncellemede kendi kaydı hariç)</summary>
        public bool AyniBolgeVarMi(string il, string ilce, int haricBolgeId)
        {
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(*) FROM dbo.Bolgeler WHERE Il = @il AND Ilce = @ilce AND BolgeId <> @id", con))
            {
                cmd.Parameters.AddWithValue("@il", il);
                cmd.Parameters.AddWithValue("@ilce", ilce);
                cmd.Parameters.AddWithValue("@id", haricBolgeId);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public int Ekle(Bolge b)
        {
            const string sql = @"INSERT INTO dbo.Bolgeler (Il, Ilce, BolgeAdi, RiskSeviyesi, Enlem, Boylam)
                                 VALUES (@il, @ilce, @ad, @risk, @enlem, @boylam);
                                 SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                Parametreler(cmd, b);
                return (int)cmd.ExecuteScalar();
            }
        }

        public void Guncelle(Bolge b)
        {
            const string sql = @"UPDATE dbo.Bolgeler
                                 SET Il = @il, Ilce = @ilce, BolgeAdi = @ad, RiskSeviyesi = @risk,
                                     Enlem = @enlem, Boylam = @boylam
                                 WHERE BolgeId = @id";
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                Parametreler(cmd, b);
                cmd.Parameters.AddWithValue("@id", b.BolgeId);
                cmd.ExecuteNonQuery();
            }
        }

        public void Sil(int bolgeId)
        {
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand("DELETE FROM dbo.Bolgeler WHERE BolgeId = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", bolgeId);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Bölgeye bağlı abonelik / uyarı / toplanma alanı sayısı (silme kuralı için).</summary>
        public int BagliKayitSayisi(int bolgeId)
        {
            const string sql = @"SELECT
                  (SELECT COUNT(*) FROM dbo.Abonelikler      WHERE BolgeId = @id)
                + (SELECT COUNT(*) FROM dbo.UyariBolgeleri   WHERE BolgeId = @id)
                + (SELECT COUNT(*) FROM dbo.ToplanmaAlanlari WHERE BolgeId = @id)";
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", bolgeId);
                return (int)cmd.ExecuteScalar();
            }
        }

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
}
