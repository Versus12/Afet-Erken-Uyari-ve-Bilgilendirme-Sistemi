namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    partial class FrmToplanmaAlanlari
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
            this.chkAboneliklerim = new System.Windows.Forms.CheckBox();
            this.dgvAlanlar = new System.Windows.Forms.DataGridView();
            this.lblToplam = new System.Windows.Forms.Label();
            this.grpDuzenle = new System.Windows.Forms.GroupBox();
            this.lblBolge = new System.Windows.Forms.Label();
            this.cmbBolge = new System.Windows.Forms.ComboBox();
            this.lblAlanAdi = new System.Windows.Forms.Label();
            this.txtAlanAdi = new System.Windows.Forms.TextBox();
            this.lblAdres = new System.Windows.Forms.Label();
            this.txtAdres = new System.Windows.Forms.TextBox();
            this.lblKapasite = new System.Windows.Forms.Label();
            this.nudKapasite = new System.Windows.Forms.NumericUpDown();
            this.lblEnlem = new System.Windows.Forms.Label();
            this.txtEnlem = new System.Windows.Forms.TextBox();
            this.lblBoylam = new System.Windows.Forms.Label();
            this.txtBoylam = new System.Windows.Forms.TextBox();
            this.btnYeni = new System.Windows.Forms.Button();
            this.btnKaydet = new System.Windows.Forms.Button();
            this.btnSil = new System.Windows.Forms.Button();
            this.pnlUst.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlanlar)).BeginInit();
            this.grpDuzenle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudKapasite)).BeginInit();
            this.SuspendLayout();
            //
            // pnlUst
            //
            this.pnlUst.Controls.Add(this.lblFiltre);
            this.pnlUst.Controls.Add(this.cmbFiltre);
            this.pnlUst.Controls.Add(this.chkAboneliklerim);
            this.pnlUst.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlUst.Location = new System.Drawing.Point(0, 0);
            this.pnlUst.Name = "pnlUst";
            this.pnlUst.Size = new System.Drawing.Size(924, 44);
            this.pnlUst.TabIndex = 0;
            //
            // lblFiltre
            //
            this.lblFiltre.AutoSize = true;
            this.lblFiltre.Location = new System.Drawing.Point(10, 14);
            this.lblFiltre.Name = "lblFiltre";
            this.lblFiltre.Text = "Bölge:";
            //
            // cmbFiltre
            //
            this.cmbFiltre.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFiltre.Location = new System.Drawing.Point(60, 10);
            this.cmbFiltre.Name = "cmbFiltre";
            this.cmbFiltre.Size = new System.Drawing.Size(220, 23);
            this.cmbFiltre.TabIndex = 0;
            this.cmbFiltre.SelectedIndexChanged += new System.EventHandler(this.Filtre_Degisti);
            //
            // chkAboneliklerim
            //
            this.chkAboneliklerim.AutoSize = true;
            this.chkAboneliklerim.Location = new System.Drawing.Point(300, 12);
            this.chkAboneliklerim.Name = "chkAboneliklerim";
            this.chkAboneliklerim.TabIndex = 1;
            this.chkAboneliklerim.Text = "Yalnızca abone olduğum bölgeler";
            this.chkAboneliklerim.UseVisualStyleBackColor = true;
            this.chkAboneliklerim.CheckedChanged += new System.EventHandler(this.Filtre_Degisti);
            //
            // dgvAlanlar
            //
            this.dgvAlanlar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAlanlar.Location = new System.Drawing.Point(0, 44);
            this.dgvAlanlar.Name = "dgvAlanlar";
            this.dgvAlanlar.Size = new System.Drawing.Size(624, 393);
            this.dgvAlanlar.TabIndex = 1;
            this.dgvAlanlar.SelectionChanged += new System.EventHandler(this.dgvAlanlar_SelectionChanged);
            //
            // lblToplam
            //
            this.lblToplam.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblToplam.Location = new System.Drawing.Point(0, 437);
            this.lblToplam.Name = "lblToplam";
            this.lblToplam.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.lblToplam.Size = new System.Drawing.Size(624, 24);
            this.lblToplam.TabIndex = 2;
            this.lblToplam.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // grpDuzenle
            //
            this.grpDuzenle.Controls.Add(this.lblBolge);
            this.grpDuzenle.Controls.Add(this.cmbBolge);
            this.grpDuzenle.Controls.Add(this.lblAlanAdi);
            this.grpDuzenle.Controls.Add(this.txtAlanAdi);
            this.grpDuzenle.Controls.Add(this.lblAdres);
            this.grpDuzenle.Controls.Add(this.txtAdres);
            this.grpDuzenle.Controls.Add(this.lblKapasite);
            this.grpDuzenle.Controls.Add(this.nudKapasite);
            this.grpDuzenle.Controls.Add(this.lblEnlem);
            this.grpDuzenle.Controls.Add(this.txtEnlem);
            this.grpDuzenle.Controls.Add(this.lblBoylam);
            this.grpDuzenle.Controls.Add(this.txtBoylam);
            this.grpDuzenle.Controls.Add(this.btnYeni);
            this.grpDuzenle.Controls.Add(this.btnKaydet);
            this.grpDuzenle.Controls.Add(this.btnSil);
            this.grpDuzenle.Dock = System.Windows.Forms.DockStyle.Right;
            this.grpDuzenle.Location = new System.Drawing.Point(624, 44);
            this.grpDuzenle.Name = "grpDuzenle";
            this.grpDuzenle.Size = new System.Drawing.Size(300, 417);
            this.grpDuzenle.TabIndex = 3;
            this.grpDuzenle.TabStop = false;
            this.grpDuzenle.Text = "Alan Düzenle (Yönetici)";
            //
            // lblBolge
            //
            this.lblBolge.AutoSize = true;
            this.lblBolge.Location = new System.Drawing.Point(12, 24);
            this.lblBolge.Name = "lblBolge";
            this.lblBolge.Text = "Bölge";
            //
            // cmbBolge
            //
            this.cmbBolge.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBolge.Location = new System.Drawing.Point(14, 42);
            this.cmbBolge.Name = "cmbBolge";
            this.cmbBolge.Size = new System.Drawing.Size(270, 23);
            this.cmbBolge.TabIndex = 0;
            //
            // lblAlanAdi
            //
            this.lblAlanAdi.AutoSize = true;
            this.lblAlanAdi.Location = new System.Drawing.Point(12, 72);
            this.lblAlanAdi.Name = "lblAlanAdi";
            this.lblAlanAdi.Text = "Alan adı";
            //
            // txtAlanAdi
            //
            this.txtAlanAdi.Location = new System.Drawing.Point(14, 90);
            this.txtAlanAdi.MaxLength = 150;
            this.txtAlanAdi.Name = "txtAlanAdi";
            this.txtAlanAdi.Size = new System.Drawing.Size(270, 23);
            this.txtAlanAdi.TabIndex = 1;
            //
            // lblAdres
            //
            this.lblAdres.AutoSize = true;
            this.lblAdres.Location = new System.Drawing.Point(12, 120);
            this.lblAdres.Name = "lblAdres";
            this.lblAdres.Text = "Adres";
            //
            // txtAdres
            //
            this.txtAdres.Location = new System.Drawing.Point(14, 138);
            this.txtAdres.MaxLength = 300;
            this.txtAdres.Multiline = true;
            this.txtAdres.Name = "txtAdres";
            this.txtAdres.Size = new System.Drawing.Size(270, 50);
            this.txtAdres.TabIndex = 2;
            //
            // lblKapasite
            //
            this.lblKapasite.AutoSize = true;
            this.lblKapasite.Location = new System.Drawing.Point(12, 196);
            this.lblKapasite.Name = "lblKapasite";
            this.lblKapasite.Text = "Kapasite (kişi)";
            //
            // nudKapasite
            //
            this.nudKapasite.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            this.nudKapasite.Location = new System.Drawing.Point(14, 214);
            this.nudKapasite.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            this.nudKapasite.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudKapasite.Name = "nudKapasite";
            this.nudKapasite.Size = new System.Drawing.Size(120, 23);
            this.nudKapasite.TabIndex = 3;
            this.nudKapasite.ThousandsSeparator = true;
            this.nudKapasite.Value = new decimal(new int[] { 100, 0, 0, 0 });
            //
            // lblEnlem
            //
            this.lblEnlem.AutoSize = true;
            this.lblEnlem.Location = new System.Drawing.Point(12, 244);
            this.lblEnlem.Name = "lblEnlem";
            this.lblEnlem.Text = "Enlem (ops.)";
            //
            // txtEnlem
            //
            this.txtEnlem.Location = new System.Drawing.Point(14, 262);
            this.txtEnlem.Name = "txtEnlem";
            this.txtEnlem.Size = new System.Drawing.Size(130, 23);
            this.txtEnlem.TabIndex = 4;
            //
            // lblBoylam
            //
            this.lblBoylam.AutoSize = true;
            this.lblBoylam.Location = new System.Drawing.Point(152, 244);
            this.lblBoylam.Name = "lblBoylam";
            this.lblBoylam.Text = "Boylam (ops.)";
            //
            // txtBoylam
            //
            this.txtBoylam.Location = new System.Drawing.Point(154, 262);
            this.txtBoylam.Name = "txtBoylam";
            this.txtBoylam.Size = new System.Drawing.Size(130, 23);
            this.txtBoylam.TabIndex = 5;
            //
            // btnYeni
            //
            this.btnYeni.Location = new System.Drawing.Point(14, 302);
            this.btnYeni.Name = "btnYeni";
            this.btnYeni.Size = new System.Drawing.Size(84, 32);
            this.btnYeni.TabIndex = 6;
            this.btnYeni.Text = "Yeni";
            this.btnYeni.UseVisualStyleBackColor = true;
            this.btnYeni.Click += new System.EventHandler(this.btnYeni_Click);
            //
            // btnKaydet
            //
            this.btnKaydet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.btnKaydet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKaydet.ForeColor = System.Drawing.Color.White;
            this.btnKaydet.Location = new System.Drawing.Point(107, 302);
            this.btnKaydet.Name = "btnKaydet";
            this.btnKaydet.Size = new System.Drawing.Size(84, 32);
            this.btnKaydet.TabIndex = 7;
            this.btnKaydet.Text = "Kaydet";
            this.btnKaydet.UseVisualStyleBackColor = false;
            this.btnKaydet.Click += new System.EventHandler(this.btnKaydet_Click);
            //
            // btnSil
            //
            this.btnSil.ForeColor = System.Drawing.Color.Firebrick;
            this.btnSil.Location = new System.Drawing.Point(200, 302);
            this.btnSil.Name = "btnSil";
            this.btnSil.Size = new System.Drawing.Size(84, 32);
            this.btnSil.TabIndex = 8;
            this.btnSil.Text = "Sil";
            this.btnSil.UseVisualStyleBackColor = true;
            this.btnSil.Click += new System.EventHandler(this.btnSil_Click);
            //
            // FrmToplanmaAlanlari
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(924, 461);
            this.Controls.Add(this.dgvAlanlar);
            this.Controls.Add(this.lblToplam);
            this.Controls.Add(this.grpDuzenle);
            this.Controls.Add(this.pnlUst);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimizeBox = false;
            this.Name = "FrmToplanmaAlanlari";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Toplanma Alanları";
            this.Load += new System.EventHandler(this.FrmToplanmaAlanlari_Load);
            this.pnlUst.ResumeLayout(false);
            this.pnlUst.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlanlar)).EndInit();
            this.grpDuzenle.ResumeLayout(false);
            this.grpDuzenle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudKapasite)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlUst;
        private System.Windows.Forms.Label lblFiltre;
        private System.Windows.Forms.ComboBox cmbFiltre;
        private System.Windows.Forms.CheckBox chkAboneliklerim;
        private System.Windows.Forms.DataGridView dgvAlanlar;
        private System.Windows.Forms.Label lblToplam;
        private System.Windows.Forms.GroupBox grpDuzenle;
        private System.Windows.Forms.Label lblBolge;
        private System.Windows.Forms.ComboBox cmbBolge;
        private System.Windows.Forms.Label lblAlanAdi;
        private System.Windows.Forms.TextBox txtAlanAdi;
        private System.Windows.Forms.Label lblAdres;
        private System.Windows.Forms.TextBox txtAdres;
        private System.Windows.Forms.Label lblKapasite;
        private System.Windows.Forms.NumericUpDown nudKapasite;
        private System.Windows.Forms.Label lblEnlem;
        private System.Windows.Forms.TextBox txtEnlem;
        private System.Windows.Forms.Label lblBoylam;
        private System.Windows.Forms.TextBox txtBoylam;
        private System.Windows.Forms.Button btnYeni;
        private System.Windows.Forms.Button btnKaydet;
        private System.Windows.Forms.Button btnSil;
    }
}
