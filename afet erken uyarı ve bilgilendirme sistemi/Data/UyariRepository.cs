using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Data
{
    /// <summary>Uyarilar ve UyariBolgeleri tabloları için veri erişimi.</summary>
    public class UyariRepository
    {
        // Hedef bölge adlarını virgülle birleştirir (SQL Server 2016 ve öncesiyle de uyumlu).
        private const string HedefBolgelerSql = @"
            STUFF((SELECT ', ' + b.BolgeAdi
                   FROM dbo.UyariBolgeleri ub JOIN dbo.Bolgeler b ON b.BolgeId = ub.BolgeId
                   WHERE ub.UyariId = u.UyariId
                   ORDER BY b.BolgeAdi
                   FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '')";

        /// <summary>
        /// Uyarıyı ve hedef bölgelerini TEK bir transaction içinde ekler.
        /// Herhangi bir adım başarısız olursa hiçbir kayıt eklenmez (atomiklik).
        /// </summary>
        public int Ekle(Uyari u)
        {
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlTransaction tr = con.BeginTransaction())
            {
                try
                {
                    int yeniId;
                    const string sql = @"INSERT INTO dbo.Uyarilar
                                           (Baslik, Mesaj, AfetTuru, Seviye, YayinTarihi, BitisTarihi, YayinlayanId)
                                         VALUES (@baslik, @mesaj, @tur, @seviye, @yayin, @bitis, @yayinlayan);
                                         SELECT CAST(SCOPE_IDENTITY() AS INT);";
                    using (SqlCommand cmd = new SqlCommand(sql, con, tr))
                    {
                        cmd.Parameters.AddWithValue("@baslik", u.Baslik);
                        cmd.Parameters.AddWithValue("@mesaj", u.Mesaj);
                        cmd.Parameters.AddWithValue("@tur", u.AfetTuru);
                        cmd.Parameters.AddWithValue("@seviye", u.Seviye);
                        cmd.Parameters.AddWithValue("@yayin", u.YayinTarihi);
                        cmd.Parameters.AddWithValue("@bitis", VeritabaniBaglanti.DbDegeri(u.BitisTarihi));
                        cmd.Parameters.AddWithValue("@yayinlayan", u.YayinlayanId);
                        yeniId = (int)cmd.ExecuteScalar();
                    }

                    foreach (int bolgeId in u.BolgeIdleri)
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "INSERT INTO dbo.UyariBolgeleri (UyariId, BolgeId) VALUES (@uid, @bid)", con, tr))
                        {
                            cmd.Parameters.AddWithValue("@uid", yeniId);
                            cmd.Parameters.AddWithValue("@bid", bolgeId);
                            cmd.ExecuteNonQuery();
                        }
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
        }

        /// <summary>Yönetim paneli için tüm uyarılar (en yeni üstte).</summary>
        public List<Uyari> TumunuListele()
        {
            string sql = @"SELECT u.UyariId, u.Baslik, u.Mesaj, u.AfetTuru, u.Seviye, u.YayinTarihi,
                                  u.BitisTarihi, u.YayinlayanId, u.AktifMi, k.KullaniciAdi AS YayinlayanAdi, "
                         + HedefBolgelerSql + @" AS HedefBolgeler
                           FROM dbo.Uyarilar u
                           JOIN dbo.Kullanicilar k ON k.Id = u.YayinlayanId
                           ORDER BY u.YayinTarihi DESC";
            return Sorgula(sql, null);
        }

        /// <summary>
        /// Kullanıcının abone olduğu bölgelere hedeflenmiş, şu an yürürlükte olan ve
        /// kullanıcının seviye tercihini karşılayan uyarılar (konum/bölge bazlı filtreleme).
        /// </summary>
        public List<Uyari> KullaniciIcinGecerliUyarilar(int kullaniciId)
        {
            string sql = @"SELECT u.UyariId, u.Baslik, u.Mesaj, u.AfetTuru, u.Seviye, u.YayinTarihi,
                                  u.BitisTarihi, u.YayinlayanId, u.AktifMi, k.KullaniciAdi AS YayinlayanAdi, "
                         + HedefBolgelerSql + @" AS HedefBolgeler
                           FROM dbo.Uyarilar u
                           JOIN dbo.Kullanicilar k ON k.Id = u.YayinlayanId
                           WHERE u.AktifMi = 1
                             AND u.YayinTarihi <= GETDATE()
                             AND (u.BitisTarihi IS NULL OR u.BitisTarihi > GETDATE())
                             AND EXISTS (SELECT 1
                                         FROM dbo.UyariBolgeleri ub
                                         JOIN dbo.Abonelikler a ON a.BolgeId = ub.BolgeId
                                         WHERE ub.UyariId = u.UyariId
                                           AND a.KullaniciId = @kid
                                           AND a.AktifMi = 1
                                           AND u.Seviye >= a.MinSeviye)
                           ORDER BY u.Seviye DESC, u.YayinTarihi DESC";
            return Sorgula(sql, kullaniciId);
        }

        public Uyari Getir(int uyariId)
        {
            string sql = @"SELECT u.UyariId, u.Baslik, u.Mesaj, u.AfetTuru, u.Seviye, u.YayinTarihi,
                                  u.BitisTarihi, u.YayinlayanId, u.AktifMi, k.KullaniciAdi AS YayinlayanAdi, "
                         + HedefBolgelerSql + @" AS HedefBolgeler
                           FROM dbo.Uyarilar u
                           JOIN dbo.Kullanicilar k ON k.Id = u.YayinlayanId
                           WHERE u.UyariId = @id";
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@id", uyariId);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    return dr.Read() ? Oku(dr) : null;
                }
            }
        }

        /// <summary>Uyarıyı silmek yerine yayından kaldırır (geçmiş kaydı korunur).</summary>
        public void YayindanKaldir(int uyariId)
        {
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand("UPDATE dbo.Uyarilar SET AktifMi = 0 WHERE UyariId = @id", con))
            {
                cmd.Parameters.AddWithValue("@id", uyariId);
                cmd.ExecuteNonQuery();
            }
        }

        private static List<Uyari> Sorgula(string sql, int? kullaniciId)
        {
            List<Uyari> liste = new List<Uyari>();
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                if (kullaniciId.HasValue) cmd.Parameters.AddWithValue("@kid", kullaniciId.Value);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read()) liste.Add(Oku(dr));
                }
            }
            return liste;
        }

        private static Uyari Oku(SqlDataReader dr)
        {
            return new Uyari
            {
                UyariId = (int)dr["UyariId"],
                Baslik = (string)dr["Baslik"],
                Mesaj = (string)dr["Mesaj"],
                AfetTuru = (string)dr["AfetTuru"],
                Seviye = Convert.ToInt32(dr["Seviye"]),
                YayinTarihi = (DateTime)dr["YayinTarihi"],
                BitisTarihi = dr["BitisTarihi"] == DBNull.Value ? (DateTime?)null : (DateTime)dr["BitisTarihi"],
                YayinlayanId = (int)dr["YayinlayanId"],
                AktifMi = (bool)dr["AktifMi"],
                YayinlayanAdi = (string)dr["YayinlayanAdi"],
                HedefBolgeler = dr["HedefBolgeler"] == DBNull.Value ? "" : (string)dr["HedefBolgeler"]
            };
        }
    }
}
