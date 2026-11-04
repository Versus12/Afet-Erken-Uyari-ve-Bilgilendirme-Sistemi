using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities
{
    internal class uyari
    {
        public int UyariId { get; set; }

        public string UyariBasligi { get; set; } = string.Empty;

        public string UyariMesaji { get; set; } = string.Empty;

        public string UyariSeviyesi { get; set; } = string.Empty;

        public DateTime UyariTarihi { get; set; }

        public int BolgeId { get; set; }
    }
}
