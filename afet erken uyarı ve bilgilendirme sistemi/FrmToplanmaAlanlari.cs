using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Business;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    /// <summary>
    /// Toplanma alanları listesi (bölge filtreli). Yönetici alan ekleyip düzenleyebilir.
    /// Harita gösterimi ve en yakın alan hesabı Hafta 11 kapsamındadır.
    /// </summary>
    public partial class FrmToplanmaAlanlari : Form
    {
        private readonly ToplanmaAlaniManager _manager = new ToplanmaAlaniManager();
        private readonly BolgeManager _bolgeManager = new BolgeManager();
        private readonly AbonelikManager _abonelikManager = new AbonelikManager();
        private bool _yukleniyor = true;
        private int _seciliId;

        public FrmToplanmaAlanlari()
        {
            InitializeComponent();
        }

        private void FrmToplanmaAlanlari_Load(object sender, EventArgs e)
        {
            grpDuzenle.Visible = Oturum.AktifKullanici != null && Oturum.AktifKullanici.YoneticiMi;

            ArayuzYardimci.IzgaraHazirla(dgvAlanlar);
            ArayuzYardimci.Kolon(dgvAlanlar, "BolgeAdi", "Bölge", 140);
            ArayuzYardimci.Kolon(dgvAlanlar, "AlanAdi", "Alan Adı", 0);
            ArayuzYardimci.Kolon(dgvAlanlar, "Adres", "Adres", 200);
            ArayuzYardimci.Kolon(dgvAlanlar, "Kapasite", "Kapasite", 70, "N0");

            List<Bolge> bolgeler = null;
            if (ArayuzYardimci.Calistir(this, () => bolgeler = _bolgeManager.Listele()))
            {
                List<Bolge> filtre = new List<Bolge> { new Bolge { BolgeId = 0, BolgeAdi = "(Tüm bölgeler)" } };
                filtre.AddRange(bolgeler);
                cmbFiltre.DisplayMember = "BolgeAdi";
                cmbFiltre.DataSource = filtre;

                cmbBolge.DisplayMember = "BolgeAdi";
                cmbBolge.DataSource = new List<Bolge>(bolgeler);
            }
            _yukleniyor = false;
            Listele();
        }

        private void Filtre_Degisti(object sender, EventArgs e)
        {
            if (!_yukleniyor) Listele();
        }

        private void Listele()
        {
            Bolge filtre = cmbFiltre.SelectedItem as Bolge;
            int? bolgeId = filtre == null || filtre.BolgeId == 0 ? (int?)null : filtre.BolgeId;

            List<ToplanmaAlani> liste = null;
            bool ok = ArayuzYardimci.Calistir(this, () =>
            {
                liste = _manager.Listele(bolgeId);
                if (chkAboneliklerim.Checked)
                {
                    // Modül entegrasyonu: abonelik modülündeki bölgelerle filtrele
                    HashSet<int> aboneBolgeler = new HashSet<int>(_abonelikManager.Aboneliklerim().Select(a => a.BolgeId));
                    liste = liste.Where(t => aboneBolgeler.Contains(t.BolgeId)).ToList();
                }
            });
            if (!ok) return;

            dgvAlanlar.DataSource = liste;
            lblToplam.Text = liste.Count + " alan  |  toplam kapasite: " + liste.Sum(t => t.Kapasite).ToString("N0") + " kişi";
        }

        private void dgvAlanlar_SelectionChanged(object sender, EventArgs e)
        {
            ToplanmaAlani t = ArayuzYardimci.SeciliNesne<ToplanmaAlani>(dgvAlanlar);
            if (t == null || !grpDuzenle.Visible) return;
            _seciliId = t.AlanId;
            cmbBolge.SelectedItem = ((List<Bolge>)cmbBolge.DataSource).FirstOrDefault(b => b.BolgeId == t.BolgeId);
            txtAlanAdi.Text = t.AlanAdi;
            txtAdres.Text = t.Adres;
            nudKapasite.Value = Math.Min(nudKapasite.Maximum, t.Kapasite);
            txtEnlem.Text = ArayuzYardimci.OndalikYaz(t.Enlem);
            txtBoylam.Text = ArayuzYardimci.OndalikYaz(t.Boylam);
        }

        private void btnYeni_Click(object sender, EventArgs e)
        {
            _seciliId = 0;
            txtAlanAdi.Clear();
            txtAdres.Clear();
            nudKapasite.Value = 100;
            txtEnlem.Clear();
            txtBoylam.Clear();
            dgvAlanlar.ClearSelection();
            txtAlanAdi.Focus();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            bool ok = ArayuzYardimci.Calistir(this, () =>
            {
                Bolge b = cmbBolge.SelectedItem as Bolge;
                _manager.Kaydet(new ToplanmaAlani
                {
                    AlanId = _seciliId,
                    BolgeId = b == null ? 0 : b.BolgeId,
                    AlanAdi = txtAlanAdi.Text,
                    Adres = txtAdres.Text,
                    Kapasite = (int)nudKapasite.Value,
                    Enlem = ArayuzYardimci.OndalikOku(txtEnlem, "Enlem"),
                    Boylam = ArayuzYardimci.OndalikOku(txtBoylam, "Boylam")
                });
            });
            if (ok)
            {
                ArayuzYardimci.Bilgi("Toplanma alanı kaydedildi.");
                Listele();
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (_seciliId == 0) { ArayuzYardimci.Uyar("Silmek için listeden bir alan seçiniz."); return; }
            if (!ArayuzYardimci.Onayla("Seçili toplanma alanı silinsin mi?")) return;
            if (ArayuzYardimci.Calistir(this, () => _manager.Sil(_seciliId)))
            {
                btnYeni_Click(sender, e);
                Listele();
            }
        }
    }
}
