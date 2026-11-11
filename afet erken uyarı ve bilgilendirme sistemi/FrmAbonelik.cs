using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Business;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    /// <summary>
    /// Vatandaş: bölgelere abone olma, tercih yönetimi (kanal, minimum seviye, aktif/pasif) ve iptal (Hafta 6).
    /// </summary>
    public partial class FrmAbonelik : Form
    {
        private readonly AbonelikManager _abonelikManager = new AbonelikManager();
        private readonly BolgeManager _bolgeManager = new BolgeManager();

        public FrmAbonelik()
        {
            InitializeComponent();
        }

        private void FrmAbonelik_Load(object sender, EventArgs e)
        {
            ArayuzYardimci.IzgaraHazirla(dgvAbonelikler);
            ArayuzYardimci.Kolon(dgvAbonelikler, "BolgeAdi", "Bölge", 0);
            ArayuzYardimci.Kolon(dgvAbonelikler, "RiskSeviyesi", "Risk", 50);
            ArayuzYardimci.Kolon(dgvAbonelikler, "BildirimKanali", "Kanal", 80);
            ArayuzYardimci.Kolon(dgvAbonelikler, "MinSeviyeAdi", "Min. Seviye", 85);
            ArayuzYardimci.Kolon(dgvAbonelikler, "Durum", "Durum", 80);
            ArayuzYardimci.Kolon(dgvAbonelikler, "AboneTarihi", "Tarih", 85, "dd.MM.yyyy");

            cmbKanal.Items.AddRange(BildirimKanallari.Tumu);
            cmbKanal.SelectedIndex = 0;
            for (int s = UyariSeviyeleri.EnDusuk; s <= UyariSeviyeleri.EnYuksek; s++)
                cmbMinSeviye.Items.Add(s + " - " + UyariSeviyeleri.Ad(s));
            cmbMinSeviye.SelectedIndex = 0;

            List<Bolge> bolgeler = null;
            if (ArayuzYardimci.Calistir(this, () => bolgeler = _bolgeManager.Listele()))
            {
                cmbBolge.DisplayMember = "BolgeAdi";
                cmbBolge.ValueMember = "BolgeId";
                cmbBolge.DataSource = bolgeler;
            }
            Listele();
        }

        /// <summary>Izgarada göstermek için abonelik + okunur alanlar.</summary>
        public class AbonelikSatiri
        {
            public Abonelik Kayit { get; set; }
            public string BolgeAdi { get { return Kayit.BolgeAdi; } }
            public int RiskSeviyesi { get { return Kayit.RiskSeviyesi; } }
            public string BildirimKanali { get { return Kayit.BildirimKanali; } }
            public string MinSeviyeAdi { get { return UyariSeviyeleri.Ad(Kayit.MinSeviye); } }
            public string Durum { get { return Kayit.AktifMi ? "Aktif" : "Durduruldu"; } }
            public DateTime AboneTarihi { get { return Kayit.AboneTarihi; } }
        }

        private void Listele()
        {
            List<Abonelik> liste = null;
            if (!ArayuzYardimci.Calistir(this, () => liste = _abonelikManager.Aboneliklerim())) return;
            dgvAbonelikler.DataSource = liste.Select(a => new AbonelikSatiri { Kayit = a }).ToList();
            lblToplam.Text = liste.Count + " / " + AbonelikManager.MaksimumAbonelik + " bölgeye abonesiniz";
            SecimDegisti();
        }

        private Abonelik SeciliAbonelik()
        {
            AbonelikSatiri s = ArayuzYardimci.SeciliNesne<AbonelikSatiri>(dgvAbonelikler);
            return s == null ? null : s.Kayit;
        }

        private void SecimDegisti()
        {
            Abonelik a = SeciliAbonelik();
            btnTercihKaydet.Enabled = btnIptal.Enabled = chkAktif.Enabled = a != null;
            if (a == null) return;
            cmbKanal.SelectedItem = a.BildirimKanali;
            cmbMinSeviye.SelectedIndex = a.MinSeviye - 1;
            chkAktif.Checked = a.AktifMi;
        }

        private void dgvAbonelikler_SelectionChanged(object sender, EventArgs e)
        {
            SecimDegisti();
        }

        private void btnAboneOl_Click(object sender, EventArgs e)
        {
            Bolge b = cmbBolge.SelectedItem as Bolge;
            bool ok = ArayuzYardimci.Calistir(this, () =>
                _abonelikManager.AboneOl(b == null ? 0 : b.BolgeId,
                                         (string)cmbKanal.SelectedItem,
                                         cmbMinSeviye.SelectedIndex + 1));
            if (ok)
            {
                ArayuzYardimci.Bilgi(b.BolgeAdi + " bölgesine abone oldunuz.");
                Listele();
            }
        }

        private void btnTercihKaydet_Click(object sender, EventArgs e)
        {
            Abonelik a = SeciliAbonelik();
            if (a == null) return;
            if (ArayuzYardimci.Calistir(this, () =>
                    _abonelikManager.TercihGuncelle(a.AbonelikId, (string)cmbKanal.SelectedItem,
                                                    cmbMinSeviye.SelectedIndex + 1, chkAktif.Checked)))
            {
                ArayuzYardimci.Bilgi("Tercihleriniz kaydedildi.");
                Listele();
            }
        }

        private void btnIptal_Click(object sender, EventArgs e)
        {
            Abonelik a = SeciliAbonelik();
            if (a == null) return;
            if (!ArayuzYardimci.Onayla(a.BolgeAdi + " aboneliğiniz iptal edilsin mi?")) return;
            if (ArayuzYardimci.Calistir(this, () => _abonelikManager.AbonelikIptal(a.AbonelikId)))
                Listele();
        }
    }
}
