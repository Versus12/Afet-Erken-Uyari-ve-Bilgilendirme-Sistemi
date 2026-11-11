using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Business;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    /// <summary>
    /// Ana menü / panel. Tüm modüller buradan açılır (Hafta 8 - uçtan uca entegrasyon).
    /// - Vatandaş: abonelik, uyarılarım (bildirim kutusu), rehber, toplanma alanları
    /// - Yönetici: ek olarak bölge yönetimi ve uyarı yayını
    /// Uygulama içi bildirim: zamanlayıcı her dakika uyarıları yeniler, yeni uyarı gelirse sesli uyarır.
    /// </summary>
    public partial class FrmAnaMenu : Form
    {
        private readonly UyariManager _uyariManager = new UyariManager();
        private readonly HashSet<int> _gorulenUyarilar = new HashSet<int>();
        private bool _ilkYukleme = true;
        private bool _zamanlayiciCalisiyor;

        /// <summary>Kullanıcı "Çıkış Yap" dediyse veya oturum düştüyse true (giriş ekranına dönülür).</summary>
        public bool GiriseDon { get; private set; }

        public FrmAnaMenu()
        {
            InitializeComponent();
        }

        private void FrmAnaMenu_Load(object sender, EventArgs e)
        {
            Kullanici k = Oturum.AktifKullanici;
            if (k == null)
            {
                GiriseDon = true;
                Close();
                return;
            }

            lblHosgeldin.Text = "Hoş geldiniz, " + k.KullaniciAdi;
            lblRol.Text = k.YoneticiMi ? "Rol: Yönetici" : "Rol: Vatandaş";

            // Arayüzde rol bazlı menü (asıl yetki kontrolü iş katmanında da yapılır)
            lblYonetim.Visible = k.YoneticiMi;
            btnBolgeYonetimi.Visible = k.YoneticiMi;
            btnUyariYayini.Visible = k.YoneticiMi;

            ArayuzYardimci.IzgaraHazirla(dgvUyarilar);
            ArayuzYardimci.Kolon(dgvUyarilar, "SeviyeAdi", "Seviye", 80);
            ArayuzYardimci.Kolon(dgvUyarilar, "AfetTuru", "Afet Türü", 85);
            ArayuzYardimci.Kolon(dgvUyarilar, "Baslik", "Başlık", 0);
            ArayuzYardimci.Kolon(dgvUyarilar, "HedefBolgeler", "Bölgeler", 200);
            ArayuzYardimci.Kolon(dgvUyarilar, "YayinTarihi", "Yayın", 115, "dd.MM.yyyy HH:mm");

            UyarilariYukle();
        }

        private void UyarilariYukle(bool arkaPlan = false)
        {
            List<Uyari> uyarilar = null;
            if (arkaPlan)
            {
                // Arka plan yenilemesinde hata penceresi açılmaz; durum çubuğuna yazılır,
                // böylece bağlantı koptuğunda her dakika mesaj kutusu çıkmaz.
                try
                {
                    uyarilar = _uyariManager.Uyarilarim(true);
                }
                catch (Exception ex)
                {
                    ArayuzYardimci.HataKaydet(ex);
                    lblDurum.Text = "Uyarılar yenilenemedi (" + DateTime.Now.ToString("HH:mm") + "): " + ex.Message;
                    return;
                }
            }
            else if (!ArayuzYardimci.Calistir(this, () => uyarilar = _uyariManager.Uyarilarim()))
            {
                return;
            }

            int yeniSayisi = 0;
            foreach (Uyari u in uyarilar)
            {
                if (_gorulenUyarilar.Add(u.UyariId) && !_ilkYukleme) yeniSayisi++;
            }

            dgvUyarilar.DataSource = uyarilar;
            lblUyariBaslik.Text = "Bölgelerimdeki Güncel Uyarılar (" + uyarilar.Count + ")";
            lblDurum.Text = "Son güncelleme: " + DateTime.Now.ToString("HH:mm:ss");

            if (uyarilar.Count == 0)
                txtDetay.Text = "Abone olduğunuz bölgelerde şu an yürürlükte uyarı yok." + Environment.NewLine +
                                "Bölge eklemek için soldaki \"Aboneliklerim\" menüsünü kullanınız.";

            if (yeniSayisi > 0)
            {
                SystemSounds.Exclamation.Play();
                lblDurum.Text = "⚠ " + yeniSayisi + " yeni uyarı geldi! (" + DateTime.Now.ToString("HH:mm") + ")";
                OneGetir();
            }
            _ilkYukleme = false;
        }

        private void OneGetir()
        {
            if (WindowState == FormWindowState.Minimized) WindowState = FormWindowState.Normal;
            Activate();
        }

        private void dgvUyarilar_SelectionChanged(object sender, EventArgs e)
        {
            Uyari u = ArayuzYardimci.SeciliNesne<Uyari>(dgvUyarilar);
            if (u == null) return;
            txtDetay.Text =
                "[" + u.SeviyeAdi.ToUpper() + "] " + u.Baslik + Environment.NewLine +
                "Afet türü: " + u.AfetTuru + "   |   Bölgeler: " + u.HedefBolgeler + Environment.NewLine +
                "Yayın: " + u.YayinTarihi.ToString("dd.MM.yyyy HH:mm") +
                (u.BitisTarihi.HasValue ? "   |   Geçerlilik: " + u.BitisTarihi.Value.ToString("dd.MM.yyyy HH:mm") : "") +
                Environment.NewLine + Environment.NewLine + u.Mesaj;
        }

        private void dgvUyarilar_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            Uyari u = dgvUyarilar.Rows[e.RowIndex].DataBoundItem as Uyari;
            if (u != null) dgvUyarilar.Rows[e.RowIndex].DefaultCellStyle.BackColor = ArayuzYardimci.SeviyeRengi(u.Seviye);
        }

        private void tmrYenile_Tick(object sender, EventArgs e)
        {
            if (_zamanlayiciCalisiyor) return; // açık bir mesaj kutusu varken tekrar girmeyi engelle
            _zamanlayiciCalisiyor = true;
            try
            {
                if (!Oturum.OturumAcikMi)
                {
                    tmrYenile.Stop();
                    ArayuzYardimci.Uyar("Uzun süre işlem yapılmadığı için oturumunuz kapandı. Lütfen tekrar giriş yapınız.");
                    GiriseDon = true;
                    Close();
                    return;
                }
                // Otomatik yenileme oturum süresini uzatmaz (arkaPlan = true).
                UyarilariYukle(true);
            }
            finally
            {
                _zamanlayiciCalisiyor = false;
            }
        }

        // ------------------------------------------------------------ Menü butonları

        private void ModulAc(Form form)
        {
            tmrYenile.Stop(); // modül açıkken arka plan yenilemesi yapılmaz
            using (form)
            {
                form.ShowDialog(this);
            }
            tmrYenile.Start();
            if (!Oturum.OturumAcikMi)
            {
                GiriseDon = true;
                Close();
                return;
            }
            UyarilariYukle();
        }

        private void btnAboneliklerim_Click(object sender, EventArgs e) { ModulAc(new FrmAbonelik()); }
        private void btnHazirlikRehberi_Click(object sender, EventArgs e) { ModulAc(new FrmHazirlikRehberi()); }
        private void btnToplanmaAlanlari_Click(object sender, EventArgs e) { ModulAc(new FrmToplanmaAlanlari()); }
        private void btnBolgeYonetimi_Click(object sender, EventArgs e) { ModulAc(new FrmBolgeYonetimi()); }
        private void btnUyariYayini_Click(object sender, EventArgs e) { ModulAc(new FrmUyariYayini()); }
        private void btnYenile_Click(object sender, EventArgs e) { UyarilariYukle(); }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            if (!ArayuzYardimci.Onayla("Oturumu kapatmak istiyor musunuz?")) return;
            GiriseDon = true;
            Close();
        }

        private void FrmAnaMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            tmrYenile.Stop();
            // Form, oturum düştüğü için kapandıysa (ör. "Yenile" sırasında süre dolmuşsa)
            // uygulamadan çıkmak yerine giriş ekranına dönülür.
            if (!Oturum.OturumAcikMi) GiriseDon = true;
        }
    }
}
