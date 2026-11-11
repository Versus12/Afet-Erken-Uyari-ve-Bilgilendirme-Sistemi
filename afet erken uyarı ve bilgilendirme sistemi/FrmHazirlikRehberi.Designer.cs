namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    partial class FrmHazirlikRehberi
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
            this.pnlUst = new System.Windows.Forms.Panel();
            this.lblFiltre = new System.Windows.Forms.Label();
            this.cmbFiltre = new System.Windows.Forms.ComboBox();
            this.dgvRehber = new System.Windows.Forms.DataGridView();
            this.pnlSag = new System.Windows.Forms.Panel();
            this.txtGoster = new System.Windows.Forms.TextBox();
            this.grpDuzenle = new System.Windows.Forms.GroupBox();
            this.lblAfetTuru = new System.Windows.Forms.Label();
            this.cmbAfetTuru = new System.Windows.Forms.ComboBox();
            this.lblAsama = new System.Windows.Forms.Label();
            this.cmbAsama = new System.Windows.Forms.ComboBox();
            this.lblSira = new System.Windows.Forms.Label();
            this.nudSira = new System.Windows.Forms.NumericUpDown();
            this.lblBaslik = new System.Windows.Forms.Label();
            this.txtBaslik = new System.Windows.Forms.TextBox();
            this.lblIcerik = new System.Windows.Forms.Label();
            this.txtIcerik = new System.Windows.Forms.TextBox();
            this.btnYeni = new System.Windows.Forms.Button();
            this.btnKaydet = new System.Windows.Forms.Button();
            this.btnSil = new System.Windows.Forms.Button();
            this.pnlUst.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRehber)).BeginInit();
            this.pnlSag.SuspendLayout();
            this.grpDuzenle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSira)).BeginInit();
            this.SuspendLayout();
            //
            // pnlUst
            //
            this.pnlUst.Controls.Add(this.lblFiltre);
            this.pnlUst.Controls.Add(this.cmbFiltre);
            this.pnlUst.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlUst.Location = new System.Drawing.Point(0, 0);
            this.pnlUst.Name = "pnlUst";
            this.pnlUst.Size = new System.Drawing.Size(944, 44);
            this.pnlUst.TabIndex = 0;
            //
            // lblFiltre
            //
            this.lblFiltre.AutoSize = true;
            this.lblFiltre.Location = new System.Drawing.Point(10, 14);
            this.lblFiltre.Name = "lblFiltre";
            this.lblFiltre.Text = "Afet türü:";
            //
            // cmbFiltre
            //
            this.cmbFiltre.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFiltre.Location = new System.Drawing.Point(80, 10);
            this.cmbFiltre.Name = "cmbFiltre";
            this.cmbFiltre.Size = new System.Drawing.Size(200, 23);
            this.cmbFiltre.TabIndex = 0;
            this.cmbFiltre.SelectedIndexChanged += new System.EventHandler(this.cmbFiltre_SelectedIndexChanged);
            //
            // dgvRehber
            //
            this.dgvRehber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRehber.Location = new System.Drawing.Point(0, 44);
            this.dgvRehber.Name = "dgvRehber";
            this.dgvRehber.Size = new System.Drawing.Size(424, 517);
            this.dgvRehber.TabIndex = 1;
            this.dgvRehber.SelectionChanged += new System.EventHandler(this.dgvRehber_SelectionChanged);
            //
            // pnlSag
            //
            this.pnlSag.Controls.Add(this.txtGoster);
            this.pnlSag.Controls.Add(this.grpDuzenle);
            this.pnlSag.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlSag.Location = new System.Drawing.Point(424, 44);
            this.pnlSag.Name = "pnlSag";
            this.pnlSag.Padding = new System.Windows.Forms.Padding(8, 0, 8, 8);
            this.pnlSag.Size = new System.Drawing.Size(520, 517);
            this.pnlSag.TabIndex = 2;
            //
            // txtGoster
            //
            this.txtGoster.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.txtGoster.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtGoster.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtGoster.Location = new System.Drawing.Point(8, 0);
            this.txtGoster.Multiline = true;
            this.txtGoster.Name = "txtGoster";
            this.txtGoster.ReadOnly = true;
            this.txtGoster.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGoster.Size = new System.Drawing.Size(504, 229);
            this.txtGoster.TabIndex = 0;
            //
            // grpDuzenle
            //
            this.grpDuzenle.Controls.Add(this.lblAfetTuru);
            this.grpDuzenle.Controls.Add(this.cmbAfetTuru);
            this.grpDuzenle.Controls.Add(this.lblAsama);
            this.grpDuzenle.Controls.Add(this.cmbAsama);
            this.grpDuzenle.Controls.Add(this.lblSira);
            this.grpDuzenle.Controls.Add(this.nudSira);
            this.grpDuzenle.Controls.Add(this.lblBaslik);
            this.grpDuzenle.Controls.Add(this.txtBaslik);
            this.grpDuzenle.Controls.Add(this.lblIcerik);
            this.grpDuzenle.Controls.Add(this.txtIcerik);
            this.grpDuzenle.Controls.Add(this.btnYeni);
            this.grpDuzenle.Controls.Add(this.btnKaydet);
            this.grpDuzenle.Controls.Add(this.btnSil);
            this.grpDuzenle.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpDuzenle.Location = new System.Drawing.Point(8, 229);
            this.grpDuzenle.Name = "grpDuzenle";
            this.grpDuzenle.Size = new System.Drawing.Size(504, 280);
            this.grpDuzenle.TabIndex = 1;
            this.grpDuzenle.TabStop = false;
            this.grpDuzenle.Text = "Rehber Düzenle (Yönetici)";
            //
            // lblAfetTuru
            //
            this.lblAfetTuru.AutoSize = true;
            this.lblAfetTuru.Location = new System.Drawing.Point(12, 24);
            this.lblAfetTuru.Name = "lblAfetTuru";
            this.lblAfetTuru.Text = "Afet türü";
            //
            // cmbAfetTuru
            //
            this.cmbAfetTuru.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAfetTuru.Location = new System.Drawing.Point(14, 42);
            this.cmbAfetTuru.Name = "cmbAfetTuru";
            this.cmbAfetTuru.Size = new System.Drawing.Size(150, 23);
            this.cmbAfetTuru.TabIndex = 0;
            //
            // lblAsama
            //
            this.lblAsama.AutoSize = true;
            this.lblAsama.Location = new System.Drawing.Point(174, 24);
            this.lblAsama.Name = "lblAsama";
            this.lblAsama.Text = "Aşama";
            //
            // cmbAsama
            //
            this.cmbAsama.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAsama.Location = new System.Drawing.Point(176, 42);
            this.cmbAsama.Name = "cmbAsama";
            this.cmbAsama.Size = new System.Drawing.Size(150, 23);
            this.cmbAsama.TabIndex = 1;
            //
            // lblSira
            //
            this.lblSira.AutoSize = true;
            this.lblSira.Location = new System.Drawing.Point(336, 24);
            this.lblSira.Name = "lblSira";
            this.lblSira.Text = "Sıra";
            //
            // nudSira
            //
            this.nudSira.Location = new System.Drawing.Point(338, 42);
            this.nudSira.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudSira.Name = "nudSira";
            this.nudSira.Size = new System.Drawing.Size(70, 23);
            this.nudSira.TabIndex = 2;
            this.nudSira.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            // lblBaslik
            //
            this.lblBaslik.AutoSize = true;
            this.lblBaslik.Location = new System.Drawing.Point(12, 72);
            this.lblBaslik.Name = "lblBaslik";
            this.lblBaslik.Text = "Başlık";
            //
            // txtBaslik
            //
            this.txtBaslik.Location = new System.Drawing.Point(14, 90);
            this.txtBaslik.MaxLength = 150;
            this.txtBaslik.Name = "txtBaslik";
            this.txtBaslik.Size = new System.Drawing.Size(476, 23);
            this.txtBaslik.TabIndex = 3;
            //
            // lblIcerik
            //
            this.lblIcerik.AutoSize = true;
            this.lblIcerik.Location = new System.Drawing.Point(12, 118);
            this.lblIcerik.Name = "lblIcerik";
            this.lblIcerik.Text = "İçerik";
            //
            // txtIcerik
            //
            this.txtIcerik.Location = new System.Drawing.Point(14, 136);
            this.txtIcerik.Multiline = true;
            this.txtIcerik.Name = "txtIcerik";
            this.txtIcerik.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtIcerik.Size = new System.Drawing.Size(476, 90);
            this.txtIcerik.TabIndex = 4;
            //
            // btnYeni
            //
            this.btnYeni.Location = new System.Drawing.Point(14, 236);
            this.btnYeni.Name = "btnYeni";
            this.btnYeni.Size = new System.Drawing.Size(100, 32);
            this.btnYeni.TabIndex = 5;
            this.btnYeni.Text = "Yeni";
            this.btnYeni.UseVisualStyleBackColor = true;
            this.btnYeni.Click += new System.EventHandler(this.btnYeni_Click);
            //
            // btnKaydet
            //
            this.btnKaydet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.btnKaydet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKaydet.ForeColor = System.Drawing.Color.White;
            this.btnKaydet.Location = new System.Drawing.Point(120, 236);
            this.btnKaydet.Name = "btnKaydet";
            this.btnKaydet.Size = new System.Drawing.Size(100, 32);
            this.btnKaydet.TabIndex = 6;
            this.btnKaydet.Text = "Kaydet";
            this.btnKaydet.UseVisualStyleBackColor = false;
            this.btnKaydet.Click += new System.EventHandler(this.btnKaydet_Click);
            //
            // btnSil
            //
            this.btnSil.ForeColor = System.Drawing.Color.Firebrick;
            this.btnSil.Location = new System.Drawing.Point(226, 236);
            this.btnSil.Name = "btnSil";
            this.btnSil.Size = new System.Drawing.Size(100, 32);
            this.btnSil.TabIndex = 7;
            this.btnSil.Text = "Sil";
            this.btnSil.UseVisualStyleBackColor = true;
            this.btnSil.Click += new System.EventHandler(this.btnSil_Click);
            //
            // FrmHazirlikRehberi
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(944, 561);
            this.Controls.Add(this.dgvRehber);
            this.Controls.Add(this.pnlSag);
            this.Controls.Add(this.pnlUst);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimizeBox = false;
            this.Name = "FrmHazirlikRehberi";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Hazırlık Rehberi";
            this.Load += new System.EventHandler(this.FrmHazirlikRehberi_Load);
            this.pnlUst.ResumeLayout(false);
            this.pnlUst.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRehber)).EndInit();
            this.pnlSag.ResumeLayout(false);
            this.pnlSag.PerformLayout();
            this.grpDuzenle.ResumeLayout(false);
            this.grpDuzenle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSira)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlUst;
        private System.Windows.Forms.Label lblFiltre;
        private System.Windows.Forms.ComboBox cmbFiltre;
        private System.Windows.Forms.DataGridView dgvRehber;
        private System.Windows.Forms.Panel pnlSag;
        private System.Windows.Forms.TextBox txtGoster;
        private System.Windows.Forms.GroupBox grpDuzenle;
        private System.Windows.Forms.Label lblAfetTuru;
        private System.Windows.Forms.ComboBox cmbAfetTuru;
        private System.Windows.Forms.Label lblAsama;
        private System.Windows.Forms.ComboBox cmbAsama;
        private System.Windows.Forms.Label lblSira;
        private System.Windows.Forms.NumericUpDown nudSira;
        private System.Windows.Forms.Label lblBaslik;
        private System.Windows.Forms.TextBox txtBaslik;
        private System.Windows.Forms.Label lblIcerik;
        private System.Windows.Forms.TextBox txtIcerik;
        private System.Windows.Forms.Button btnYeni;
        private System.Windows.Forms.Button btnKaydet;
        private System.Windows.Forms.Button btnSil;
    }
}
