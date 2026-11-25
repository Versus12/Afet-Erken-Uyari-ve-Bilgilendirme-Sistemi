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
    /// Yönetici: bir uyarının e-posta gönderim kayıtları (Hafta 9 - toplu bildirim ve hata yönetimi).
    /// Başarısız gönderimler buradan yeniden denenebilir. (Küçük bir form olduğu için kontroller kodla oluşturulur.)
    /// </summary>
    public class FrmBildirimKayitlari : Form
    {
        private readonly int _uyariId;
        private readonly BildirimManager _manager = new BildirimManager();
        private readonly DataGridView _dgv = new DataGridView();
        private readonly Label _lblOzet = new Label();

        public FrmBildirimKayitlari(int uyariId, string baslik)
        {
            _uyariId = uyariId;
            Text = "E-posta Gönderim Kayıtları - " + baslik;
            Font = new Font("Segoe UI", 9F);
            BackColor = Color.White;
            ClientSize = new Size(900, 420);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            ShowInTaskbar = false;

            Panel ust = new Panel { Dock = DockStyle.Top, Height = 44, Padding = new Padding(8) };
            Button btnYenile = new Button { Text = "Yenile", Location = new Point(8, 8), Size = new Size(90, 28) };
            Button btnTekrar = new Button { Text = "Başarısızları Yeniden Dene", Location = new Point(106, 8), Size = new Size(200, 28) };
            Label lblSaglayici = new Label
            {
                AutoSize = true,
                Location = new Point(320, 14),
                ForeColor = Color.DimGray,
                Text = "Gönderim yöntemi: " + _manager.SaglayiciAdi
            };
            btnYenile.Click += (s, e) => Listele();
            btnTekrar.Click += BtnTekrar_Click;
            ust.Controls.AddRange(new Control[] { btnYenile, btnTekrar, lblSaglayici });

            _lblOzet.Dock = DockStyle.Bottom;
            _lblOzet.Height = 28;
            _lblOzet.TextAlign = ContentAlignment.MiddleLeft;
            _lblOzet.Padding = new Padding(8, 0, 0, 0);

            _dgv.Dock = DockStyle.Fill;
            ArayuzYardimci.IzgaraHazirla(_dgv);
            ArayuzYardimci.Kolon(_dgv, "KullaniciAdi", "Kullanıcı", 100);
            ArayuzYardimci.Kolon(_dgv, "Alici", "E-posta", 180);
            ArayuzYardimci.Kolon(_dgv, "DurumAdi", "Durum", 170);
            ArayuzYardimci.Kolon(_dgv, "DenemeSayisi", "Deneme", 55);
            ArayuzYardimci.Kolon(_dgv, "GonderimTarihi", "Gönderim", 105, "dd.MM.yyyy HH:mm");
            ArayuzYardimci.Kolon(_dgv, "Aciklama", "Hata / Mesaj no", 0);

            Controls.Add(_dgv);
            Controls.Add(_lblOzet);
            Controls.Add(ust);
            Load += (s, e) => Listele();
        }

        public class Satir
        {
            public Bildirim Kayit { get; set; }
            public string KullaniciAdi { get { return Kayit.KullaniciAdi; } }
            public string Alici { get { return Kayit.Alici; } }
            public string DurumAdi { get { return BildirimDurumlari.Ad(Kayit.Durum); } }
            public int DenemeSayisi { get { return Kayit.DenemeSayisi; } }
            public DateTime? GonderimTarihi { get { return Kayit.GonderimTarihi; } }
            public string Aciklama { get { return Kayit.SonHata ?? Kayit.SaglayiciMesajId ?? ""; } }
        }

        private void Listele()
        {
            List<Bildirim> liste = null;
            if (!ArayuzYardimci.Calistir(this, () => liste = _manager.Kayitlar(_uyariId))) return;
            _dgv.DataSource = liste.Select(b => new Satir { Kayit = b }).ToList();
            foreach (DataGridViewRow r in _dgv.Rows)
            {
                Satir s = (Satir)r.DataBoundItem;
                if (s.Kayit.Durum == BildirimDurumlari.Basarisiz) r.DefaultCellStyle.BackColor = Color.FromArgb(250, 200, 200);
                else if (s.Kayit.Durum == BildirimDurumlari.Bekliyor) r.DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 191);
            }
            _lblOzet.Text = liste.Count == 0
                ? "Bu uyarı için e-posta kaydı yok (E-posta kanalını seçen abone yok)."
                : liste.Count + " kayıt  |  gönderildi: " + liste.Count(b => b.Durum == BildirimDurumlari.Gonderildi) +
                  "  |  deneme modu: " + liste.Count(b => b.Durum == BildirimDurumlari.DenemeModu) +
                  "  |  başarısız: " + liste.Count(b => b.Durum == BildirimDurumlari.Basarisiz) +
                  "  |  bekliyor: " + liste.Count(b => b.Durum == BildirimDurumlari.Bekliyor);
        }

        private void BtnTekrar_Click(object sender, EventArgs e)
        {
            BildirimSonucu sonuc = null;
            if (ArayuzYardimci.Calistir(this, () => sonuc = _manager.BasarisizlariYenidenDene(_uyariId)))
            {
                ArayuzYardimci.Bilgi(sonuc.Toplam == 0 ? "Yeniden denenecek başarısız kayıt yok." : sonuc.ToString());
                Listele();
            }
        }
    }
}
