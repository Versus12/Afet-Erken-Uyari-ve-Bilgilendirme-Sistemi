using System;
using System.Windows.Forms;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Business;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    /// <summary>Vatandaş kayıt ekranı. Başarılı kayıtta DialogResult.OK ile kapanır.</summary>
    public partial class FrmKayit : Form
    {
        private readonly KimlikDogrulamaManager _kimlik = new KimlikDogrulamaManager();

        public string KayitOlanKullaniciAdi { get; private set; }

        public FrmKayit()
        {
            InitializeComponent();
        }

        private void btnKayitOl_Click(object sender, EventArgs e)
        {
            Kullanici yeni = null;
            bool basarili = ArayuzYardimci.Calistir(this, () =>
            {
                yeni = _kimlik.KayitOl(txtKullaniciAdi.Text, txtEposta.Text, txtSifre.Text, txtSifreTekrar.Text);
            });

            if (basarili)
            {
                KayitOlanKullaniciAdi = yeni.KullaniciAdi;
                ArayuzYardimci.Bilgi("Hesabınız oluşturuldu. Şimdi giriş yapabilirsiniz.");
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnVazgec_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
