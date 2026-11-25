using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Business;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    /// <summary>
    /// Hafta 10 - AI günlüğü: her taslak denemesinin istemi, ham çıktısı ve doğrulama notları.
    /// Satır seçilince ayrıntı alt kutuda görünür.
    /// </summary>
    public class FrmYapayZekaGunlugu : Form
    {
        private readonly SadeUyariManager _manager = new SadeUyariManager();
        private readonly DataGridView _dgv = new DataGridView();
        private readonly TextBox _txtAyrinti = new TextBox();

        public FrmYapayZekaGunlugu()
        {
            Text = "Yapay Zekâ Günlüğü (istem ve çıktılar)";
            Font = new Font("Segoe UI", 9F);
            BackColor = Color.White;
            ClientSize = new Size(1000, 620);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            ShowInTaskbar = false;

            ArayuzYardimci.IzgaraHazirla(_dgv);
            ArayuzYardimci.Kolon(_dgv, "Tarih", "Tarih", 110, "dd.MM.yyyy HH:mm");
            ArayuzYardimci.Kolon(_dgv, "KullaniciAdi", "Kullanıcı", 80);
            ArayuzYardimci.Kolon(_dgv, "Saglayici", "Yöntem", 150);
            ArayuzYardimci.Kolon(_dgv, "HedefBolgeler", "Bölgeler", 150);
            ArayuzYardimci.Kolon(_dgv, "Durum", "Doğrulama", 80);
            ArayuzYardimci.Kolon(_dgv, "Baslik", "Taslak başlık", 0);
            _dgv.Dock = DockStyle.Fill;
            _dgv.SelectionChanged += (s, e) => AyrintiGoster();

            _txtAyrinti.Dock = DockStyle.Bottom;
            _txtAyrinti.Height = 260;
            _txtAyrinti.Multiline = true;
            _txtAyrinti.ReadOnly = true;
            _txtAyrinti.ScrollBars = ScrollBars.Both;
            _txtAyrinti.WordWrap = true;
            _txtAyrinti.Font = new Font("Consolas", 9F);

            Controls.Add(_dgv);
            Controls.Add(_txtAyrinti);
            Load += (s, e) => Listele();
        }

        public class Satir
        {
            public YapayZekaKaydi Kayit { get; set; }
            public DateTime Tarih { get { return Kayit.Tarih; } }
            public string KullaniciAdi { get { return Kayit.KullaniciAdi; } }
            public string Saglayici { get { return Kayit.Saglayici; } }
            public string HedefBolgeler { get { return Kayit.HedefBolgeler; } }
            public string Durum { get { return Kayit.Gecerli ? "Geçti" : "Geçmedi"; } }
            public string Baslik { get { return Kayit.Baslik ?? "(yok)"; } }
        }

        private void Listele()
        {
            List<YapayZekaKaydi> liste = null;
            if (!ArayuzYardimci.Calistir(this, () => liste = _manager.Gunluk(100))) return;
            _dgv.DataSource = liste.Select(k => new Satir { Kayit = k }).ToList();
            if (liste.Count == 0) _txtAyrinti.Text = "Henüz kayıt yok.";
        }

        private void AyrintiGoster()
        {
            Satir s = ArayuzYardimci.SeciliNesne<Satir>(_dgv);
            if (s == null) return;
            YapayZekaKaydi k = s.Kayit;
            string nl = Environment.NewLine;
            _txtAyrinti.Text =
                "YÖNTEM: " + k.Saglayici + (k.SureMs.HasValue ? "   SÜRE: " + k.SureMs + " ms" : "") + nl +
                "DOĞRULAMA:" + nl + (k.DogrulamaNotlari ?? "") + nl + nl +
                "TASLAK: " + (k.Baslik ?? "") + nl + (k.Mesaj ?? "") +
                (k.OnerilenSeviye.HasValue ? nl + "Seviye: " + UyariSeviyeleri.Ad(k.OnerilenSeviye.Value) : "") + nl + nl +
                "HAM ÇIKTI:" + nl + (k.HamCikti ?? "(yok)") + nl + nl +
                "İSTEM (PROMPT):" + nl + k.Istem.Replace("\n", nl);
        }
    }
}
