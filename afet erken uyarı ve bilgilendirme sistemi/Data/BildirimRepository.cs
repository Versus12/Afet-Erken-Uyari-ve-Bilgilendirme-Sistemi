using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Data
{
    /// <summary>Bildirimler tablosu: e-posta kuyruğu ve gönderim kaydı (Hafta 9).</summary>
    public class BildirimRepository
    {
        /// <summary>
        /// Uyarının ulaşacağı ve E-posta kanalını seçmiş aboneleri kuyruğa ekler.
        /// Uyarı ile AYNI transaction içinde çağrılır: uyarı kaydedilemezse kuyruk da oluşmaz.
        /// Kullanıcı birden çok hedef bölgeye abone olsa bile tek e-posta alır (DISTINCT + UQ_Bildirimler).
        /// </summary>
        public static int KuyrugaEkle(SqlConnection con, SqlTransaction tr, int uyariId)
        {
            const string sql = @"
                INSERT INTO dbo.Bildirimler (UyariId, KullaniciId, Kanal, Alici)
                SELECT DISTINCT @uid, k.Id, N'Eposta', k.Eposta
                FROM dbo.UyariBolgeleri ub
                JOIN dbo.Uyarilar u     ON u.UyariId = ub.UyariId
                JOIN dbo.Abonelikler a  ON a.BolgeId = ub.BolgeId
                JOIN dbo.Kullanicilar k ON k.Id = a.KullaniciId
                WHERE ub.UyariId = @uid
                  AND a.AktifMi = 1
                  AND a.BildirimKanali = N'Eposta'
                  AND u.Seviye >= a.MinSeviye
                  AND k.AktifMi = 1
                  AND NOT EXISTS (SELECT 1 FROM dbo.Bildirimler b
                                  WHERE b.UyariId = @uid AND b.KullaniciId = k.Id AND b.Kanal = N'Eposta');";
            using (SqlCommand cmd = new SqlCommand(sql, con, tr))
            {
                cmd.Parameters.AddWithValue("@uid", uyariId);
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Gönderilmeyi bekleyen kayıtlar.</summary>
        public List<Bildirim> Bekleyenler(int uyariId)
        {
            return Sorgula("WHERE b.UyariId = @uid AND b.Durum = N'Bekliyor'", uyariId);
        }

        /// <summary>Bir uyarının tüm gönderim kayıtları (yönetici ekranı).</summary>
        public List<Bildirim> Listele(int uyariId)
        {
            return Sorgula("WHERE b.UyariId = @uid", uyariId);
        }

        /// <summary>Başarısız kayıtları yeniden denemek için tekrar kuyruğa alır.</summary>
        public int BasarisizlariKuyrugaAl(int uyariId)
        {
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(
                "UPDATE dbo.Bildirimler SET Durum = N'Bekliyor' WHERE UyariId = @uid AND Durum = N'Basarisiz'", con))
            {
                cmd.Parameters.AddWithValue("@uid", uyariId);
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Bir gönderim denemesinin sonucunu yazar.</summary>
        public void SonucYaz(int bildirimId, string durum, int denemeSayisi, string hata, string saglayiciMesajId)
        {
            const string sql = @"UPDATE dbo.Bildirimler
                                 SET Durum = @durum, DenemeSayisi = @deneme, SonHata = @hata,
                                     SaglayiciMesajId = @mid,
                                     GonderimTarihi = CASE WHEN @durum IN (N'Gonderildi', N'DenemeModu') THEN GETDATE() ELSE GonderimTarihi END
                                 WHERE BildirimId = @id";
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@durum", durum);
                cmd.Parameters.AddWithValue("@deneme", Math.Min(denemeSayisi, 255));
                cmd.Parameters.AddWithValue("@hata", VeritabaniBaglanti.DbDegeri(Kisalt(hata, 500)));
                cmd.Parameters.AddWithValue("@mid", VeritabaniBaglanti.DbDegeri(Kisalt(saglayiciMesajId, 200)));
                cmd.Parameters.AddWithValue("@id", bildirimId);
                cmd.ExecuteNonQuery();
            }
        }

        private static List<Bildirim> Sorgula(string kosul, int uyariId)
        {
            string sql = @"SELECT b.BildirimId, b.UyariId, b.KullaniciId, b.Kanal, b.Alici, b.Durum, b.DenemeSayisi,
                                  b.SonHata, b.SaglayiciMesajId, b.OlusturmaTarihi, b.GonderimTarihi, k.KullaniciAdi
                           FROM dbo.Bildirimler b
                           JOIN dbo.Kullanicilar k ON k.Id = b.KullaniciId " + kosul + @"
                           ORDER BY b.BildirimId";
            List<Bildirim> liste = new List<Bildirim>();
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@uid", uyariId);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        liste.Add(new Bildirim
                        {
                            BildirimId = (int)dr["BildirimId"],
                            UyariId = (int)dr["UyariId"],
                            KullaniciId = (int)dr["KullaniciId"],
                            Kanal = (string)dr["Kanal"],
                            Alici = (string)dr["Alici"],
                            Durum = (string)dr["Durum"],
                            DenemeSayisi = Convert.ToInt32(dr["DenemeSayisi"]),
                            SonHata = dr["SonHata"] as string,
                            SaglayiciMesajId = dr["SaglayiciMesajId"] as string,
                            OlusturmaTarihi = (DateTime)dr["OlusturmaTarihi"],
                            GonderimTarihi = dr["GonderimTarihi"] == DBNull.Value ? (DateTime?)null : (DateTime)dr["GonderimTarihi"],
                            KullaniciAdi = (string)dr["KullaniciAdi"]
                        });
                    }
                }
            }
            return liste;
        }

        private static string Kisalt(string s, int uzunluk)
        {
            return s == null || s.Length <= uzunluk ? s : s.Substring(0, uzunluk);
        }
    }
}
