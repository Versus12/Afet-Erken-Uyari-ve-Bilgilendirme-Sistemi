using System;
using System.Windows.Forms;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Business;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    internal static class Program
    {
        /// <summary>
        /// Uygulama akışı:  Giriş  ->  Ana Menü  ->  (Çıkış yapılırsa) tekrar Giriş
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += (s, e) =>
            {
                ArayuzYardimci.HataKaydet(e.Exception);
                ArayuzYardimci.Hata("Beklenmeyen bir hata oluştu:\n" + e.Exception.Message);
            };

            while (true)
            {
                using (FrmGiris giris = new FrmGiris())
                {
                    if (giris.ShowDialog() != DialogResult.OK)
                        return; // kullanıcı giriş ekranını kapattı
                }

                bool tekrarGiris;
                using (FrmAnaMenu ana = new FrmAnaMenu())
                {
                    ana.ShowDialog();
                    tekrarGiris = ana.GiriseDon;
                }

                Oturum.Kapat();
                if (!tekrarGiris) return;
            }
        }
    }
}
