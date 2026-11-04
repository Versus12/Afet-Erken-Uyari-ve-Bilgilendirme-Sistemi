using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities
{
    internal class Abonelik
    {
        public int AbonelikId { get; set; }

        public string AboneAdi { get; set; } = string.Empty;

        public string Telefon { get; set; } = string.Empty;

        public int BolgeId { get; set; }
    }
}
