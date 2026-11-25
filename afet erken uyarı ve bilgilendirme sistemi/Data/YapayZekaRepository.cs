using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Data
{
    /// <summary>YapayZekaKayitlari tablosu: AI günlüğü (Hafta 10).</summary>
    public class YapayZekaRepository
    {
        public int Ekle(YapayZekaKaydi k)
        {
            const string sql = @"INSERT INTO dbo.YapayZekaKayitlari
                (KullaniciId, Saglayici, HedefBolgeler, KaynakMetin, Istem, HamCikti, Baslik, Mesaj,
                 OnerilenSeviye, Gecerli, DogrulamaNotlari, SureMs)
                VALUES (@kid, @saglayici, @bolgeler, @kaynak, @istem, @ham, @baslik, @mesaj,
                        @seviye, @gecerli, @notlar, @sure);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@kid", k.KullaniciId);
                cmd.Parameters.AddWithValue("@saglayici", Kisalt(k.Saglayici, 100));
                cmd.Parameters.AddWithValue("@bolgeler", Kisalt(k.HedefBolgeler, 500));
                cmd.Parameters.AddWithValue("@kaynak", Kisalt(k.KaynakMetin, 4000));
                cmd.Parameters.AddWithValue("@istem", k.Istem);
                cmd.Parameters.AddWithValue("@ham", VeritabaniBaglanti.DbDegeri(k.HamCikti));
                cmd.Parameters.AddWithValue("@baslik", VeritabaniBaglanti.DbDegeri(Kisalt(k.Baslik, 150)));
                cmd.Parameters.AddWithValue("@mesaj", VeritabaniBaglanti.DbDegeri(Kisalt(k.Mesaj, 2000)));
                cmd.Parameters.AddWithValue("@seviye", VeritabaniBaglanti.DbDegeri(k.OnerilenSeviye));
                cmd.Parameters.AddWithValue("@gecerli", k.Gecerli);
                cmd.Parameters.AddWithValue("@notlar", VeritabaniBaglanti.DbDegeri(Kisalt(k.DogrulamaNotlari, 2000)));
                cmd.Parameters.AddWithValue("@sure", VeritabaniBaglanti.DbDegeri(k.SureMs));
                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>En son kayıtlar (en yeni üstte).</summary>
        public List<YapayZekaKaydi> SonKayitlar(int adet)
        {
            const string sql = @"SELECT TOP (@adet) y.*, k.KullaniciAdi
                                 FROM dbo.YapayZekaKayitlari y
                                 JOIN dbo.Kullanicilar k ON k.Id = y.KullaniciId
                                 ORDER BY y.KayitId DESC";
            List<YapayZekaKaydi> liste = new List<YapayZekaKaydi>();
            using (SqlConnection con = VeritabaniBaglanti.BaglantiAc())
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@adet", adet);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        liste.Add(new YapayZekaKaydi
                        {
                            KayitId = (int)dr["KayitId"],
                            KullaniciId = (int)dr["KullaniciId"],
                            Tarih = (DateTime)dr["Tarih"],
                            Saglayici = (string)dr["Saglayici"],
                            HedefBolgeler = (string)dr["HedefBolgeler"],
                            KaynakMetin = (string)dr["KaynakMetin"],
                            Istem = (string)dr["Istem"],
                            HamCikti = dr["HamCikti"] as string,
                            Baslik = dr["Baslik"] as string,
                            Mesaj = dr["Mesaj"] as string,
                            OnerilenSeviye = dr["OnerilenSeviye"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["OnerilenSeviye"]),
                            Gecerli = (bool)dr["Gecerli"],
                            DogrulamaNotlari = dr["DogrulamaNotlari"] as string,
                            SureMs = dr["SureMs"] == DBNull.Value ? (int?)null : (int)dr["SureMs"],
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
