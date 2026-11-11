using System;
using System.Collections.Generic;
using System.Windows.Forms;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Business;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    /// <summary>Yönetici: bir veya birden çok bölgeye uyarı yayınlama ve yayından kaldırma (Hafta 7).</summary>
    public partial class FrmUyariYayini : Form
    {
        private readonly UyariManager _uyariManager = new UyariManager();
        private readonly BolgeManager _bolgeManager = new BolgeManager();

        public FrmUyariYayini()
        {
            InitializeComponent();
        }

        private void FrmUyariYayini_Load(object sender, EventArgs e)
        {
            cmbAfetTuru.Items.AddRange(AfetTurleri.Tumu);
            cmbAfetTuru.SelectedIndex = 0;
            for (int s = UyariSeviyeleri.EnDusuk; s <= UyariSeviyeleri.EnYuksek; s++)
                cmbSeviye.Items.Add(s + " - " + UyariSeviyeleri.Ad(s));
            cmbSeviye.SelectedIndex = 1;

            dtpBitis.Value = DateTime.Now.AddDays(1);

            List<Bolge> bolgeler = null;
            if (ArayuzYardimci.Calistir(this, () => bolgeler = _bolgeManager.Listele()))
            {
                clbBolgeler.DisplayMember = "BolgeAdi";
                foreach (Bolge b in bolgeler) clbBolgeler.Items.Add(b);
            }

            ArayuzYardimci.IzgaraHazirla(dgvUyarilar);
            ArayuzYardimci.Kolon(dgvUyarilar, "UyariId", "No", 35);
            ArayuzYardimci.Kolon(dgvUyarilar, "SeviyeAdi", "Seviye", 60);
            ArayuzYardimci.Kolon(dgvUyarilar, "AfetTuru", "Tür", 60);
            ArayuzYardimci.Kolon(dgvUyarilar, "Baslik", "Başlık", 0);
            ArayuzYardimci.Kolon(dgvUyarilar, "HedefBolgeler", "Bölgeler", 140);
            ArayuzYardimci.Kolon(dgvUyarilar, "YayinTarihi", "Yayın", 100, "dd.MM.yyyy HH:mm");
            ArayuzYardimci.Kolon(dgvUyarilar, "BitisTarihi", "Bitiş", 100, "dd.MM.yyyy HH:mm");
            ArayuzYardimci.Kolon(dgvUyarilar, "Durum", "Durum", 75);
            dgvUyarilar.Columns["colBaslik"].MinimumWidth = 140;
            dgvUyarilar.ShowCellToolTips = true;
            Listele();
        }

        public class UyariSatiri
        {
            public Uyari Kayit { get; set; }
            public int UyariId { get { return Kayit.UyariId; } }
            public string SeviyeAdi { get { return Kayit.SeviyeAdi; } }
            public string AfetTuru { get { return Kayit.AfetTuru; } }
            public string Baslik { get { return Kayit.Baslik; } }
            public string HedefBolgeler { get { return Kayit.HedefBolgeler; } }
            public DateTime YayinTarihi { get { return Kayit.YayinTarihi; } }
            public DateTime? BitisTarihi { get { return Kayit.BitisTarihi; } }
            public string Durum
            {
                get
                {
                    if (!Kayit.AktifMi) return "Kaldırıldı";
                    return Kayit.GecerliMi(DateTime.Now) ? "Yayında" : "Süresi doldu";
                }
            }
        }

        private void Listele()
        {
            List<Uyari> liste = null;
            if (!ArayuzYardimci.Calistir(this, () => liste = _uyariManager.TumUyarilar())) return;
            List<UyariSatiri> satirlar = new List<UyariSatiri>();
            foreach (Uyari u in liste) satirlar.Add(new UyariSatiri { Kayit = u });
            dgvUyarilar.DataSource = satirlar;
        }

        private void chkTumBolgeler_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < clbBolgeler.Items.Count; i++)
                clbBolgeler.SetItemChecked(i, chkTumBolgeler.Checked);
        }

        private void chkBitis_CheckedChanged(object sender, EventArgs e)
        {
            dtpBitis.Enabled = chkBitis.Checked;
        }

        private void txtMesaj_TextChanged(object sender, EventArgs e)
        {
            lblKarakter.Text = txtMesaj.Text.Length + " / 2000";
        }

        private void btnYayinla_Click(object sender, EventArgs e)
        {
            Uyari u = new Uyari
            {
                Baslik = txtBaslik.Text,
                Mesaj = txtMesaj.Text,
                AfetTuru = (string)cmbAfetTuru.SelectedItem,
                Seviye = cmbSeviye.SelectedIndex + 1,
                YayinTarihi = DateTime.Now,
                BitisTarihi = chkBitis.Checked ? dtpBitis.Value : (DateTime?)null
            };
            foreach (object o in clbBolgeler.CheckedItems) u.BolgeIdleri.Add(((Bolge)o).BolgeId);

            string ozet = "\"" + u.Baslik.Trim() + "\" uyarısı " + u.BolgeIdleri.Count +
                          " bölgeye " + UyariSeviyeleri.Ad(u.Seviye).ToUpper() + " seviyesinde yayınlanacak. Onaylıyor musunuz?";
            if (u.BolgeIdleri.Count > 0 && !ArayuzYardimci.Onayla(ozet)) return;

            int alici = 0;
            if (ArayuzYardimci.Calistir(this, () => _uyariManager.Yayinla(u, out alici)))
            {
                ArayuzYardimci.Bilgi("Uyarı yayınlandı. Ulaşacağı abone sayısı: " + alici);
                Temizle();
                Listele();
            }
        }

        private void Temizle()
        {
            txtBaslik.Clear();
            txtMesaj.Clear();
            chkTumBolgeler.Checked = false;
            for (int i = 0; i < clbBolgeler.Items.Count; i++) clbBolgeler.SetItemChecked(i, false);
            dtpBitis.Value = DateTime.Now.AddDays(1);
        }

        private void btnYayindanKaldir_Click(object sender, EventArgs e)
        {
            UyariSatiri s = ArayuzYardimci.SeciliNesne<UyariSatiri>(dgvUyarilar);
            if (s == null) { ArayuzYardimci.Uyar("Listeden bir uyarı seçiniz."); return; }
            if (!ArayuzYardimci.Onayla("\"" + s.Baslik + "\" yayından kaldırılsın mı?")) return;
            if (ArayuzYardimci.Calistir(this, () => _uyariManager.YayindanKaldir(s.UyariId)))
                Listele();
        }

        private void dgvUyarilar_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            UyariSatiri s = dgvUyarilar.Rows[e.RowIndex].DataBoundItem as UyariSatiri;
            if (s == null) return;
            dgvUyarilar.Rows[e.RowIndex].DefaultCellStyle.BackColor = s.Durum == "Yayında"
                ? ArayuzYardimci.SeviyeRengi(s.Kayit.Seviye)
                : System.Drawing.Color.Gainsboro;
        }
    }
}
