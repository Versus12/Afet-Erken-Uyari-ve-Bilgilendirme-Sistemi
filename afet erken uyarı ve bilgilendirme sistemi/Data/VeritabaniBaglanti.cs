using System;
using System.Configuration;
using System.Data.SqlClient;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Data
{
    /// <summary>
    /// Veri katmanının tek bağlantı noktası.
    /// Bağlantı cümlesi koda gömülmez; App.config içindeki "AfetDB" kaydından okunur.
    /// </summary>
    public static class VeritabaniBaglanti
    {
        private const string BaglantiAdi = "AfetDB";

        public static string BaglantiCumlesi
        {
            get
            {
                ConnectionStringSettings ayar = ConfigurationManager.ConnectionStrings[BaglantiAdi];
                if (ayar == null || string.IsNullOrWhiteSpace(ayar.ConnectionString))
                    throw new InvalidOperationException(
                        "App.config dosyasında '" + BaglantiAdi + "' bağlantı cümlesi bulunamadı.");
                return ayar.ConnectionString;
            }
        }

        /// <summary>Açılmış yeni bir bağlantı döndürür. Çağıran 'using' ile kapatmalıdır.</summary>
        public static SqlConnection BaglantiAc()
        {
            SqlConnection baglanti = new SqlConnection(BaglantiCumlesi);
            baglanti.Open();
            return baglanti;
        }

        /// <summary>Uygulama açılışında veritabanına erişilebiliyor mu kontrol eder.</summary>
        public static bool BaglantiTestEt(out string hataMesaji)
        {
            try
            {
                using (SqlConnection baglanti = BaglantiAc())
                {
                    hataMesaji = null;
                    return true;
                }
            }
            catch (Exception ex)
            {
                hataMesaji = ex.Message;
                return false;
            }
        }

        /// <summary>DBNull değerini null'a çevirir.</summary>
        public static object DbDegeri(object deger)
        {
            return deger ?? DBNull.Value;
        }
    }
}
