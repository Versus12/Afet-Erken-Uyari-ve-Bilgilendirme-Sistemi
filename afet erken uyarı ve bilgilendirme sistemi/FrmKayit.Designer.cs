namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    partial class FrmKayit
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
            this.lblBaslik = new System.Windows.Forms.Label();
            this.lblKullaniciAdi = new System.Windows.Forms.Label();
            this.txtKullaniciAdi = new System.Windows.Forms.TextBox();
            this.lblEposta = new System.Windows.Forms.Label();
            this.txtEposta = new System.Windows.Forms.TextBox();
            this.lblSifre = new System.Windows.Forms.Label();
            this.txtSifre = new System.Windows.Forms.TextBox();
            this.lblSifreTekrar = new System.Windows.Forms.Label();
            this.txtSifreTekrar = new System.Windows.Forms.TextBox();
            this.lblSifreKural = new System.Windows.Forms.Label();
            this.btnKayitOl = new System.Windows.Forms.Button();
            this.btnVazgec = new System.Windows.Forms.Button();
            this.pnlUst.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlUst
            //
            this.pnlUst.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.pnlUst.Controls.Add(this.lblBaslik);
            this.pnlUst.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlUst.Location = new System.Drawing.Point(0, 0);
            this.pnlUst.Name = "pnlUst";
            this.pnlUst.Size = new System.Drawing.Size(424, 60);
            this.pnlUst.TabIndex = 0;
            //
            // lblBaslik
            //
            this.lblBaslik.AutoSize = true;
            this.lblBaslik.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.lblBaslik.ForeColor = System.Drawing.Color.White;
            this.lblBaslik.Location = new System.Drawing.Point(18, 15);
            this.lblBaslik.Name = "lblBaslik";
            this.lblBaslik.Size = new System.Drawing.Size(180, 25);
            this.lblBaslik.TabIndex = 0;
            this.lblBaslik.Text = "Vatandaş Kaydı";
            //
            // lblKullaniciAdi
            //
            this.lblKullaniciAdi.AutoSize = true;
            this.lblKullaniciAdi.Location = new System.Drawing.Point(48, 78);
            this.lblKullaniciAdi.Name = "lblKullaniciAdi";
            this.lblKullaniciAdi.Size = new System.Drawing.Size(74, 15);
            this.lblKullaniciAdi.TabIndex = 1;
            this.lblKullaniciAdi.Text = "Kullanıcı adı";
            //
            // txtKullaniciAdi
            //
            this.txtKullaniciAdi.Location = new System.Drawing.Point(51, 98);
            this.txtKullaniciAdi.MaxLength = 50;
            this.txtKullaniciAdi.Name = "txtKullaniciAdi";
            this.txtKullaniciAdi.Size = new System.Drawing.Size(320, 23);
            this.txtKullaniciAdi.TabIndex = 2;
            //
            // lblEposta
            //
            this.lblEposta.AutoSize = true;
            this.lblEposta.Location = new System.Drawing.Point(48, 130);
            this.lblEposta.Name = "lblEposta";
            this.lblEposta.Size = new System.Drawing.Size(45, 15);
            this.lblEposta.TabIndex = 3;
            this.lblEposta.Text = "E-posta";
            //
            // txtEposta
            //
            this.txtEposta.Location = new System.Drawing.Point(51, 150);
            this.txtEposta.MaxLength = 150;
            this.txtEposta.Name = "txtEposta";
            this.txtEposta.Size = new System.Drawing.Size(320, 23);
            this.txtEposta.TabIndex = 4;
            //
            // lblSifre
            //
            this.lblSifre.AutoSize = true;
            this.lblSifre.Location = new System.Drawing.Point(48, 182);
            this.lblSifre.Name = "lblSifre";
            this.lblSifre.Size = new System.Drawing.Size(30, 15);
            this.lblSifre.TabIndex = 5;
            this.lblSifre.Text = "Şifre";
            //
            // txtSifre
            //
            this.txtSifre.Location = new System.Drawing.Point(51, 202);
            this.txtSifre.MaxLength = 100;
            this.txtSifre.Name = "txtSifre";
            this.txtSifre.Size = new System.Drawing.Size(320, 23);
            this.txtSifre.TabIndex = 6;
            this.txtSifre.UseSystemPasswordChar = true;
            //
            // lblSifreTekrar
            //
            this.lblSifreTekrar.AutoSize = true;
            this.lblSifreTekrar.Location = new System.Drawing.Point(48, 234);
            this.lblSifreTekrar.Name = "lblSifreTekrar";
            this.lblSifreTekrar.Size = new System.Drawing.Size(70, 15);
            this.lblSifreTekrar.TabIndex = 7;
            this.lblSifreTekrar.Text = "Şifre (tekrar)";
            //
            // txtSifreTekrar
            //
            this.txtSifreTekrar.Location = new System.Drawing.Point(51, 254);
            this.txtSifreTekrar.MaxLength = 100;
            this.txtSifreTekrar.Name = "txtSifreTekrar";
            this.txtSifreTekrar.Size = new System.Drawing.Size(320, 23);
            this.txtSifreTekrar.TabIndex = 8;
            this.txtSifreTekrar.UseSystemPasswordChar = true;
            //
            // lblSifreKural
            //
            this.lblSifreKural.ForeColor = System.Drawing.Color.Gray;
            this.lblSifreKural.Location = new System.Drawing.Point(48, 282);
            this.lblSifreKural.Name = "lblSifreKural";
            this.lblSifreKural.Size = new System.Drawing.Size(330, 32);
            this.lblSifreKural.TabIndex = 9;
            this.lblSifreKural.Text = "En az 8 karakter; en az bir büyük harf, bir küçük harf ve bir rakam.";
            //
            // btnKayitOl
            //
            this.btnKayitOl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.btnKayitOl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKayitOl.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.btnKayitOl.ForeColor = System.Drawing.Color.White;
            this.btnKayitOl.Location = new System.Drawing.Point(51, 322);
            this.btnKayitOl.Name = "btnKayitOl";
            this.btnKayitOl.Size = new System.Drawing.Size(200, 36);
            this.btnKayitOl.TabIndex = 10;
            this.btnKayitOl.Text = "Hesap Oluştur";
            this.btnKayitOl.UseVisualStyleBackColor = false;
            this.btnKayitOl.Click += new System.EventHandler(this.btnKayitOl_Click);
            //
            // btnVazgec
            //
            this.btnVazgec.Location = new System.Drawing.Point(261, 322);
            this.btnVazgec.Name = "btnVazgec";
            this.btnVazgec.Size = new System.Drawing.Size(110, 36);
            this.btnVazgec.TabIndex = 11;
            this.btnVazgec.Text = "Vazgeç";
            this.btnVazgec.UseVisualStyleBackColor = true;
            this.btnVazgec.Click += new System.EventHandler(this.btnVazgec_Click);
            //
            // FrmKayit
            //
            this.AcceptButton = this.btnKayitOl;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnVazgec;
            this.ClientSize = new System.Drawing.Size(424, 382);
            this.Controls.Add(this.btnVazgec);
            this.Controls.Add(this.btnKayitOl);
            this.Controls.Add(this.lblSifreKural);
            this.Controls.Add(this.txtSifreTekrar);
            this.Controls.Add(this.lblSifreTekrar);
            this.Controls.Add(this.txtSifre);
            this.Controls.Add(this.lblSifre);
            this.Controls.Add(this.txtEposta);
            this.Controls.Add(this.lblEposta);
            this.Controls.Add(this.txtKullaniciAdi);
            this.Controls.Add(this.lblKullaniciAdi);
            this.Controls.Add(this.pnlUst);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmKayit";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Kayıt Ol";
            this.pnlUst.ResumeLayout(false);
            this.pnlUst.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlUst;
        private System.Windows.Forms.Label lblBaslik;
        private System.Windows.Forms.Label lblKullaniciAdi;
        private System.Windows.Forms.TextBox txtKullaniciAdi;
        private System.Windows.Forms.Label lblEposta;
        private System.Windows.Forms.TextBox txtEposta;
        private System.Windows.Forms.Label lblSifre;
        private System.Windows.Forms.TextBox txtSifre;
        private System.Windows.Forms.Label lblSifreTekrar;
        private System.Windows.Forms.TextBox txtSifreTekrar;
        private System.Windows.Forms.Label lblSifreKural;
        private System.Windows.Forms.Button btnKayitOl;
        private System.Windows.Forms.Button btnVazgec;
    }
}
