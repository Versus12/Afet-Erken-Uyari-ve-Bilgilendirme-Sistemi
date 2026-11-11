using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Data
{
    /// <summary>HazirlikRehberi tablosu için CRUD işlemleri.</summary>
    public class RehberRepository
    {
        /// <summary>afetTuru null ise tüm rehber kayıtları döner.</summary>
        public List<HazirlikRehberi> Listele(string afetTuru)
        {
            const string sql = @"SELECT RehberId, AfetTuru, Asama, Baslik, Icerik, Sira, GuncellemeTarihi
                                 FROM dbo.HazirlikRehberi
                                 WHERE (@tur IS NULL OR AfetTuru = @tur)
                                 ORDER BY AfetTuru,
                                          CASE Asama WHEN N'Önce' THEN 1 WHEN N'Sırasında' THEN 2 ELSE 3 END,
                                          Sira";
            List<HazirlikRehberi> liste = new List<HazirlikRehberi>();
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@tur", VeritabaniBaglanti.DbDegeri(afetTuru));
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        liste.Add(new HazirlikRehberi
                        {
                            RehberId = (int)dr["RehberId"],
                            AfetTuru = (string)dr["AfetTuru"],
                            Asama = (string)dr["Asama"],
                            Baslik = (string)dr["Baslik"],
                            Icerik = (string)dr["Icerik"],
                            Sira = (int)dr["Sira"],
                            GuncellemeTarihi = (DateTime)dr["GuncellemeTarihi"]
                        });
                    }
                }
            }
            return liste;
        }

        public int Ekle(HazirlikRehberi r)
        {
            const string sql = @"INSERT INTO dbo.HazirlikRehberi (AfetTuru, Asama, Baslik, Icerik, Sira)
                                 VALUES (@tur, @asama, @baslik, @icerik, @sira);
                                 SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                Parametreler(cmd, r);
                return (int)cmd.ExecuteScalar();
            }
        }

        public void Guncelle(HazirlikRehberi r)
        {
            const string sql = @"UPDATE dbo.HazirlikRehberi
                                 SET AfetTuru = @tur, Asama = @asama, Baslik = @baslik, Icerik = @icerik,
                                     Sira = @sira, GuncellemeTarihi = GETDATE()
                                 WHERE RehberId = @id";
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                Parametreler(cmd, r);
                cmd.Parameters.AddWithValue("@id", r.RehberId);
                cmd.ExecuteNonQuery();
            }
        }

        public void Sil(int rehberId)
        {
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand("DELETE FROM dbo.HazirlikRehberi WHERE RehberId = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", rehberId);
                cmd.ExecuteNonQuery();
            }
        }

        private static void Parametreler(SqlCommand cmd, HazirlikRehberi r)
        {
            cmd.Parameters.AddWithValue("@tur", r.AfetTuru);
            cmd.Parameters.AddWithValue("@asama", r.Asama);
            cmd.Parameters.AddWithValue("@baslik", r.Baslik);
            cmd.Parameters.AddWithValue("@icerik", r.Icerik);
            cmd.Parameters.AddWithValue("@sira", r.Sira);
        }
    }
}
