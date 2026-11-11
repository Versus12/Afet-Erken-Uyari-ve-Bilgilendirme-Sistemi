using System;
using System.Collections.Generic;
using System.Windows.Forms;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Business;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    /// <summary>
    /// Hazırlık rehberi (Hafta 7). Herkes afet türüne göre okuyabilir;
    /// yönetici için düzenleme paneli görünür.
    /// </summary>
    public partial class FrmHazirlikRehberi : Form
    {
        private const string TumTurler = "(Tüm afet türleri)";
        private readonly RehberManager _manager = new RehberManager();
        private int _seciliId;

        public FrmHazirlikRehberi()
        {
            InitializeComponent();
        }

        private void FrmHazirlikRehberi_Load(object sender, EventArgs e)
        {
            bool yonetici = Oturum.AktifKullanici != null && Oturum.AktifKullanici.YoneticiMi;
            grpDuzenle.Visible = yonetici;

            cmbFiltre.Items.Add(TumTurler);
            cmbFiltre.Items.AddRange(AfetTurleri.Tumu);
            cmbAfetTuru.Items.AddRange(AfetTurleri.Tumu);
            cmbAsama.Items.AddRange(RehberAsamalari.Tumu);
            cmbAfetTuru.SelectedIndex = 0;
            cmbAsama.SelectedIndex = 0;

            ArayuzYardimci.IzgaraHazirla(dgvRehber);
            ArayuzYardimci.Kolon(dgvRehber, "AfetTuru", "Afet", 70);
            ArayuzYardimci.Kolon(dgvRehber, "Asama", "Aşama", 75);
            ArayuzYardimci.Kolon(dgvRehber, "Baslik", "Başlık", 0);

            cmbFiltre.SelectedIndex = 0; // Listele() tetiklenir
        }

        private void cmbFiltre_SelectedIndexChanged(object sender, EventArgs e)
        {
            Listele();
        }

        private void Listele()
        {
            string tur = cmbFiltre.SelectedIndex <= 0 ? null : (string)cmbFiltre.SelectedItem;
            List<HazirlikRehberi> liste = null;
            if (ArayuzYardimci.Calistir(this, () => liste = _manager.Listele(tur)))
            {
                dgvRehber.DataSource = liste;
                if (liste.Count == 0) txtGoster.Text = "Bu afet türü için henüz rehber içeriği eklenmemiş.";
            }
        }

        private void dgvRehber_SelectionChanged(object sender, EventArgs e)
        {
            HazirlikRehberi r = ArayuzYardimci.SeciliNesne<HazirlikRehberi>(dgvRehber);
            if (r == null) return;

            txtGoster.Text = r.AfetTuru.ToUpper() + " - " + r.Asama + Environment.NewLine +
                             r.Baslik + Environment.NewLine + Environment.NewLine + r.Icerik;

            if (grpDuzenle.Visible)
            {
                _seciliId = r.RehberId;
                cmbAfetTuru.SelectedItem = r.AfetTuru;
                cmbAsama.SelectedItem = r.Asama;
                txtBaslik.Text = r.Baslik;
                txtIcerik.Text = r.Icerik;
                nudSira.Value = Math.Max(nudSira.Minimum, Math.Min(nudSira.Maximum, r.Sira));
            }
        }

        private void btnYeni_Click(object sender, EventArgs e)
        {
            _seciliId = 0;
            txtBaslik.Clear();
            txtIcerik.Clear();
            nudSira.Value = 1;
            dgvRehber.ClearSelection();
            txtBaslik.Focus();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            HazirlikRehberi r = new HazirlikRehberi
            {
                RehberId = _seciliId,
                AfetTuru = (string)cmbAfetTuru.SelectedItem,
                Asama = (string)cmbAsama.SelectedItem,
                Baslik = txtBaslik.Text,
                Icerik = txtIcerik.Text,
                Sira = (int)nudSira.Value
            };
            if (ArayuzYardimci.Calistir(this, () => _manager.Kaydet(r)))
            {
                ArayuzYardimci.Bilgi("Rehber kaydedildi.");
                Listele();
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (_seciliId == 0) { ArayuzYardimci.Uyar("Silmek için listeden bir kayıt seçiniz."); return; }
            if (!ArayuzYardimci.Onayla("Seçili rehber kaydı silinsin mi?")) return;
            if (ArayuzYardimci.Calistir(this, () => _manager.Sil(_seciliId)))
            {
                btnYeni_Click(sender, e);
                Listele();
            }
        }
    }
}
