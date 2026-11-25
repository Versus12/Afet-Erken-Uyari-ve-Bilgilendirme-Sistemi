using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using afet_erken_uyarı_ve_bilgilendirme_sistemi.Entities;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Business
{
    /// <summary>
    /// Hafta 11 - Konum hesapları.
    /// İki nokta arasındaki kuş uçuşu mesafe Haversine formülüyle bulunur:
    ///   a = sin²(Δφ/2) + cos φ1 · cos φ2 · sin²(Δλ/2)
    ///   d = 2 · R · atan2(√a, √(1−a))        (R = 6371 km, φ = enlem, λ = boylam, radyan)
    /// Dünya'nın eğriliğini hesaba kattığı için düz (Pisagor) hesaptan daha doğrudur.
    /// </summary>
    public static class KonumHesap
    {
        public const double DunyaYaricapiKm = 6371.0;

        /// <summary>Ortalama yürüme hızı (km/sa); yürüme süresi tahmini için.</summary>
        public const double YurumeHiziKmSa = 4.5;

        public static double MesafeKm(double enlem1, double boylam1, double enlem2, double boylam2)
        {
            double fi1 = Radyan(enlem1), fi2 = Radyan(enlem2);
            double dFi = Radyan(enlem2 - enlem1), dLambda = Radyan(boylam2 - boylam1);
            double a = Math.Sin(dFi / 2) * Math.Sin(dFi / 2) +
                       Math.Cos(fi1) * Math.Cos(fi2) * Math.Sin(dLambda / 2) * Math.Sin(dLambda / 2);
            return 2 * DunyaYaricapiKm * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        public static double MesafeKm(decimal enlem1, decimal boylam1, decimal enlem2, decimal boylam2)
        {
            return MesafeKm((double)enlem1, (double)boylam1, (double)enlem2, (double)boylam2);
        }

        /// <summary>Kuş uçuşu mesafeye göre kaba yürüme süresi (dakika). Gerçek yol biraz daha uzundur.</summary>
        public static int YurumeDakika(double km)
        {
            return (int)Math.Ceiling(km / YurumeHiziKmSa * 60);
        }

        public static void Dogrula(decimal enlem, decimal boylam)
        {
            if (enlem < -90 || enlem > 90) throw new IsKuraliHatasi("Enlem -90 ile 90 arasında olmalıdır.");
            if (boylam < -180 || boylam > 180) throw new IsKuraliHatasi("Boylam -180 ile 180 arasında olmalıdır.");
        }

        private static double Radyan(double derece) { return derece * Math.PI / 180.0; }
    }

    /// <summary>
    /// Masaüstü uygulamasındaki harita için HTML sayfası üretir (Leaflet + OpenStreetMap).
    /// Sayfa WebView2 bileşeninde gösterilir; web sitesi aynı Leaflet kütüphanesini kullanır.
    /// </summary>
    public static class HaritaSayfasi
    {
        private static readonly CultureInfo Ic = CultureInfo.InvariantCulture;

        public static string Olustur(IEnumerable<ToplanmaAlani> alanlar, decimal? enlem, decimal? boylam,
                                     IList<YakinAlan> enYakinlar, string konumAciklamasi)
        {
            HashSet<int> yakinIdler = new HashSet<int>((enYakinlar ?? new List<YakinAlan>()).Select(y => y.Alan.AlanId));
            int? ilk = enYakinlar != null && enYakinlar.Count > 0 ? enYakinlar[0].Alan.AlanId : (int?)null;

            StringBuilder js = new StringBuilder("[");
            foreach (ToplanmaAlani t in alanlar.Where(a => a.Enlem.HasValue && a.Boylam.HasValue))
            {
                YakinAlan y = enYakinlar == null ? null : enYakinlar.FirstOrDefault(k => k.Alan.AlanId == t.AlanId);
                js.Append("{ad:").Append(JsMetin(t.AlanAdi))
                  .Append(",bolge:").Append(JsMetin(t.BolgeAdi))
                  .Append(",kapasite:").Append(t.Kapasite)
                  .Append(",e:").Append(t.Enlem.Value.ToString(Ic))
                  .Append(",b:").Append(t.Boylam.Value.ToString(Ic))
                  .Append(",yakin:").Append(yakinIdler.Contains(t.AlanId) ? "true" : "false")
                  .Append(",ilk:").Append(ilk == t.AlanId ? "true" : "false")
                  .Append(",km:").Append(y == null ? "null" : y.MesafeKm.ToString("0.00", Ic))
                  .Append("},");
            }
            js.Append("]");

            string konum = enlem.HasValue && boylam.HasValue
                ? "{e:" + enlem.Value.ToString(Ic) + ",b:" + boylam.Value.ToString(Ic) + ",ad:" + JsMetin(konumAciklamasi ?? "Konumunuz") + "}"
                : "null";

            return @"<!DOCTYPE html>
<html lang=""tr""><head><meta charset=""utf-8"">
<meta name=""viewport"" content=""width=device-width, initial-scale=1"">
<link rel=""stylesheet"" href=""https://unpkg.com/leaflet@1.9.4/dist/leaflet.css"">
<script src=""https://unpkg.com/leaflet@1.9.4/dist/leaflet.js""></script>
<style>html,body,#harita{height:100%;margin:0} .yakin{font-weight:bold;color:#c0392b}</style>
</head><body><div id=""harita""></div>
<script>
var alanlar = " + js + @";
var konum = " + konum + @";
var harita = L.map('harita');
L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', { maxZoom: 19,
  attribution: '&copy; OpenStreetMap katkıcıları' }).addTo(harita);
var sinirlar = [], ilkIsaret = null;
alanlar.forEach(function (a) {
  var renk = a.ilk ? '#c0392b' : (a.yakin ? '#e67e22' : '#2c3e50');
  var m = L.circleMarker([a.e, a.b], { radius: a.ilk ? 11 : 8, color: renk, fillColor: renk, fillOpacity: 0.85 }).addTo(harita);
  m.bindPopup('<b>' + a.ad + '</b><br>' + a.bolge + '<br>Kapasite: ' + a.kapasite.toLocaleString('tr-TR') +
              (a.km !== null ? '<br><span class=""yakin"">' + a.km.toFixed(2).replace('.', ',') + ' km</span>' : ''));
  if (a.ilk) ilkIsaret = m;
  sinirlar.push([a.e, a.b]);
});
if (konum) {
  L.marker([konum.e, konum.b]).addTo(harita).bindTooltip(konum.ad, { permanent: true, direction: 'top' });
  alanlar.filter(function (a) { return a.ilk; }).forEach(function (a) {
    L.polyline([[konum.e, konum.b], [a.e, a.b]], { color: '#c0392b', dashArray: '6 6' }).addTo(harita);
  });
  var odak = [[konum.e, konum.b]];
  alanlar.filter(function (a) { return a.yakin; }).forEach(function (a) { odak.push([a.e, a.b]); });
  harita.fitBounds(odak, { padding: [40, 40], maxZoom: 15 });
} else if (sinirlar.length) {
  harita.fitBounds(sinirlar, { padding: [30, 30], maxZoom: 13 });
} else {
  harita.setView([39.0, 35.0], 6);
}
if (ilkIsaret) ilkIsaret.openPopup();
</script></body></html>";
        }

        /// <summary>Metni JavaScript dizesine güvenli çevirir (tırnak, ters bölü, &lt; karakterleri kaçırılır).</summary>
        private static string JsMetin(string s)
        {
            StringBuilder sb = new StringBuilder("'");
            foreach (char c in s ?? "")
            {
                if (c == '\'' || c == '\\') sb.Append('\\').Append(c);
                else if (c == '<') sb.Append("\\u003c");
                else if (c == '\n' || c == '\r') sb.Append(' ');
                else sb.Append(c);
            }
            return sb.Append("'").ToString();
        }
    }
}
