using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Business;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi
{
    /// <summary>
    /// Sunum katmanındaki formların ortak kullandığı yardımcılar:
    /// hata yönetimi, mesaj kutuları, DataGridView ayarları, sayı okuma.
    /// </summary>
    public static class ArayuzYardimci
    {
        public static readonly Color KoyuRenk = Color.FromArgb(44, 62, 80);

        /// <summary>
        /// Bir arayüz işlemini çalıştırır ve hataları türüne göre kullanıcıya gösterir.
        /// İşlem başarılıysa true döner.
        /// </summary>
        public static bool Calistir(Form form, Action islem)
        {
            Cursor eski = form != null ? form.Cursor : Cursors.Default;
            try
            {
                if (form != null) form.Cursor = Cursors.WaitCursor;
                islem();
                return true;
            }
            catch (IsKuraliHatasi ex)
            {
                Uyar(ex.Message);
            }
            catch (YetkisizErisimHatasi ex)
            {
                MessageBox.Show(ex.Message, "Yetki", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                // Oturum düştüyse alt formu kapat; ana menü girişe yönlendirir.
                if (!Oturum.OturumAcikMi && form != null && form.Modal) form.Close();
            }
            catch (SqlException ex)
            {
                HataKaydet(ex);
                Hata("Veritabanı hatası oluştu. Bağlantıyı kontrol ediniz.\n\nAyrıntı: " + ex.Message);
            }
            catch (Exception ex)
            {
                HataKaydet(ex);
                Hata("Beklenmeyen bir hata oluştu:\n" + ex.Message);
            }
            finally
            {
                if (form != null) form.Cursor = eski;
            }
            return false;
        }

        public static void Bilgi(string mesaj)
        {
            MessageBox.Show(mesaj, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void Uyar(string mesaj)
        {
            MessageBox.Show(mesaj, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void Hata(string mesaj)
        {
            MessageBox.Show(mesaj, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static bool Onayla(string mesaj)
        {
            return MessageBox.Show(mesaj, "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                   == DialogResult.Yes;
        }

        /// <summary>Teknik hataları kullanıcının AppData klasöründeki günlük dosyasına yazar.</summary>
        public static void HataKaydet(Exception ex)
        {
            try
            {
                string klasor = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AfetErkenUyari");
                Directory.CreateDirectory(klasor);
                File.AppendAllText(Path.Combine(klasor, "hata.log"),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + ex + Environment.NewLine + Environment.NewLine);
            }
            catch
            {
                // Günlük yazılamazsa uygulamayı durdurmayalım.
            }
        }

        // ------------------------------------------------------------------ DataGridView

        public static void IzgaraHazirla(DataGridView dgv)
        {
            dgv.AutoGenerateColumns = false;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.MultiSelect = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowHeadersVisible = false;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.FixedSingle;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = KoyuRenk;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgv.Columns.Clear();
        }

        /// <summary>Izgaraya sütun ekler. genislik 0 ise sütun kalan alanı doldurur.</summary>
        public static void Kolon(DataGridView dgv, string ozellik, string baslik, int genislik, string format = null)
        {
            DataGridViewTextBoxColumn k = new DataGridViewTextBoxColumn
            {
                DataPropertyName = ozellik,
                Name = "col" + ozellik,
                HeaderText = baslik
            };
            if (genislik > 0) k.Width = genislik;
            else k.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (format != null) k.DefaultCellStyle.Format = format;
            dgv.Columns.Add(k);
        }

        /// <summary>Seçili satırın bağlı olduğu nesneyi döndürür (yoksa null).</summary>
        public static T SeciliNesne<T>(DataGridView dgv) where T : class
        {
            return dgv.CurrentRow == null ? null : dgv.CurrentRow.DataBoundItem as T;
        }

        /// <summary>Uyarı seviyesine göre satır rengi.</summary>
        public static Color SeviyeRengi(int seviye)
        {
            switch (seviye)
            {
                case 4: return Color.FromArgb(250, 200, 200); // Acil
                case 3: return Color.FromArgb(253, 223, 190); // Tehlike
                case 2: return Color.FromArgb(255, 243, 191); // Dikkat
                default: return Color.White;                  // Bilgi
            }
        }

        // ------------------------------------------------------------------ Sayı okuma

        /// <summary>Boş ise null döner; "38,88" ve "38.88" biçimlerini kabul eder.</summary>
        public static decimal? OndalikOku(TextBox txt, string alanAdi)
        {
            string metin = txt.Text.Trim();
            if (metin.Length == 0) return null;
            decimal deger;
            if (!decimal.TryParse(metin.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out deger))
            {
                txt.Focus();
                throw new IsKuraliHatasi(alanAdi + " geçerli bir sayı olmalıdır.");
            }
            return Math.Round(deger, 6);
        }

        public static string OndalikYaz(decimal? deger)
        {
            return deger.HasValue ? deger.Value.ToString("0.######", CultureInfo.InvariantCulture) : "";
        }
    }
}
