namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    partial class FrmUyariYayini
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
            this.pnlSol = new System.Windows.Forms.Panel();
            this.lblBaslikForm = new System.Windows.Forms.Label();
            this.lblBaslik = new System.Windows.Forms.Label();
            this.txtBaslik = new System.Windows.Forms.TextBox();
            this.lblMesaj = new System.Windows.Forms.Label();
            this.txtMesaj = new System.Windows.Forms.TextBox();
            this.lblKarakter = new System.Windows.Forms.Label();
            this.lblAfetTuru = new System.Windows.Forms.Label();
            this.cmbAfetTuru = new System.Windows.Forms.ComboBox();
            this.lblSeviye = new System.Windows.Forms.Label();
            this.cmbSeviye = new System.Windows.Forms.ComboBox();
            this.lblBolgeler = new System.Windows.Forms.Label();
            this.chkTumBolgeler = new System.Windows.Forms.CheckBox();
            this.clbBolgeler = new System.Windows.Forms.CheckedListBox();
            this.chkBitis = new System.Windows.Forms.CheckBox();
            this.dtpBitis = new System.Windows.Forms.DateTimePicker();
            this.btnYayinla = new System.Windows.Forms.Button();
            this.dgvUyarilar = new System.Windows.Forms.DataGridView();
            this.pnlAlt = new System.Windows.Forms.Panel();
            this.btnYayindanKaldir = new System.Windows.Forms.Button();
            this.lblListe = new System.Windows.Forms.Label();
            this.pnlSol.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUyarilar)).BeginInit();
            this.pnlAlt.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlSol
            //
            this.pnlSol.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.pnlSol.Controls.Add(this.lblBaslikForm);
            this.pnlSol.Controls.Add(this.lblBaslik);
            this.pnlSol.Controls.Add(this.txtBaslik);
            this.pnlSol.Controls.Add(this.lblMesaj);
            this.pnlSol.Controls.Add(this.txtMesaj);
            this.pnlSol.Controls.Add(this.lblKarakter);
            this.pnlSol.Controls.Add(this.lblAfetTuru);
            this.pnlSol.Controls.Add(this.cmbAfetTuru);
            this.pnlSol.Controls.Add(this.lblSeviye);
            this.pnlSol.Controls.Add(this.cmbSeviye);
            this.pnlSol.Controls.Add(this.lblBolgeler);
            this.pnlSol.Controls.Add(this.chkTumBolgeler);
            this.pnlSol.Controls.Add(this.clbBolgeler);
            this.pnlSol.Controls.Add(this.chkBitis);
            this.pnlSol.Controls.Add(this.dtpBitis);
            this.pnlSol.Controls.Add(this.btnYayinla);
            this.pnlSol.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSol.Location = new System.Drawing.Point(0, 0);
            this.pnlSol.Name = "pnlSol";
            this.pnlSol.Size = new System.Drawing.Size(340, 581);
            this.pnlSol.TabIndex = 0;
            //
            // lblBaslikForm
            //
            this.lblBaslikForm.AutoSize = true;
            this.lblBaslikForm.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblBaslikForm.Location = new System.Drawing.Point(16, 12);
            this.lblBaslikForm.Name = "lblBaslikForm";
            this.lblBaslikForm.Text = "Yeni Uyarı";
            //
            // lblBaslik
            //
            this.lblBaslik.AutoSize = true;
            this.lblBaslik.Location = new System.Drawing.Point(18, 42);
            this.lblBaslik.Name = "lblBaslik";
            this.lblBaslik.Text = "Başlık";
            //
            // txtBaslik
            //
            this.txtBaslik.Location = new System.Drawing.Point(20, 60);
            this.txtBaslik.MaxLength = 150;
            this.txtBaslik.Name = "txtBaslik";
            this.txtBaslik.Size = new System.Drawing.Size(300, 23);
            this.txtBaslik.TabIndex = 1;
            //
            // lblMesaj
            //
            this.lblMesaj.AutoSize = true;
            this.lblMesaj.Location = new System.Drawing.Point(18, 90);
            this.lblMesaj.Name = "lblMesaj";
            this.lblMesaj.Text = "Mesaj (sade ve anlaşılır yazınız)";
            //
            // txtMesaj
            //
            this.txtMesaj.Location = new System.Drawing.Point(20, 108);
            this.txtMesaj.MaxLength = 2000;
            this.txtMesaj.Multiline = true;
            this.txtMesaj.Name = "txtMesaj";
            this.txtMesaj.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMesaj.Size = new System.Drawing.Size(300, 100);
            this.txtMesaj.TabIndex = 2;
            this.txtMesaj.TextChanged += new System.EventHandler(this.txtMesaj_TextChanged);
            //
            // lblKarakter
            //
            this.lblKarakter.ForeColor = System.Drawing.Color.Gray;
            this.lblKarakter.Location = new System.Drawing.Point(220, 210);
            this.lblKarakter.Name = "lblKarakter";
            this.lblKarakter.Size = new System.Drawing.Size(100, 16);
            this.lblKarakter.Text = "0 / 2000";
            this.lblKarakter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblAfetTuru
            //
            this.lblAfetTuru.AutoSize = true;
            this.lblAfetTuru.Location = new System.Drawing.Point(18, 226);
            this.lblAfetTuru.Name = "lblAfetTuru";
            this.lblAfetTuru.Text = "Afet türü";
            //
            // cmbAfetTuru
            //
            this.cmbAfetTuru.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAfetTuru.Location = new System.Drawing.Point(20, 244);
            this.cmbAfetTuru.Name = "cmbAfetTuru";
            this.cmbAfetTuru.Size = new System.Drawing.Size(145, 23);
            this.cmbAfetTuru.TabIndex = 3;
            //
            // lblSeviye
            //
            this.lblSeviye.AutoSize = true;
            this.lblSeviye.Location = new System.Drawing.Point(173, 226);
            this.lblSeviye.Name = "lblSeviye";
            this.lblSeviye.Text = "Seviye";
            //
            // cmbSeviye
            //
            this.cmbSeviye.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSeviye.Location = new System.Drawing.Point(175, 244);
            this.cmbSeviye.Name = "cmbSeviye";
            this.cmbSeviye.Size = new System.Drawing.Size(145, 23);
            this.cmbSeviye.TabIndex = 4;
            //
            // lblBolgeler
            //
            this.lblBolgeler.AutoSize = true;
            this.lblBolgeler.Location = new System.Drawing.Point(18, 276);
            this.lblBolgeler.Name = "lblBolgeler";
            this.lblBolgeler.Text = "Hedef bölgeler";
            //
            // chkTumBolgeler
            //
            this.chkTumBolgeler.AutoSize = true;
            this.chkTumBolgeler.Location = new System.Drawing.Point(220, 275);
            this.chkTumBolgeler.Name = "chkTumBolgeler";
            this.chkTumBolgeler.TabIndex = 5;
            this.chkTumBolgeler.Text = "Tümünü seç";
            this.chkTumBolgeler.UseVisualStyleBackColor = true;
            this.chkTumBolgeler.CheckedChanged += new System.EventHandler(this.chkTumBolgeler_CheckedChanged);
            //
            // clbBolgeler
            //
            this.clbBolgeler.CheckOnClick = true;
            this.clbBolgeler.Location = new System.Drawing.Point(20, 296);
            this.clbBolgeler.Name = "clbBolgeler";
            this.clbBolgeler.Size = new System.Drawing.Size(300, 166);
            this.clbBolgeler.TabIndex = 6;
            //
            // chkBitis
            //
            this.chkBitis.AutoSize = true;
            this.chkBitis.Checked = true;
            this.chkBitis.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBitis.Location = new System.Drawing.Point(20, 474);
            this.chkBitis.Name = "chkBitis";
            this.chkBitis.TabIndex = 7;
            this.chkBitis.Text = "Geçerlilik bitişi";
            this.chkBitis.UseVisualStyleBackColor = true;
            this.chkBitis.CheckedChanged += new System.EventHandler(this.chkBitis_CheckedChanged);
            //
            // dtpBitis
            //
            this.dtpBitis.CustomFormat = "dd.MM.yyyy HH:mm";
            this.dtpBitis.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpBitis.Location = new System.Drawing.Point(150, 472);
            this.dtpBitis.Name = "dtpBitis";
            this.dtpBitis.Size = new System.Drawing.Size(170, 23);
            this.dtpBitis.TabIndex = 8;
            //
            // btnYayinla
            //
            this.btnYayinla.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.btnYayinla.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnYayinla.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnYayinla.ForeColor = System.Drawing.Color.White;
            this.btnYayinla.Location = new System.Drawing.Point(20, 512);
            this.btnYayinla.Name = "btnYayinla";
            this.btnYayinla.Size = new System.Drawing.Size(300, 40);
            this.btnYayinla.TabIndex = 9;
            this.btnYayinla.Text = "Uyarıyı Yayınla";
            this.btnYayinla.UseVisualStyleBackColor = false;
            this.btnYayinla.Click += new System.EventHandler(this.btnYayinla_Click);
            //
            // dgvUyarilar
            //
            this.dgvUyarilar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUyarilar.Location = new System.Drawing.Point(340, 32);
            this.dgvUyarilar.Name = "dgvUyarilar";
            this.dgvUyarilar.Size = new System.Drawing.Size(644, 503);
            this.dgvUyarilar.TabIndex = 1;
            this.dgvUyarilar.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgvUyarilar_RowPrePaint);
            //
            // lblListe
            //
            this.lblListe.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblListe.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblListe.Location = new System.Drawing.Point(340, 0);
            this.lblListe.Name = "lblListe";
            this.lblListe.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.lblListe.Size = new System.Drawing.Size(644, 32);
            this.lblListe.Text = "Yayınlanan Uyarılar";
            this.lblListe.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlAlt
            //
            this.pnlAlt.Controls.Add(this.btnYayindanKaldir);
            this.pnlAlt.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlAlt.Location = new System.Drawing.Point(340, 535);
            this.pnlAlt.Name = "pnlAlt";
            this.pnlAlt.Size = new System.Drawing.Size(644, 46);
            this.pnlAlt.TabIndex = 2;
            //
            // btnYayindanKaldir
            //
            this.btnYayindanKaldir.ForeColor = System.Drawing.Color.Firebrick;
            this.btnYayindanKaldir.Location = new System.Drawing.Point(8, 6);
            this.btnYayindanKaldir.Name = "btnYayindanKaldir";
            this.btnYayindanKaldir.Size = new System.Drawing.Size(200, 34);
            this.btnYayindanKaldir.TabIndex = 0;
            this.btnYayindanKaldir.Text = "Seçili Uyarıyı Yayından Kaldır";
            this.btnYayindanKaldir.UseVisualStyleBackColor = true;
            this.btnYayindanKaldir.Click += new System.EventHandler(this.btnYayindanKaldir_Click);
            //
            // FrmUyariYayini
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1124, 581);
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.dgvUyarilar);
            this.Controls.Add(this.lblListe);
            this.Controls.Add(this.pnlAlt);
            this.Controls.Add(this.pnlSol);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimizeBox = false;
            this.Name = "FrmUyariYayini";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Uyarı Yayını (Yönetici)";
            this.Load += new System.EventHandler(this.FrmUyariYayini_Load);
            this.pnlSol.ResumeLayout(false);
            this.pnlSol.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUyarilar)).EndInit();
            this.pnlAlt.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlSol;
        private System.Windows.Forms.Label lblBaslikForm;
        private System.Windows.Forms.Label lblBaslik;
        private System.Windows.Forms.TextBox txtBaslik;
        private System.Windows.Forms.Label lblMesaj;
        private System.Windows.Forms.TextBox txtMesaj;
        private System.Windows.Forms.Label lblKarakter;
        private System.Windows.Forms.Label lblAfetTuru;
        private System.Windows.Forms.ComboBox cmbAfetTuru;
        private System.Windows.Forms.Label lblSeviye;
        private System.Windows.Forms.ComboBox cmbSeviye;
        private System.Windows.Forms.Label lblBolgeler;
        private System.Windows.Forms.CheckBox chkTumBolgeler;
        private System.Windows.Forms.CheckedListBox clbBolgeler;
        private System.Windows.Forms.CheckBox chkBitis;
        private System.Windows.Forms.DateTimePicker dtpBitis;
        private System.Windows.Forms.Button btnYayinla;
        private System.Windows.Forms.DataGridView dgvUyarilar;
        private System.Windows.Forms.Panel pnlAlt;
        private System.Windows.Forms.Button btnYayindanKaldir;
        private System.Windows.Forms.Label lblListe;
    }
}
