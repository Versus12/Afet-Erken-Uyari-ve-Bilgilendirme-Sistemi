namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    partial class FrmAnaMenu
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlUst = new System.Windows.Forms.Panel();
            this.btnCikis = new System.Windows.Forms.Button();
            this.lblRol = new System.Windows.Forms.Label();
            this.lblHosgeldin = new System.Windows.Forms.Label();
            this.lblUygulamaAdi = new System.Windows.Forms.Label();
            this.pnlMenu = new System.Windows.Forms.Panel();
            this.btnUyariYayini = new System.Windows.Forms.Button();
            this.btnBolgeYonetimi = new System.Windows.Forms.Button();
            this.lblYonetim = new System.Windows.Forms.Label();
            this.btnToplanmaAlanlari = new System.Windows.Forms.Button();
            this.btnHazirlikRehberi = new System.Windows.Forms.Button();
            this.btnAboneliklerim = new System.Windows.Forms.Button();
            this.lblVatandas = new System.Windows.Forms.Label();
            this.pnlIcerik = new System.Windows.Forms.Panel();
            this.dgvUyarilar = new System.Windows.Forms.DataGridView();
            this.txtDetay = new System.Windows.Forms.TextBox();
            this.pnlUyariBaslik = new System.Windows.Forms.Panel();
            this.btnYenile = new System.Windows.Forms.Button();
            this.lblUyariBaslik = new System.Windows.Forms.Label();
            this.lblDurum = new System.Windows.Forms.Label();
            this.tmrYenile = new System.Windows.Forms.Timer(this.components);
            this.pnlUst.SuspendLayout();
            this.pnlMenu.SuspendLayout();
            this.pnlIcerik.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUyarilar)).BeginInit();
            this.pnlUyariBaslik.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlUst
            //
            this.pnlUst.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.pnlUst.Controls.Add(this.btnCikis);
            this.pnlUst.Controls.Add(this.lblRol);
            this.pnlUst.Controls.Add(this.lblHosgeldin);
            this.pnlUst.Controls.Add(this.lblUygulamaAdi);
            this.pnlUst.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlUst.Location = new System.Drawing.Point(0, 0);
            this.pnlUst.Name = "pnlUst";
            this.pnlUst.Size = new System.Drawing.Size(984, 64);
            this.pnlUst.TabIndex = 0;
            //
            // btnCikis
            //
            this.btnCikis.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCikis.BackColor = System.Drawing.Color.White;
            this.btnCikis.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCikis.Location = new System.Drawing.Point(868, 16);
            this.btnCikis.Name = "btnCikis";
            this.btnCikis.Size = new System.Drawing.Size(100, 32);
            this.btnCikis.TabIndex = 3;
            this.btnCikis.Text = "Çıkış Yap";
            this.btnCikis.UseVisualStyleBackColor = false;
            this.btnCikis.Click += new System.EventHandler(this.btnCikis_Click);
            //
            // lblRol
            //
            this.lblRol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRol.ForeColor = System.Drawing.Color.White;
            this.lblRol.Location = new System.Drawing.Point(592, 34);
            this.lblRol.Name = "lblRol";
            this.lblRol.Size = new System.Drawing.Size(260, 18);
            this.lblRol.TabIndex = 2;
            this.lblRol.Text = "Rol";
            this.lblRol.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblHosgeldin
            //
            this.lblHosgeldin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHosgeldin.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblHosgeldin.ForeColor = System.Drawing.Color.White;
            this.lblHosgeldin.Location = new System.Drawing.Point(592, 12);
            this.lblHosgeldin.Name = "lblHosgeldin";
            this.lblHosgeldin.Size = new System.Drawing.Size(260, 20);
            this.lblHosgeldin.TabIndex = 1;
            this.lblHosgeldin.Text = "Hoş geldiniz";
            this.lblHosgeldin.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblUygulamaAdi
            //
            this.lblUygulamaAdi.AutoSize = true;
            this.lblUygulamaAdi.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold);
            this.lblUygulamaAdi.ForeColor = System.Drawing.Color.White;
            this.lblUygulamaAdi.Location = new System.Drawing.Point(16, 16);
            this.lblUygulamaAdi.Name = "lblUygulamaAdi";
            this.lblUygulamaAdi.Size = new System.Drawing.Size(420, 30);
            this.lblUygulamaAdi.TabIndex = 0;
            this.lblUygulamaAdi.Text = "Afet Erken Uyarı ve Bilgilendirme";
            //
            // pnlMenu
            //
            this.pnlMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.pnlMenu.Controls.Add(this.btnUyariYayini);
            this.pnlMenu.Controls.Add(this.btnBolgeYonetimi);
            this.pnlMenu.Controls.Add(this.lblYonetim);
            this.pnlMenu.Controls.Add(this.btnToplanmaAlanlari);
            this.pnlMenu.Controls.Add(this.btnHazirlikRehberi);
            this.pnlMenu.Controls.Add(this.btnAboneliklerim);
            this.pnlMenu.Controls.Add(this.lblVatandas);
            this.pnlMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlMenu.Location = new System.Drawing.Point(0, 64);
            this.pnlMenu.Name = "pnlMenu";
            this.pnlMenu.Size = new System.Drawing.Size(220, 473);
            this.pnlMenu.TabIndex = 1;
            //
            // lblVatandas
            //
            this.lblVatandas.ForeColor = System.Drawing.Color.Silver;
            this.lblVatandas.Location = new System.Drawing.Point(14, 14);
            this.lblVatandas.Name = "lblVatandas";
            this.lblVatandas.Size = new System.Drawing.Size(190, 18);
            this.lblVatandas.TabIndex = 0;
            this.lblVatandas.Text = "MENÜ";
            //
            // btnAboneliklerim
            //
            this.btnAboneliklerim.FlatAppearance.BorderSize = 0;
            this.btnAboneliklerim.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAboneliklerim.ForeColor = System.Drawing.Color.White;
            this.btnAboneliklerim.Location = new System.Drawing.Point(10, 38);
            this.btnAboneliklerim.Name = "btnAboneliklerim";
            this.btnAboneliklerim.Size = new System.Drawing.Size(200, 40);
            this.btnAboneliklerim.TabIndex = 1;
            this.btnAboneliklerim.Text = "  Bölge Aboneliklerim";
            this.btnAboneliklerim.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAboneliklerim.UseVisualStyleBackColor = true;
            this.btnAboneliklerim.Click += new System.EventHandler(this.btnAboneliklerim_Click);
            //
            // btnHazirlikRehberi
            //
            this.btnHazirlikRehberi.FlatAppearance.BorderSize = 0;
            this.btnHazirlikRehberi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHazirlikRehberi.ForeColor = System.Drawing.Color.White;
            this.btnHazirlikRehberi.Location = new System.Drawing.Point(10, 82);
            this.btnHazirlikRehberi.Name = "btnHazirlikRehberi";
            this.btnHazirlikRehberi.Size = new System.Drawing.Size(200, 40);
            this.btnHazirlikRehberi.TabIndex = 2;
            this.btnHazirlikRehberi.Text = "  Hazırlık Rehberi";
            this.btnHazirlikRehberi.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHazirlikRehberi.UseVisualStyleBackColor = true;
            this.btnHazirlikRehberi.Click += new System.EventHandler(this.btnHazirlikRehberi_Click);
            //
            // btnToplanmaAlanlari
            //
            this.btnToplanmaAlanlari.FlatAppearance.BorderSize = 0;
            this.btnToplanmaAlanlari.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToplanmaAlanlari.ForeColor = System.Drawing.Color.White;
            this.btnToplanmaAlanlari.Location = new System.Drawing.Point(10, 126);
            this.btnToplanmaAlanlari.Name = "btnToplanmaAlanlari";
            this.btnToplanmaAlanlari.Size = new System.Drawing.Size(200, 40);
            this.btnToplanmaAlanlari.TabIndex = 3;
            this.btnToplanmaAlanlari.Text = "  Toplanma Alanları";
            this.btnToplanmaAlanlari.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnToplanmaAlanlari.UseVisualStyleBackColor = true;
            this.btnToplanmaAlanlari.Click += new System.EventHandler(this.btnToplanmaAlanlari_Click);
            //
            // lblYonetim
            //
            this.lblYonetim.ForeColor = System.Drawing.Color.Silver;
            this.lblYonetim.Location = new System.Drawing.Point(14, 186);
            this.lblYonetim.Name = "lblYonetim";
            this.lblYonetim.Size = new System.Drawing.Size(190, 18);
            this.lblYonetim.TabIndex = 4;
            this.lblYonetim.Text = "YÖNETİM PANELİ";
            //
            // btnBolgeYonetimi
            //
            this.btnBolgeYonetimi.FlatAppearance.BorderSize = 0;
            this.btnBolgeYonetimi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBolgeYonetimi.ForeColor = System.Drawing.Color.White;
            this.btnBolgeYonetimi.Location = new System.Drawing.Point(10, 210);
            this.btnBolgeYonetimi.Name = "btnBolgeYonetimi";
            this.btnBolgeYonetimi.Size = new System.Drawing.Size(200, 40);
            this.btnBolgeYonetimi.TabIndex = 5;
            this.btnBolgeYonetimi.Text = "  Bölge Yönetimi";
            this.btnBolgeYonetimi.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBolgeYonetimi.UseVisualStyleBackColor = true;
            this.btnBolgeYonetimi.Click += new System.EventHandler(this.btnBolgeYonetimi_Click);
            //
            // btnUyariYayini
            //
            this.btnUyariYayini.FlatAppearance.BorderSize = 0;
            this.btnUyariYayini.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUyariYayini.ForeColor = System.Drawing.Color.White;
            this.btnUyariYayini.Location = new System.Drawing.Point(10, 254);
            this.btnUyariYayini.Name = "btnUyariYayini";
            this.btnUyariYayini.Size = new System.Drawing.Size(200, 40);
            this.btnUyariYayini.TabIndex = 6;
            this.btnUyariYayini.Text = "  Uyarı Yayını";
            this.btnUyariYayini.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUyariYayini.UseVisualStyleBackColor = true;
            this.btnUyariYayini.Click += new System.EventHandler(this.btnUyariYayini_Click);
            //
            // pnlIcerik
            //
            this.pnlIcerik.Controls.Add(this.dgvUyarilar);
            this.pnlIcerik.Controls.Add(this.txtDetay);
            this.pnlIcerik.Controls.Add(this.pnlUyariBaslik);
            this.pnlIcerik.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlIcerik.Location = new System.Drawing.Point(220, 64);
            this.pnlIcerik.Name = "pnlIcerik";
            this.pnlIcerik.Padding = new System.Windows.Forms.Padding(12);
            this.pnlIcerik.Size = new System.Drawing.Size(764, 473);
            this.pnlIcerik.TabIndex = 2;
            //
            // pnlUyariBaslik
            //
            this.pnlUyariBaslik.Controls.Add(this.btnYenile);
            this.pnlUyariBaslik.Controls.Add(this.lblUyariBaslik);
            this.pnlUyariBaslik.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlUyariBaslik.Location = new System.Drawing.Point(12, 12);
            this.pnlUyariBaslik.Name = "pnlUyariBaslik";
            this.pnlUyariBaslik.Size = new System.Drawing.Size(740, 40);
            this.pnlUyariBaslik.TabIndex = 0;
            //
            // lblUyariBaslik
            //
            this.lblUyariBaslik.AutoSize = true;
            this.lblUyariBaslik.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblUyariBaslik.Location = new System.Drawing.Point(0, 8);
            this.lblUyariBaslik.Name = "lblUyariBaslik";
            this.lblUyariBaslik.Size = new System.Drawing.Size(280, 21);
            this.lblUyariBaslik.TabIndex = 0;
            this.lblUyariBaslik.Text = "Bölgelerimdeki Güncel Uyarılar";
            //
            // btnYenile
            //
            this.btnYenile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnYenile.Location = new System.Drawing.Point(640, 4);
            this.btnYenile.Name = "btnYenile";
            this.btnYenile.Size = new System.Drawing.Size(100, 30);
            this.btnYenile.TabIndex = 1;
            this.btnYenile.Text = "Yenile";
            this.btnYenile.UseVisualStyleBackColor = true;
            this.btnYenile.Click += new System.EventHandler(this.btnYenile_Click);
            //
            // dgvUyarilar
            //
            this.dgvUyarilar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUyarilar.Location = new System.Drawing.Point(12, 52);
            this.dgvUyarilar.Name = "dgvUyarilar";
            this.dgvUyarilar.Size = new System.Drawing.Size(740, 269);
            this.dgvUyarilar.TabIndex = 1;
            this.dgvUyarilar.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgvUyarilar_RowPrePaint);
            this.dgvUyarilar.SelectionChanged += new System.EventHandler(this.dgvUyarilar_SelectionChanged);
            //
            // txtDetay
            //
            this.txtDetay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.txtDetay.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtDetay.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtDetay.Location = new System.Drawing.Point(12, 321);
            this.txtDetay.Multiline = true;
            this.txtDetay.Name = "txtDetay";
            this.txtDetay.ReadOnly = true;
            this.txtDetay.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDetay.Size = new System.Drawing.Size(740, 140);
            this.txtDetay.TabIndex = 2;
            //
            // lblDurum
            //
            this.lblDurum.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.lblDurum.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblDurum.Location = new System.Drawing.Point(0, 537);
            this.lblDurum.Name = "lblDurum";
            this.lblDurum.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lblDurum.Size = new System.Drawing.Size(984, 24);
            this.lblDurum.TabIndex = 3;
            this.lblDurum.Text = "Hazır";
            this.lblDurum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // tmrYenile
            //
            this.tmrYenile.Enabled = true;
            this.tmrYenile.Interval = 60000;
            this.tmrYenile.Tick += new System.EventHandler(this.tmrYenile_Tick);
            //
            // FrmAnaMenu
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this.pnlIcerik);
            this.Controls.Add(this.pnlMenu);
            this.Controls.Add(this.lblDurum);
            this.Controls.Add(this.pnlUst);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(900, 560);
            this.Name = "FrmAnaMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ana Menü - Afet Erken Uyarı ve Bilgilendirme Sistemi";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmAnaMenu_FormClosed);
            this.Load += new System.EventHandler(this.FrmAnaMenu_Load);
            this.pnlUst.ResumeLayout(false);
            this.pnlUst.PerformLayout();
            this.pnlMenu.ResumeLayout(false);
            this.pnlIcerik.ResumeLayout(false);
            this.pnlIcerik.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUyarilar)).EndInit();
            this.pnlUyariBaslik.ResumeLayout(false);
            this.pnlUyariBaslik.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlUst;
        private System.Windows.Forms.Button btnCikis;
        private System.Windows.Forms.Label lblRol;
        private System.Windows.Forms.Label lblHosgeldin;
        private System.Windows.Forms.Label lblUygulamaAdi;
        private System.Windows.Forms.Panel pnlMenu;
        private System.Windows.Forms.Button btnUyariYayini;
        private System.Windows.Forms.Button btnBolgeYonetimi;
        private System.Windows.Forms.Label lblYonetim;
        private System.Windows.Forms.Button btnToplanmaAlanlari;
        private System.Windows.Forms.Button btnHazirlikRehberi;
        private System.Windows.Forms.Button btnAboneliklerim;
        private System.Windows.Forms.Label lblVatandas;
        private System.Windows.Forms.Panel pnlIcerik;
        private System.Windows.Forms.DataGridView dgvUyarilar;
        private System.Windows.Forms.TextBox txtDetay;
        private System.Windows.Forms.Panel pnlUyariBaslik;
        private System.Windows.Forms.Button btnYenile;
        private System.Windows.Forms.Label lblUyariBaslik;
        private System.Windows.Forms.Label lblDurum;
        private System.Windows.Forms.Timer tmrYenile;
    }
}
