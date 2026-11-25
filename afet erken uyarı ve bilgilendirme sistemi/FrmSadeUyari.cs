using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Business;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    /// <summary>
    /// Hafta 10 - Yapay zekâ bileşeni: yönetici kurumun teknik duyurusunu yapıştırır, sistem seçili bölgeler için
    /// sade bir uyarı taslağı üretir ve doğrulama sonucunu gösterir. Geçerli taslak "Forma Aktar" ile yayın formuna gider.
    /// (Küçük bir form olduğu için kontroller kodla oluşturulur.)
    /// </summary>
    public class FrmSadeUyari : Form
    {
        private readonly SadeUyariManager _manager = new SadeUyariManager();
        private readonly List<int> _bolgeIdleri;
        private readonly string _afetTuru;

        private readonly TextBox _txtKaynak = new TextBox();
        private readonly TextBox _txtBaslik = new TextBox();
        private readonly TextBox _txtMesaj = new TextBox();
        private readonly Label _lblSeviye = new Label();
        private readonly Label _lblYontem = new Label();
        private readonly TextBox _txtNotlar = new TextBox();
        private readonly Button _btnUret = new Button();
        private readonly Button _btnAktar = new Button();

        /// <summary>Yönetici "Forma Aktar" dediyse doludur.</summary>
        public SadeUyariTaslagi Taslak { get; private set; }

        public FrmSadeUyari(List<int> bolgeIdleri, string bolgeAdlari, string afetTuru)
        {
            _bolgeIdleri = bolgeIdleri;
            _afetTuru = afetTuru;

            Text = "Yapay Zekâ ile Sade Uyarı Taslağı";
            Font = new Font("Segoe UI", 9F);
            BackColor = Color.White;
            ClientSize = new Size(860, 600);
            MinimumSize = new Size(760, 560);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            ShowInTaskbar = false;

            Label lblBilgi = new Label
            {
                Location = new Point(16, 10), Size = new Size(828, 36),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Text = "Hedef bölgeler: " + bolgeAdlari + "   |   Afet türü önerisi: " + afetTuru +
                       "\nYöntem: " + _manager.YontemAdi
            };
            Label lblKaynak = new Label { Location = new Point(16, 52), AutoSize = true, Text = "Kurumun duyuru metni (AFAD, MGM, valilik vb. - buraya yapıştırın):" };
            _txtKaynak.SetBounds(16, 72, 828, 130);
            _txtKaynak.Multiline = true;
            _txtKaynak.ScrollBars = ScrollBars.Vertical;
            _txtKaynak.MaxLength = SadeUyariManager.KaynakEnCok;
            _txtKaynak.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            _btnUret.SetBounds(16, 210, 220, 34);
            _btnUret.Text = "Sade Uyarı Taslağı Üret";
            _btnUret.BackColor = Color.FromArgb(142, 68, 173);
            _btnUret.ForeColor = Color.White;
            _btnUret.FlatStyle = FlatStyle.Flat;
            _btnUret.Click += BtnUret_Click;
            _lblYontem.SetBounds(250, 214, 594, 30);
            _lblYontem.ForeColor = Color.DimGray;
            _lblYontem.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Label lblBaslik = new Label { Location = new Point(16, 254), AutoSize = true, Text = "Taslak başlık" };
            _txtBaslik.SetBounds(16, 272, 828, 23);
            _txtBaslik.ReadOnly = true;
            _txtBaslik.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Label lblMesaj = new Label { Location = new Point(16, 302), AutoSize = true, Text = "Taslak mesaj" };
            _txtMesaj.SetBounds(16, 320, 828, 90);
            _txtMesaj.Multiline = true;
            _txtMesaj.ReadOnly = true;
            _txtMesaj.ScrollBars = ScrollBars.Vertical;
            _txtMesaj.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            _lblSeviye.SetBounds(16, 416, 828, 20);
            _lblSeviye.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            Label lblNotlar = new Label { Location = new Point(16, 440), AutoSize = true, Text = "Doğrulama sonucu" };
            _txtNotlar.SetBounds(16, 458, 828, 90);
            _txtNotlar.Multiline = true;
            _txtNotlar.ReadOnly = true;
            _txtNotlar.ScrollBars = ScrollBars.Vertical;
            _txtNotlar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            _btnAktar.SetBounds(16, 556, 220, 34);
            _btnAktar.Text = "Forma Aktar";
            _btnAktar.Enabled = false;
            _btnAktar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _btnAktar.Click += (s, e) => { DialogResult = DialogResult.OK; Close(); };
            Button btnKapat = new Button { Text = "Vazgeç", DialogResult = DialogResult.Cancel, Anchor = AnchorStyles.Bottom | AnchorStyles.Left };
            btnKapat.SetBounds(244, 556, 120, 34);
            CancelButton = btnKapat;

            Controls.AddRange(new Control[] { lblBilgi, lblKaynak, _txtKaynak, _btnUret, _lblYontem, lblBaslik, _txtBaslik,
                                              lblMesaj, _txtMesaj, _lblSeviye, lblNotlar, _txtNotlar, _btnAktar, btnKapat });
        }

        private void BtnUret_Click(object sender, EventArgs e)
        {
            SadeUyariTaslagi t = null;
            _lblYontem.Text = "Taslak hazırlanıyor...";
            _lblYontem.Refresh();
            bool ok = ArayuzYardimci.Calistir(this, () => t = _manager.TaslakUret(_txtKaynak.Text, _bolgeIdleri, _afetTuru));
            _lblYontem.Text = "";
            if (!ok) return;

            Taslak = null;
            _txtBaslik.Text = t.Baslik;
            _txtMesaj.Text = t.Mesaj;
            _lblSeviye.Text = "Önerilen: " + t.AfetTuru + " · " + t.Seviye + " - " + UyariSeviyeleri.Ad(t.Seviye) +
                              "   (üreten: " + t.Saglayici + ")";

            List<string> satirlar = new List<string>();
            if (!string.IsNullOrEmpty(t.Not)) satirlar.Add("NOT: " + t.Not);
            satirlar.Add(t.Gecerli ? "✔ Doğrulamadan geçti." : "✖ Doğrulamadan geçemedi; taslak forma aktarılamaz.");
            foreach (string h in t.Hatalar) satirlar.Add("HATA: " + h);
            foreach (string u in t.Uyarilar) satirlar.Add("UYARI: " + u);
            _txtNotlar.Text = string.Join(Environment.NewLine, satirlar);
            _txtNotlar.BackColor = t.Gecerli ? Color.FromArgb(232, 246, 236) : Color.FromArgb(250, 220, 220);

            _btnAktar.Enabled = t.Gecerli;
            if (t.Gecerli) Taslak = t;
        }
    }
}
