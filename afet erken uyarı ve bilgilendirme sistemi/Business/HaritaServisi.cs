using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Business
{
    /// <summary>Adresten bulunan konum.</summary>
    public class KonumSonucu
    {
        public decimal Enlem { get; set; }
        public decimal Boylam { get; set; }
        public string BulunanAdres { get; set; }
    }

    /// <summary>
    /// Harita API entegrasyonu (Hafta 9): OpenStreetMap Nominatim ile adresten koordinat bulma (geocoding).
    /// GET https://nominatim.openstreetmap.org/search?q=...&amp;format=json&amp;limit=1&amp;countrycodes=tr
    /// - Anahtar gerektirmez; kullanım kuralı gereği uygulamayı tanıtan bir User-Agent gönderilir.
    /// - Hız sınırı: saniyede en fazla 1 istek (kurala uymak için istekler arasında 1,1 sn beklenir).
    /// - Zaman aşımı 10 sn; ağ/servis hataları kullanıcıya anlaşılır mesajla gösterilir.
    /// - Farklı bir Nominatim sunucusu AFET_HARITA_URL ortam değişkeniyle verilebilir.
    /// </summary>
    public class HaritaServisi
    {
        private const string VarsayilanAdres = "https://nominatim.openstreetmap.org";
        private static readonly HttpClient Http = OlusturHttp();
        private static readonly HizSinirlayici Sinirlayici = new HizSinirlayici(TimeSpan.FromMilliseconds(1100));

        private static HttpClient OlusturHttp()
        {
            HttpClient h = GuvenliHttp.Olustur(TimeSpan.FromSeconds(10));
            h.DefaultRequestHeaders.UserAgent.ParseAdd("AfetErkenUyari-P14/0.9 (ogrenci projesi)");
            h.DefaultRequestHeaders.AcceptLanguage.ParseAdd("tr");
            return h;
        }

        private static string SunucuAdresi()
        {
            string s = Environment.GetEnvironmentVariable("AFET_HARITA_URL");
            return string.IsNullOrWhiteSpace(s) ? VarsayilanAdres : s.Trim().TrimEnd('/');
        }

        /// <summary>Adresi koordinata çevirir. Bulunamazsa IsKuraliHatasi fırlatır.</summary>
        public KonumSonucu AdresBul(string adres)
        {
            Oturum.GirisGerekli();
            adres = (adres ?? "").Trim();
            if (adres.Length < 5) throw new IsKuraliHatasi("Konum aramak için önce adresi (en az 5 karakter) yazınız.");

            string url = SunucuAdresi() + "/search?format=json&limit=1&countrycodes=tr&q=" + Uri.EscapeDataString(adres);

            Sinirlayici.Bekle();
            HttpResponseMessage yanit;
            try
            {
                yanit = Task.Run(() => Http.GetAsync(url)).GetAwaiter().GetResult();
            }
            catch (TaskCanceledException)
            {
                throw new IsKuraliHatasi("Harita servisi 10 saniye içinde yanıt vermedi. Daha sonra tekrar deneyiniz.");
            }
            catch (HttpRequestException)
            {
                throw new IsKuraliHatasi("Harita servisine ulaşılamadı. İnternet bağlantınızı kontrol ediniz.");
            }

            using (yanit)
            {
                if ((int)yanit.StatusCode == 429)
                    throw new IsKuraliHatasi("Harita servisine çok sık istek gönderildi. Birkaç saniye bekleyip tekrar deneyiniz.");
                if (!yanit.IsSuccessStatusCode)
                    throw new IsKuraliHatasi("Harita servisi hata verdi (" + (int)yanit.StatusCode + ").");

                string icerik = yanit.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return Ayristir(icerik, adres);
            }
        }

        /// <summary>
        /// Yanıtı ayrıştırır. Örnek yanıt:
        /// [{"lat":"38.8847","lon":"40.4939","display_name":"Bingöl, Türkiye", ...}]
        /// </summary>
        public static KonumSonucu Ayristir(string json, string aranan)
        {
            List<Dictionary<string, object>> sonuclar;
            try
            {
                sonuclar = new JavaScriptSerializer().Deserialize<List<Dictionary<string, object>>>(json);
            }
            catch (ArgumentException)
            {
                throw new IsKuraliHatasi("Harita servisinden beklenmeyen bir yanıt geldi.");
            }
            catch (InvalidOperationException)
            {
                throw new IsKuraliHatasi("Harita servisinden beklenmeyen bir yanıt geldi.");
            }

            if (sonuclar == null || sonuclar.Count == 0)
                throw new IsKuraliHatasi("\"" + aranan + "\" için konum bulunamadı. Adresi il/ilçe ekleyerek tekrar deneyiniz.");

            Dictionary<string, object> ilk = sonuclar[0];
            decimal enlem, boylam;
            if (!ilk.ContainsKey("lat") || !ilk.ContainsKey("lon") ||
                !decimal.TryParse(Convert.ToString(ilk["lat"], CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture, out enlem) ||
                !decimal.TryParse(Convert.ToString(ilk["lon"], CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture, out boylam))
                throw new IsKuraliHatasi("Harita servisinin yanıtında koordinat bulunamadı.");

            return new KonumSonucu
            {
                Enlem = Math.Round(enlem, 6),
                Boylam = Math.Round(boylam, 6),
                BulunanAdres = ilk.ContainsKey("display_name") ? Convert.ToString(ilk["display_name"]) : aranan
            };
        }

        /// <summary>Koordinatı OpenStreetMap'te işaretli gösteren bağlantı.</summary>
        public static string HaritaBaglantisi(decimal enlem, decimal boylam)
        {
            string e = enlem.ToString("0.######", CultureInfo.InvariantCulture);
            string b = boylam.ToString("0.######", CultureInfo.InvariantCulture);
            return "https://www.openstreetmap.org/?mlat=" + e + "&mlon=" + b + "#map=17/" + e + "/" + b;
        }
    }
}
