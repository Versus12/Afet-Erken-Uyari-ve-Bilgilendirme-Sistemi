using System;
using System.Collections.Generic;
using System.Windows.Forms;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Business;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    /// <summary>Yönetici: bölge ekleme / güncelleme / silme (Hafta 6).</summary>
    public partial class FrmBolgeYonetimi : Form
    {
        private readonly BolgeManager _manager = new BolgeManager();
        private int _seciliBolgeId;

        public FrmBolgeYonetimi()
        {
            InitializeComponent();
        }

        private void FrmBolgeYonetimi_Load(object sender, EventArgs e)
        {
            ArayuzYardimci.IzgaraHazirla(dgvBolgeler);
            ArayuzYardimci.Kolon(dgvBolgeler, "BolgeId", "No", 45);
            ArayuzYardimci.Kolon(dgvBolgeler, "Il", "İl", 0);
            ArayuzYardimci.Kolon(dgvBolgeler, "Ilce", "İlçe", 0);
            ArayuzYardimci.Kolon(dgvBolgeler, "RiskSeviyesi", "Risk (1-5)", 75);
            ArayuzYardimci.Kolon(dgvBolgeler, "Enlem", "Enlem", 80);
            ArayuzYardimci.Kolon(dgvBolgeler, "Boylam", "Boylam", 80);
            Listele();
        }

        private void Listele()
        {
            List<Bolge> liste = null;
            if (ArayuzYardimci.Calistir(this, () => liste = _manager.Listele()))
            {
                dgvBolgeler.DataSource = liste;
                lblToplam.Text = "Toplam " + liste.Count + " bölge";
            }
            Temizle();
        }

        private Bolge FormdanOku()
        {
            return new Bolge
            {
                BolgeId = _seciliBolgeId,
                Il = txtIl.Text,
                Ilce = txtIlce.Text,
                RiskSeviyesi = (int)nudRisk.Value,
                Enlem = ArayuzYardimci.OndalikOku(txtEnlem, "Enlem"),
                Boylam = ArayuzYardimci.OndalikOku(txtBoylam, "Boylam")
            };
        }

        private void Temizle()
        {
            _seciliBolgeId = 0;
            txtIl.Clear();
            txtIlce.Clear();
            nudRisk.Value = 1;
            txtEnlem.Clear();
            txtBoylam.Clear();
            dgvBolgeler.ClearSelection();
            btnGuncelle.Enabled = btnSil.Enabled = false;
            txtIl.Focus();
        }

        private void dgvBolgeler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Bolge b = ArayuzYardimci.SeciliNesne<Bolge>(dgvBolgeler);
            if (b == null) return;
            _seciliBolgeId = b.BolgeId;
            txtIl.Text = b.Il;
            txtIlce.Text = b.Ilce;
            nudRisk.Value = b.RiskSeviyesi;
            txtEnlem.Text = ArayuzYardimci.OndalikYaz(b.Enlem);
            txtBoylam.Text = ArayuzYardimci.OndalikYaz(b.Boylam);
            btnGuncelle.Enabled = btnSil.Enabled = true;
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            bool ok = ArayuzYardimci.Calistir(this, () =>
            {
                Bolge b = FormdanOku();
                b.BolgeId = 0;
                _manager.Ekle(b);
            });
            if (ok) { ArayuzYardimci.Bilgi("Bölge eklendi."); Listele(); }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (ArayuzYardimci.Calistir(this, () => _manager.Guncelle(FormdanOku())))
            {
                ArayuzYardimci.Bilgi("Bölge güncellendi.");
                Listele();
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (_seciliBolgeId == 0) return;
            if (!ArayuzYardimci.Onayla("Seçili bölge silinsin mi?")) return;
            if (ArayuzYardimci.Calistir(this, () => _manager.Sil(_seciliBolgeId)))
            {
                ArayuzYardimci.Bilgi("Bölge silindi.");
                Listele();
            }
        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            Temizle();
        }
    }
}
