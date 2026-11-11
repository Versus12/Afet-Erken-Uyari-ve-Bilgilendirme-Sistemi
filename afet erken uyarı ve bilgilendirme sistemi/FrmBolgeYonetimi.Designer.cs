namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    partial class FrmBolgeYonetimi
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
            this.dgvBolgeler = new System.Windows.Forms.DataGridView();
            this.pnlSag = new System.Windows.Forms.Panel();
            this.lblBaslik = new System.Windows.Forms.Label();
            this.lblIl = new System.Windows.Forms.Label();
            this.txtIl = new System.Windows.Forms.TextBox();
            this.lblIlce = new System.Windows.Forms.Label();
            this.txtIlce = new System.Windows.Forms.TextBox();
            this.lblRisk = new System.Windows.Forms.Label();
            this.nudRisk = new System.Windows.Forms.NumericUpDown();
            this.lblEnlem = new System.Windows.Forms.Label();
            this.txtEnlem = new System.Windows.Forms.TextBox();
            this.lblBoylam = new System.Windows.Forms.Label();
            this.txtBoylam = new System.Windows.Forms.TextBox();
            this.btnEkle = new System.Windows.Forms.Button();
            this.btnGuncelle = new System.Windows.Forms.Button();
            this.btnSil = new System.Windows.Forms.Button();
            this.btnTemizle = new System.Windows.Forms.Button();
            this.lblToplam = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBolgeler)).BeginInit();
            this.pnlSag.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRisk)).BeginInit();
            this.SuspendLayout();
            //
            // dgvBolgeler
            //
            this.dgvBolgeler.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBolgeler.Location = new System.Drawing.Point(0, 0);
            this.dgvBolgeler.Name = "dgvBolgeler";
            this.dgvBolgeler.Size = new System.Drawing.Size(544, 437);
            this.dgvBolgeler.TabIndex = 0;
            this.dgvBolgeler.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBolgeler_CellClick);
            //
            // pnlSag
            //
            this.pnlSag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.pnlSag.Controls.Add(this.lblBaslik);
            this.pnlSag.Controls.Add(this.lblIl);
            this.pnlSag.Controls.Add(this.txtIl);
            this.pnlSag.Controls.Add(this.lblIlce);
            this.pnlSag.Controls.Add(this.txtIlce);
            this.pnlSag.Controls.Add(this.lblRisk);
            this.pnlSag.Controls.Add(this.nudRisk);
            this.pnlSag.Controls.Add(this.lblEnlem);
            this.pnlSag.Controls.Add(this.txtEnlem);
            this.pnlSag.Controls.Add(this.lblBoylam);
            this.pnlSag.Controls.Add(this.txtBoylam);
            this.pnlSag.Controls.Add(this.btnEkle);
            this.pnlSag.Controls.Add(this.btnGuncelle);
            this.pnlSag.Controls.Add(this.btnSil);
            this.pnlSag.Controls.Add(this.btnTemizle);
            this.pnlSag.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlSag.Location = new System.Drawing.Point(544, 0);
            this.pnlSag.Name = "pnlSag";
            this.pnlSag.Size = new System.Drawing.Size(280, 461);
            this.pnlSag.TabIndex = 1;
            //
            // lblBaslik
            //
            this.lblBaslik.AutoSize = true;
            this.lblBaslik.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblBaslik.Location = new System.Drawing.Point(16, 14);
            this.lblBaslik.Name = "lblBaslik";
            this.lblBaslik.Text = "Bölge Bilgileri";
            //
            // lblIl
            //
            this.lblIl.AutoSize = true;
            this.lblIl.Location = new System.Drawing.Point(18, 50);
            this.lblIl.Name = "lblIl";
            this.lblIl.Text = "İl";
            //
            // txtIl
            //
            this.txtIl.Location = new System.Drawing.Point(20, 68);
            this.txtIl.MaxLength = 50;
            this.txtIl.Name = "txtIl";
            this.txtIl.Size = new System.Drawing.Size(240, 23);
            this.txtIl.TabIndex = 1;
            //
            // lblIlce
            //
            this.lblIlce.AutoSize = true;
            this.lblIlce.Location = new System.Drawing.Point(18, 98);
            this.lblIlce.Name = "lblIlce";
            this.lblIlce.Text = "İlçe";
            //
            // txtIlce
            //
            this.txtIlce.Location = new System.Drawing.Point(20, 116);
            this.txtIlce.MaxLength = 50;
            this.txtIlce.Name = "txtIlce";
            this.txtIlce.Size = new System.Drawing.Size(240, 23);
            this.txtIlce.TabIndex = 2;
            //
            // lblRisk
            //
            this.lblRisk.AutoSize = true;
            this.lblRisk.Location = new System.Drawing.Point(18, 146);
            this.lblRisk.Name = "lblRisk";
            this.lblRisk.Text = "Risk seviyesi (1 düşük - 5 çok yüksek)";
            //
            // nudRisk
            //
            this.nudRisk.Location = new System.Drawing.Point(20, 164);
            this.nudRisk.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            this.nudRisk.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudRisk.Name = "nudRisk";
            this.nudRisk.Size = new System.Drawing.Size(80, 23);
            this.nudRisk.TabIndex = 3;
            this.nudRisk.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // lblEnlem
            //
            this.lblEnlem.AutoSize = true;
            this.lblEnlem.Location = new System.Drawing.Point(18, 194);
            this.lblEnlem.Name = "lblEnlem";
            this.lblEnlem.Text = "Enlem (ops.)";
            //
            // txtEnlem
            //
            this.txtEnlem.Location = new System.Drawing.Point(20, 212);
            this.txtEnlem.Name = "txtEnlem";
            this.txtEnlem.Size = new System.Drawing.Size(115, 23);
            this.txtEnlem.TabIndex = 4;
            //
            // lblBoylam
            //
            this.lblBoylam.AutoSize = true;
            this.lblBoylam.Location = new System.Drawing.Point(143, 194);
            this.lblBoylam.Name = "lblBoylam";
            this.lblBoylam.Text = "Boylam (ops.)";
            //
            // txtBoylam
            //
            this.txtBoylam.Location = new System.Drawing.Point(145, 212);
            this.txtBoylam.Name = "txtBoylam";
            this.txtBoylam.Size = new System.Drawing.Size(115, 23);
            this.txtBoylam.TabIndex = 5;
            //
            // btnEkle
            //
            this.btnEkle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.btnEkle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEkle.ForeColor = System.Drawing.Color.White;
            this.btnEkle.Location = new System.Drawing.Point(20, 256);
            this.btnEkle.Name = "btnEkle";
            this.btnEkle.Size = new System.Drawing.Size(115, 34);
            this.btnEkle.TabIndex = 6;
            this.btnEkle.Text = "Ekle";
            this.btnEkle.UseVisualStyleBackColor = false;
            this.btnEkle.Click += new System.EventHandler(this.btnEkle_Click);
            //
            // btnGuncelle
            //
            this.btnGuncelle.Location = new System.Drawing.Point(145, 256);
            this.btnGuncelle.Name = "btnGuncelle";
            this.btnGuncelle.Size = new System.Drawing.Size(115, 34);
            this.btnGuncelle.TabIndex = 7;
            this.btnGuncelle.Text = "Güncelle";
            this.btnGuncelle.UseVisualStyleBackColor = true;
            this.btnGuncelle.Click += new System.EventHandler(this.btnGuncelle_Click);
            //
            // btnSil
            //
            this.btnSil.ForeColor = System.Drawing.Color.Firebrick;
            this.btnSil.Location = new System.Drawing.Point(20, 296);
            this.btnSil.Name = "btnSil";
            this.btnSil.Size = new System.Drawing.Size(115, 34);
            this.btnSil.TabIndex = 8;
            this.btnSil.Text = "Sil";
            this.btnSil.UseVisualStyleBackColor = true;
            this.btnSil.Click += new System.EventHandler(this.btnSil_Click);
            //
            // btnTemizle
            //
            this.btnTemizle.Location = new System.Drawing.Point(145, 296);
            this.btnTemizle.Name = "btnTemizle";
            this.btnTemizle.Size = new System.Drawing.Size(115, 34);
            this.btnTemizle.TabIndex = 9;
            this.btnTemizle.Text = "Temizle";
            this.btnTemizle.UseVisualStyleBackColor = true;
            this.btnTemizle.Click += new System.EventHandler(this.btnTemizle_Click);
            //
            // lblToplam
            //
            this.lblToplam.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblToplam.Location = new System.Drawing.Point(0, 437);
            this.lblToplam.Name = "lblToplam";
            this.lblToplam.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.lblToplam.Size = new System.Drawing.Size(544, 24);
            this.lblToplam.TabIndex = 2;
            this.lblToplam.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // FrmBolgeYonetimi
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(824, 461);
            this.Controls.Add(this.dgvBolgeler);
            this.Controls.Add(this.lblToplam);
            this.Controls.Add(this.pnlSag);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimizeBox = false;
            this.Name = "FrmBolgeYonetimi";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Bölge Yönetimi (Yönetici)";
            this.Load += new System.EventHandler(this.FrmBolgeYonetimi_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBolgeler)).EndInit();
            this.pnlSag.ResumeLayout(false);
            this.pnlSag.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRisk)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvBolgeler;
        private System.Windows.Forms.Panel pnlSag;
        private System.Windows.Forms.Label lblBaslik;
        private System.Windows.Forms.Label lblIl;
        private System.Windows.Forms.TextBox txtIl;
        private System.Windows.Forms.Label lblIlce;
        private System.Windows.Forms.TextBox txtIlce;
        private System.Windows.Forms.Label lblRisk;
        private System.Windows.Forms.NumericUpDown nudRisk;
        private System.Windows.Forms.Label lblEnlem;
        private System.Windows.Forms.TextBox txtEnlem;
        private System.Windows.Forms.Label lblBoylam;
        private System.Windows.Forms.TextBox txtBoylam;
        private System.Windows.Forms.Button btnEkle;
        private System.Windows.Forms.Button btnGuncelle;
        private System.Windows.Forms.Button btnSil;
        private System.Windows.Forms.Button btnTemizle;
        private System.Windows.Forms.Label lblToplam;
    }
}
