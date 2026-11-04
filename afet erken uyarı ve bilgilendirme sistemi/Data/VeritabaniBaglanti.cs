
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace afet_erken_uyari_ve_bilgilendirme_sistemi
{
    public class VeritabaniBaglanti
    {
        private string baglantiAdresi =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=DbHelix;Integrated Security=True;TrustServerCertificate=True;";

        public SqlConnection BaglantiOlustur()
        {
            return new SqlConnection(baglantiAdresi);
        }

        public void BaglantiTestEt()
        {
            try
            {
                using (SqlConnection baglanti = BaglantiOlustur())
                {
                    baglanti.Open();

                    MessageBox.Show("Veritabanı bağlantısı başarılı!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlantı hatası: " + ex.Message);
            }
        }
    }
}
