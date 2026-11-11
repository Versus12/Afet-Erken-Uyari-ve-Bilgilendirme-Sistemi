using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Data
{
    /// <summary>Abonelikler tablosu (Kullanıcı - Bölge N-N ara tablosu) için CRUD işlemleri.</summary>
    public class AbonelikRepository
    {
        public List<Abonelik> KullaniciAbonelikleri(int kullaniciId)
        {
            const string sql = @"SELECT a.AbonelikId, a.KullaniciId, a.BolgeId, a.BildirimKanali, a.MinSeviye,
                                        a.AboneTarihi, a.AktifMi, b.BolgeAdi, b.RiskSeviyesi
                                 FROM dbo.Abonelikler a
                                 JOIN dbo.Bolgeler b ON b.BolgeId = a.BolgeId
                                 WHERE a.KullaniciId = @kid
                                 ORDER BY b.BolgeAdi";
            List<Abonelik> liste = new List<Abonelik>();
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@kid", kullaniciId);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        liste.Add(new Abonelik
                        {
                            AbonelikId = (int)dr["AbonelikId"],
                            KullaniciId = (int)dr["KullaniciId"],
                            BolgeId = (int)dr["BolgeId"],
                            BildirimKanali = (string)dr["BildirimKanali"],
                            MinSeviye = Convert.ToInt32(dr["MinSeviye"]),
                            AboneTarihi = (DateTime)dr["AboneTarihi"],
                            AktifMi = (bool)dr["AktifMi"],
                            BolgeAdi = (string)dr["BolgeAdi"],
                            RiskSeviyesi = Convert.ToInt32(dr["RiskSeviyesi"])
                        });
                    }
                }
            }
            return liste;
        }

        public Abonelik Getir(int abonelikId)
        {
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(
                "SELECT AbonelikId, KullaniciId, BolgeId FROM dbo.Abonelikler WHERE AbonelikId = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", abonelikId);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read()) return null;
                    return new Abonelik
                    {
                        AbonelikId = (int)dr["AbonelikId"],
                        KullaniciId = (int)dr["KullaniciId"],
                        BolgeId = (int)dr["BolgeId"]
                    };
                }
            }
        }

        public bool AbonelikVarMi(int kullaniciId, int bolgeId)
        {
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(*) FROM dbo.Abonelikler WHERE KullaniciId = @kid AND BolgeId = @bid", con))
            {
                cmd.Parameters.AddWithValue("@kid", kullaniciId);
                cmd.Parameters.AddWithValue("@bid", bolgeId);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public int AbonelikSayisi(int kullaniciId)
        {
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(*) FROM dbo.Abonelikler WHERE KullaniciId = @kid", con))
            {
                cmd.Parameters.AddWithValue("@kid", kullaniciId);
                return (int)cmd.ExecuteScalar();
            }
        }

        public int Ekle(Abonelik a)
        {
            const string sql = @"INSERT INTO dbo.Abonelikler (KullaniciId, BolgeId, BildirimKanali, MinSeviye)
                                 VALUES (@kid, @bid, @kanal, @min);
                                 SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@kid", a.KullaniciId);
                cmd.Parameters.AddWithValue("@bid", a.BolgeId);
                cmd.Parameters.AddWithValue("@kanal", a.BildirimKanali);
                cmd.Parameters.AddWithValue("@min", a.MinSeviye);
                return (int)cmd.ExecuteScalar();
            }
        }

        public void TercihGuncelle(int abonelikId, string kanal, int minSeviye, bool aktifMi)
        {
            const string sql = @"UPDATE dbo.Abonelikler
                                 SET BildirimKanali = @kanal, MinSeviye = @min, AktifMi = @aktif
                                 WHERE AbonelikId = @id";
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@kanal", kanal);
                cmd.Parameters.AddWithValue("@min", minSeviye);
                cmd.Parameters.AddWithValue("@aktif", aktifMi);
                cmd.Parameters.AddWithValue("@id", abonelikId);
                cmd.ExecuteNonQuery();
            }
        }

        public void Sil(int abonelikId)
        {
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand("DELETE FROM dbo.Abonelikler WHERE AbonelikId = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", abonelikId);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Uyarının ulaşacağı (aktif abone) kişi sayısı - yayın sonrası bilgi için.</summary>
        public int UyariAliciSayisi(int uyariId)
        {
            const string sql = @"SELECT COUNT(DISTINCT a.KullaniciId)
                                 FROM dbo.Abonelikler a
                                 JOIN dbo.UyariBolgeleri ub ON ub.BolgeId = a.BolgeId
                                 JOIN dbo.Uyarilar u ON u.UyariId = ub.UyariId
                                 WHERE ub.UyariId = @uid AND a.AktifMi = 1 AND u.Seviye >= a.MinSeviye";
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@uid", uyariId);
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}
