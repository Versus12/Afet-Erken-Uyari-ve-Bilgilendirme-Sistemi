using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Business;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    /// <summary>
    /// Hafta 11 - Toplanma Alanı Haritası Modülü.
    /// Kullanıcı konumunu üç yoldan verir: bölge merkezi, adres (Hafta 9 harita API'si) ya da enlem/boylam.
    /// En yakın 3 alan Haversine mesafesiyle bulunur; solda liste, sağda harita (Leaflet + OpenStreetMap, WebView2 içinde).
    /// (Kontroller kodla oluşturulur.)
    /// </summary>
    public class FrmToplanmaHaritasi : Form
    {
        private const string SanalAlanAdi = "afet-uyari.harita";

        private readonly ToplanmaAlaniManager _manager = new ToplanmaAlaniManager();
        private readonly BolgeManager _bolgeManager = new BolgeManager();
        private readonly AbonelikManager _abonelikManager = new AbonelikManager();

        private readonly ComboBox _cmbBolge = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly TextBox _txtAdres = new TextBox();
        private readonly TextBox _txtEnlem = new TextBox();
        private readonly TextBox _txtBoylam = new TextBox();
        private readonly CheckBox _chkAboneliklerim = new CheckBox { Text = "Yalnızca abone olduğum bölgeler", AutoSize = true };
        private readonly DataGridView _dgv = new DataGridView();
        private readonly Label _lblBilgi = new Label();
        private readonly WebView2 _harita = new WebView2();
        private readonly Label _lblHaritaHata = new Label();
        private bool _haritaHazir;
        private string _haritaKlasoru;

        public FrmToplanmaHaritasi()
        {
            Text = "Toplanma Alanı Haritası ve En Yakın Alan";
            Font = new Font("Segoe UI", 9F);
            BackColor = Color.White;
            ClientSize = new Size(1180, 680);
            MinimumSize = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            ShowInTaskbar = false;

            // ---------------- Üst panel: konum seçimi
            Panel ust = new Panel { Dock = DockStyle.Top, Height = 92, Padding = new Padding(8) };
            ust.Controls.Add(new Label { Text = "Bölge merkezi:", Location = new Point(10, 14), AutoSize = true });
            _cmbBolge.SetBounds(110, 10, 220, 23);
            Button btnBolge = Dugme("Bölgeden Bul", 338, 9, (s, e) => Bul(() =>
                _manager.BolgeMerkezindenEnYakinlar(((Bolge)_cmbBolge.SelectedItem).BolgeId, _chkAboneliklerim.Checked)));

            ust.Controls.Add(new Label { Text = "Adres:", Location = new Point(470, 14), AutoSize = true });
            _txtAdres.SetBounds(520, 10, 300, 23);
            Button btnAdres = Dugme("Adresten Bul", 828, 9, (s, e) => Bul(() =>
                _manager.AdrestenEnYakinlar(_txtAdres.Text, _chkAboneliklerim.Checked)));

            ust.Controls.Add(new Label { Text = "Enlem / Boylam:", Location = new Point(10, 52), AutoSize = true });
            _txtEnlem.SetBounds(110, 48, 105, 23);
            _txtBoylam.SetBounds(225, 48, 105, 23);
            Button btnKoordinat = Dugme("Koordinattan Bul", 338, 47, (s, e) => Bul(() =>
                _manager.EnYakinlar(ArayuzYardimci.OndalikOku(_txtEnlem, "Enlem") ?? throw new IsKuraliHatasi("Enlem giriniz."),
                                    ArayuzYardimci.OndalikOku(_txtBoylam, "Boylam") ?? throw new IsKuraliHatasi("Boylam giriniz."),
                                    "Girilen koordinat", _chkAboneliklerim.Checked)));
            _chkAboneliklerim.Location = new Point(520, 51);
            ust.Controls.AddRange(new Control[] { _cmbBolge, btnBolge, _txtAdres, btnAdres, _txtEnlem, _txtBoylam, btnKoordinat, _chkAboneliklerim });
            AcceptButton = btnAdres;

            // ---------------- Sol panel: en yakın alanlar
            Panel sol = new Panel { Dock = DockStyle.Left, Width = 420, Padding = new Padding(8, 0, 4, 8) };
            Label baslik = new Label { Text = "En yakın toplanma alanları", Dock = DockStyle.Top, Height = 26, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
            ArayuzYardimci.IzgaraHazirla(_dgv);
            ArayuzYardimci.Kolon(_dgv, "Sira", "#", 28);
            ArayuzYardimci.Kolon(_dgv, "AlanAdi", "Alan", 0);
            ArayuzYardimci.Kolon(_dgv, "MesafeMetni", "Mesafe", 70);
            ArayuzYardimci.Kolon(_dgv, "YurumeMetni", "Yürüyüş", 70);
            ArayuzYardimci.Kolon(_dgv, "Kapasite", "Kapasite", 65, "N0");
            _dgv.Dock = DockStyle.Fill;
            _lblBilgi.Dock = DockStyle.Bottom;
            _lblBilgi.Height = 110;
            _lblBilgi.Padding = new Padding(2, 6, 2, 0);
            _lblBilgi.Text = "Konumunuzu seçip \"Bul\" düğmesine basın. Mesafeler kuş uçuşudur (Haversine); yürüme süresi 4,5 km/sa ile tahmini hesaplanır.";
            sol.Controls.Add(_dgv);
            sol.Controls.Add(_lblBilgi);
            sol.Controls.Add(baslik);

            // ---------------- Harita
            _harita.Dock = DockStyle.Fill;
            _lblHaritaHata.Dock = DockStyle.Fill;
            _lblHaritaHata.TextAlign = ContentAlignment.MiddleCenter;
            _lblHaritaHata.Visible = false;

            Controls.Add(_harita);
            Controls.Add(_lblHaritaHata);
            Controls.Add(sol);
            Controls.Add(ust);
            Load += FrmToplanmaHaritasi_Load;
        }

        public class Satir
        {
            public int Sira { get; set; }
            public YakinAlan Kayit { get; set; }
            public string AlanAdi { get { return Kayit.Alan.AlanAdi + " (" + Kayit.Alan.BolgeAdi + ")"; } }
            public string MesafeMetni { get { return Kayit.MesafeKm < 1 ? (Kayit.MesafeKm * 1000).ToString("0") + " m" : Kayit.MesafeKm.ToString("0.0") + " km"; } }
            public string YurumeMetni { get { return Kayit.YurumeDakika > 180 ? "-" : "~" + Kayit.YurumeDakika + " dk"; } }
            public int Kapasite { get { return Kayit.Alan.Kapasite; } }
        }

        private Button Dugme(string metin, int x, int y, EventHandler tikla)
        {
            Button b = new Button { Text = metin, Location = new Point(x, y), Size = new Size(124, 26) };
            b.Click += tikla;
            return b;
        }

        private async void FrmToplanmaHaritasi_Load(object sender, EventArgs e)
        {
            List<Bolge> bolgeler = null;
            if (ArayuzYardimci.Calistir(this, () => bolgeler = _bolgeManager.Listele()))
            {
                _cmbBolge.DisplayMember = "BolgeAdi";
                _cmbBolge.DataSource = bolgeler;
                // Varsayılan: kullanıcının abone olduğu ilk bölge
                List<Abonelik> abonelikler = null;
                if (ArayuzYardimci.Calistir(this, () => abonelikler = _abonelikManager.Aboneliklerim()) && abonelikler.Count > 0)
                    _cmbBolge.SelectedItem = bolgeler.FirstOrDefault(b => b.BolgeId == abonelikler[0].BolgeId);
            }

            try
            {
                // Tarayıcı verileri program klasörüne değil kullanıcının AppData klasörüne yazılır.
                string veri = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AfetErkenUyari", "WebView2");
                _haritaKlasoru = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AfetErkenUyari", "Harita");
                Directory.CreateDirectory(_haritaKlasoru);
                CoreWebView2Environment ortam = await CoreWebView2Environment.CreateAsync(null, veri);
                await _harita.EnsureCoreWebView2Async(ortam);
                // Sayfa sanal bir adresten açılır; OpenStreetMap harita karoları geçerli bir kaynak (Referer) ister.
                _harita.CoreWebView2.SetVirtualHostNameToFolderMapping(SanalAlanAdi, _haritaKlasoru, CoreWebView2HostResourceAccessKind.Allow);
                _haritaHazir = true;
            }
            catch (Exception ex)
            {
                ArayuzYardimci.HataKaydet(ex);
                _harita.Visible = false;
                _lblHaritaHata.Visible = true;
                _lblHaritaHata.Text = "Harita bileşeni (Microsoft Edge WebView2) açılamadı.\nEn yakın alan listesi yine çalışır.\n\nAyrıntı: " + ex.Message;
            }

            // İlk açılışta tüm alanlar haritada; abone olunan bölge varsa oradan en yakınlar
            if (_cmbBolge.SelectedItem != null)
                Bul(() => _manager.BolgeMerkezindenEnYakinlar(((Bolge)_cmbBolge.SelectedItem).BolgeId, false));
            else
                HaritayiGoster(null);
        }

        private void Bul(Func<EnYakinSonucu> islem)
        {
            EnYakinSonucu sonuc = null;
            if (!ArayuzYardimci.Calistir(this, () => sonuc = islem())) return;

            int i = 0;
            _dgv.DataSource = sonuc.Alanlar.Select(y => new Satir { Sira = ++i, Kayit = y }).ToList();
            if (_dgv.Rows.Count > 0) _dgv.Rows[0].DefaultCellStyle.BackColor = Color.FromArgb(253, 223, 190);

            string bilgi = "Konum: " + sonuc.KonumAciklamasi + "\n(" + ArayuzYardimci.OndalikYaz(sonuc.Enlem) + ", " + ArayuzYardimci.OndalikYaz(sonuc.Boylam) + ")";
            if (sonuc.Alanlar.Count > 0)
                bilgi += "\nEn yakın: " + sonuc.Alanlar[0].Alan.AlanAdi + " — " + new Satir { Kayit = sonuc.Alanlar[0] }.MesafeMetni;
            if (sonuc.KonumsuzAlanSayisi > 0)
                bilgi += "\nNot: koordinatı olmayan " + sonuc.KonumsuzAlanSayisi + " alan hesaba katılmadı.";
            if (!string.IsNullOrEmpty(sonuc.Uyari)) bilgi += "\n⚠ " + sonuc.Uyari;
            _lblBilgi.Text = bilgi;
            _lblBilgi.ForeColor = string.IsNullOrEmpty(sonuc.Uyari) ? Color.Black : Color.Firebrick;

            _txtEnlem.Text = ArayuzYardimci.OndalikYaz(sonuc.Enlem);
            _txtBoylam.Text = ArayuzYardimci.OndalikYaz(sonuc.Boylam);
            HaritayiGoster(sonuc);
        }

        private void HaritayiGoster(EnYakinSonucu sonuc)
        {
            if (!_haritaHazir) return;
            List<ToplanmaAlani> alanlar = null;
            if (!ArayuzYardimci.Calistir(this, () => alanlar = _manager.Listele(null))) return;
            if (sonuc != null && _chkAboneliklerim.Checked)
            {
                HashSet<int> gosterilen = new HashSet<int>(sonuc.Alanlar.Select(y => y.Alan.BolgeId));
                alanlar = alanlar.Where(a => gosterilen.Contains(a.BolgeId)).ToList();
            }
            string html = HaritaSayfasi.Olustur(alanlar, sonuc == null ? (decimal?)null : sonuc.Enlem,
                                                sonuc == null ? (decimal?)null : sonuc.Boylam,
                                                sonuc == null ? null : sonuc.Alanlar, sonuc == null ? null : sonuc.KonumAciklamasi);
            File.WriteAllText(Path.Combine(_haritaKlasoru, "harita.html"), html, System.Text.Encoding.UTF8);
            _harita.CoreWebView2.Navigate("https://" + SanalAlanAdi + "/harita.html?t=" + DateTime.Now.Ticks);
        }
    }
}
