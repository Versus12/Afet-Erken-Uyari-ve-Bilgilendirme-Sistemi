
using System;
using System.Windows.Forms;

namespace afet_erken_uyari_ve_bilgilendirme_sistemi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            VeritabaniBaglanti db = new VeritabaniBaglanti();

            db.BaglantiTestEt();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
