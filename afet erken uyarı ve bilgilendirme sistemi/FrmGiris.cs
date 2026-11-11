using System;
using System.Drawing;
using System.Windows.Forms;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Business;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Data;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    /// <summary>Giriş ekranı. Başarılı girişte DialogResult.OK ile kapanır.</summary>
    public partial class FrmGiris : Form
    {
        private readonly KimlikDogrulamaManager _kimlik = new KimlikDogrulamaManager();

        public FrmGiris()
        {
            InitializeComponent();
        }

        private void FrmGiris_Load(object sender, EventArgs e)
        {
            string hata;
            if (VeritabaniBaglanti.BaglantiTestEt(out hata))
            {
                lblDurum.Text = "● Veritabanı bağlantısı hazır";
                lblDurum.ForeColor = Color.SeaGreen;
            }
            else
            {
                lblDurum.Text = "● Veritabanına bağlanılamadı (App.config > AfetDB)";
                lblDurum.ForeColor = Color.Firebrick;
                ArayuzYardimci.HataKaydet(new Exception(hata));
            }
        }

        private void btnGirisYap_Click(object sender, EventArgs e)
        {
            bool basarili = ArayuzYardimci.Calistir(this, () =>
                _kimlik.GirisYap(txtKullaniciAdi.Text, txtSifre.Text));

            if (basarili)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                txtSifre.Clear();
                txtSifre.Focus();
            }
        }

        private void chkSifreGoster_CheckedChanged(object sender, EventArgs e)
        {
            txtSifre.UseSystemPasswordChar = !chkSifreGoster.Checked;
        }

        private void lnkKayitOl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (FrmKayit kayit = new FrmKayit())
            {
                if (kayit.ShowDialog(this) == DialogResult.OK)
                {
                    txtKullaniciAdi.Text = kayit.KayitOlanKullaniciAdi;
                    txtSifre.Clear();
                    txtSifre.Focus();
                }
            }
        }
    }
}
