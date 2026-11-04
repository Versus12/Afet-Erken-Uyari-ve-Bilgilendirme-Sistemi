using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities
{
    internal class ToplanmaAlani
    {
        public int ToplanmaAlaniId { get; set; }

        public string AlanAdi { get; set; } = string.Empty;

        public string Adres { get; set; } = string.Empty;

        public int Kapasite { get; set; }

        public int BolgeId { get; set; }
    }
}
