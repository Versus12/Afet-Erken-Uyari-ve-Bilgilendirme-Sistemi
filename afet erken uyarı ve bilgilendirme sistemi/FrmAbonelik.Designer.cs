namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    partial class FrmAbonelik
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
            this.dgvAbonelikler = new System.Windows.Forms.DataGridView();
            this.pnlSag = new System.Windows.Forms.Panel();
            this.lblYeniBaslik = new System.Windows.Forms.Label();
            this.lblBolge = new System.Windows.Forms.Label();
            this.cmbBolge = new System.Windows.Forms.ComboBox();
            this.lblKanal = new System.Windows.Forms.Label();
            this.cmbKanal = new System.Windows.Forms.ComboBox();
            this.lblMinSeviye = new System.Windows.Forms.Label();
            this.cmbMinSeviye = new System.Windows.Forms.ComboBox();
            this.btnAboneOl = new System.Windows.Forms.Button();
            this.lblSeciliBaslik = new System.Windows.Forms.Label();
            this.lblSeciliAciklama = new System.Windows.Forms.Label();
            this.chkAktif = new System.Windows.Forms.CheckBox();
            this.btnTercihKaydet = new System.Windows.Forms.Button();
            this.btnIptal = new System.Windows.Forms.Button();
            this.lblToplam = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAbonelikler)).BeginInit();
            this.pnlSag.SuspendLayout();
            this.SuspendLayout();
            //
            // dgvAbonelikler
            //
            this.dgvAbonelikler.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAbonelikler.Location = new System.Drawing.Point(0, 0);
            this.dgvAbonelikler.Name = "dgvAbonelikler";
            this.dgvAbonelikler.Size = new System.Drawing.Size(564, 437);
            this.dgvAbonelikler.TabIndex = 0;
            this.dgvAbonelikler.SelectionChanged += new System.EventHandler(this.dgvAbonelikler_SelectionChanged);
            //
            // pnlSag
            //
            this.pnlSag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.pnlSag.Controls.Add(this.lblYeniBaslik);
            this.pnlSag.Controls.Add(this.lblBolge);
            this.pnlSag.Controls.Add(this.cmbBolge);
            this.pnlSag.Controls.Add(this.lblKanal);
            this.pnlSag.Controls.Add(this.cmbKanal);
            this.pnlSag.Controls.Add(this.lblMinSeviye);
            this.pnlSag.Controls.Add(this.cmbMinSeviye);
            this.pnlSag.Controls.Add(this.btnAboneOl);
            this.pnlSag.Controls.Add(this.lblSeciliBaslik);
            this.pnlSag.Controls.Add(this.lblSeciliAciklama);
            this.pnlSag.Controls.Add(this.chkAktif);
            this.pnlSag.Controls.Add(this.btnTercihKaydet);
            this.pnlSag.Controls.Add(this.btnIptal);
            this.pnlSag.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlSag.Location = new System.Drawing.Point(564, 0);
            this.pnlSag.Name = "pnlSag";
            this.pnlSag.Size = new System.Drawing.Size(300, 461);
            this.pnlSag.TabIndex = 1;
            //
            // lblYeniBaslik
            //
            this.lblYeniBaslik.AutoSize = true;
            this.lblYeniBaslik.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblYeniBaslik.Location = new System.Drawing.Point(16, 14);
            this.lblYeniBaslik.Name = "lblYeniBaslik";
            this.lblYeniBaslik.Text = "Abonelik ve Tercihler";
            //
            // lblBolge
            //
            this.lblBolge.AutoSize = true;
            this.lblBolge.Location = new System.Drawing.Point(18, 48);
            this.lblBolge.Name = "lblBolge";
            this.lblBolge.Text = "Bölge (yeni abonelik için)";
            //
            // cmbBolge
            //
            this.cmbBolge.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBolge.Location = new System.Drawing.Point(20, 66);
            this.cmbBolge.Name = "cmbBolge";
            this.cmbBolge.Size = new System.Drawing.Size(260, 23);
            this.cmbBolge.TabIndex = 1;
            //
            // lblKanal
            //
            this.lblKanal.AutoSize = true;
            this.lblKanal.Location = new System.Drawing.Point(18, 96);
            this.lblKanal.Name = "lblKanal";
            this.lblKanal.Text = "Bildirim kanalı";
            //
            // cmbKanal
            //
            this.cmbKanal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKanal.Location = new System.Drawing.Point(20, 114);
            this.cmbKanal.Name = "cmbKanal";
            this.cmbKanal.Size = new System.Drawing.Size(260, 23);
            this.cmbKanal.TabIndex = 2;
            //
            // lblMinSeviye
            //
            this.lblMinSeviye.AutoSize = true;
            this.lblMinSeviye.Location = new System.Drawing.Point(18, 144);
            this.lblMinSeviye.Name = "lblMinSeviye";
            this.lblMinSeviye.Text = "Bu seviye ve üzerindeki uyarıları göster";
            //
            // cmbMinSeviye
            //
            this.cmbMinSeviye.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMinSeviye.Location = new System.Drawing.Point(20, 162);
            this.cmbMinSeviye.Name = "cmbMinSeviye";
            this.cmbMinSeviye.Size = new System.Drawing.Size(260, 23);
            this.cmbMinSeviye.TabIndex = 3;
            //
            // btnAboneOl
            //
            this.btnAboneOl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.btnAboneOl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAboneOl.ForeColor = System.Drawing.Color.White;
            this.btnAboneOl.Location = new System.Drawing.Point(20, 196);
            this.btnAboneOl.Name = "btnAboneOl";
            this.btnAboneOl.Size = new System.Drawing.Size(260, 34);
            this.btnAboneOl.TabIndex = 4;
            this.btnAboneOl.Text = "Seçili Bölgeye Abone Ol";
            this.btnAboneOl.UseVisualStyleBackColor = false;
            this.btnAboneOl.Click += new System.EventHandler(this.btnAboneOl_Click);
            //
            // lblSeciliBaslik
            //
            this.lblSeciliBaslik.AutoSize = true;
            this.lblSeciliBaslik.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblSeciliBaslik.Location = new System.Drawing.Point(16, 252);
            this.lblSeciliBaslik.Name = "lblSeciliBaslik";
            this.lblSeciliBaslik.Text = "Listeden seçili abonelik";
            //
            // lblSeciliAciklama
            //
            this.lblSeciliAciklama.ForeColor = System.Drawing.Color.Gray;
            this.lblSeciliAciklama.Location = new System.Drawing.Point(18, 274);
            this.lblSeciliAciklama.Name = "lblSeciliAciklama";
            this.lblSeciliAciklama.Size = new System.Drawing.Size(264, 34);
            this.lblSeciliAciklama.Text = "Kanal ve seviye seçimini değiştirip \"Tercihleri Kaydet\" deyiniz.";
            //
            // chkAktif
            //
            this.chkAktif.AutoSize = true;
            this.chkAktif.Location = new System.Drawing.Point(20, 312);
            this.chkAktif.Name = "chkAktif";
            this.chkAktif.TabIndex = 5;
            this.chkAktif.Text = "Bildirimler aktif";
            this.chkAktif.UseVisualStyleBackColor = true;
            //
            // btnTercihKaydet
            //
            this.btnTercihKaydet.Location = new System.Drawing.Point(20, 340);
            this.btnTercihKaydet.Name = "btnTercihKaydet";
            this.btnTercihKaydet.Size = new System.Drawing.Size(260, 34);
            this.btnTercihKaydet.TabIndex = 6;
            this.btnTercihKaydet.Text = "Tercihleri Kaydet";
            this.btnTercihKaydet.UseVisualStyleBackColor = true;
            this.btnTercihKaydet.Click += new System.EventHandler(this.btnTercihKaydet_Click);
            //
            // btnIptal
            //
            this.btnIptal.ForeColor = System.Drawing.Color.Firebrick;
            this.btnIptal.Location = new System.Drawing.Point(20, 380);
            this.btnIptal.Name = "btnIptal";
            this.btnIptal.Size = new System.Drawing.Size(260, 34);
            this.btnIptal.TabIndex = 7;
            this.btnIptal.Text = "Aboneliği İptal Et";
            this.btnIptal.UseVisualStyleBackColor = true;
            this.btnIptal.Click += new System.EventHandler(this.btnIptal_Click);
            //
            // lblToplam
            //
            this.lblToplam.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblToplam.Location = new System.Drawing.Point(0, 437);
            this.lblToplam.Name = "lblToplam";
            this.lblToplam.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.lblToplam.Size = new System.Drawing.Size(564, 24);
            this.lblToplam.TabIndex = 2;
            this.lblToplam.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // FrmAbonelik
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(864, 461);
            this.Controls.Add(this.dgvAbonelikler);
            this.Controls.Add(this.lblToplam);
            this.Controls.Add(this.pnlSag);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimizeBox = false;
            this.Name = "FrmAbonelik";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bölge Aboneliklerim";
            this.Load += new System.EventHandler(this.FrmAbonelik_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAbonelikler)).EndInit();
            this.pnlSag.ResumeLayout(false);
            this.pnlSag.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvAbonelikler;
        private System.Windows.Forms.Panel pnlSag;
        private System.Windows.Forms.Label lblYeniBaslik;
        private System.Windows.Forms.Label lblBolge;
        private System.Windows.Forms.ComboBox cmbBolge;
        private System.Windows.Forms.Label lblKanal;
        private System.Windows.Forms.ComboBox cmbKanal;
        private System.Windows.Forms.Label lblMinSeviye;
        private System.Windows.Forms.ComboBox cmbMinSeviye;
        private System.Windows.Forms.Button btnAboneOl;
        private System.Windows.Forms.Label lblSeciliBaslik;
        private System.Windows.Forms.Label lblSeciliAciklama;
        private System.Windows.Forms.CheckBox chkAktif;
        private System.Windows.Forms.Button btnTercihKaydet;
        private System.Windows.Forms.Button btnIptal;
        private System.Windows.Forms.Label lblToplam;
    }
}
