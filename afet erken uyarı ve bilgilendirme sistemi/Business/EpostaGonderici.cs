using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Business
{
    /// <summary>
    /// Harici HTTPS servisleri için ortak HttpClient oluşturucu.
    /// .NET Framework bazı durumlarda eski TLS sürümlerini varsayılan alır; servisler (Brevo, OpenStreetMap)
    /// TLS 1.2 ve üstünü istediği için TLS 1.2 açıkça etkinleştirilir.
    /// </summary>
    internal static class GuvenliHttp
    {
        public static HttpClient Olustur(TimeSpan zamanAsimi)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            return new HttpClient { Timeout = zamanAsimi };
        }
    }

    /// <summary>Gönderilecek tek bir e-posta.</summary>
    public class EpostaMesaji
    {
        public string Alici { get; set; }
        public string Konu { get; set; }
        public string Govde { get; set; }
    }

    /// <summary>
    /// E-posta gönderim hatası. <see cref="TekrarDenenebilir"/> true ise (zaman aşımı, 429, 5xx, ağ hatası)
    /// gönderim birkaç kez daha denenir; false ise (geçersiz adres, 400) hemen "Başarısız" yazılır.
    /// <see cref="YapilandirmaHatasi"/> true ise (yanlış API anahtarı - 401/403) kalan gönderimler de durdurulur.
    /// </summary>
    public class BildirimGonderimHatasi : Exception
    {
        public bool TekrarDenenebilir { get; private set; }
        public bool YapilandirmaHatasi { get; private set; }
        public TimeSpan? Bekleme { get; private set; }

        public BildirimGonderimHatasi(string mesaj, bool tekrarDenenebilir, bool yapilandirmaHatasi = false,
                                      TimeSpan? bekleme = null) : base(mesaj)
        {
            TekrarDenenebilir = tekrarDenenebilir;
            YapilandirmaHatasi = yapilandirmaHatasi;
            Bekleme = bekleme;
        }
    }

    /// <summary>E-posta gönderen servislerin ortak arayüzü (gerçek API veya deneme modu).</summary>
    public interface IEpostaGonderici
    {
        /// <summary>"Brevo API" / "Deneme modu" gibi ekranda gösterilecek ad.</summary>
        string Ad { get; }

        /// <summary>Gerçekten gönderiyorsa true; deneme modunda false.</summary>
        bool GercekGonderim { get; }

        /// <summary>E-postayı gönderir, servisin verdiği mesaj numarasını döndürür. Hata olursa BildirimGonderimHatasi fırlatır.</summary>
        string Gonder(EpostaMesaji mesaj);
    }

    /// <summary>
    /// Ortam değişkenlerine bakarak hangi göndericinin kullanılacağını seçer.
    /// API anahtarı KODA ve DEPOYA yazılmaz; yalnızca ortam değişkeninden okunur.
    ///   AFET_EPOSTA_API_ANAHTARI : Brevo API anahtarı (yoksa deneme modu)
    ///   AFET_EPOSTA_GONDEREN     : Brevo'da doğrulanmış gönderen adresi
    /// </summary>
    public static class EpostaGondericiSecici
    {
        public const string AnahtarDegiskeni = "AFET_EPOSTA_API_ANAHTARI";
        public const string GonderenDegiskeni = "AFET_EPOSTA_GONDEREN";

        public static IEpostaGonderici Olustur()
        {
            string anahtar = OrtamDegiskeni(AnahtarDegiskeni);
            string gonderen = OrtamDegiskeni(GonderenDegiskeni);
            if (!string.IsNullOrEmpty(anahtar) && !string.IsNullOrEmpty(gonderen))
                return new BrevoEpostaGonderici(anahtar, gonderen);
            return new DenemeModuEpostaGonderici();
        }

        /// <summary>Önce işlem, sonra kullanıcı düzeyindeki ortam değişkenine bakar.</summary>
        private static string OrtamDegiskeni(string ad)
        {
            string deger = Environment.GetEnvironmentVariable(ad);
            if (string.IsNullOrWhiteSpace(deger))
                deger = Environment.GetEnvironmentVariable(ad, EnvironmentVariableTarget.User);
            return string.IsNullOrWhiteSpace(deger) ? null : deger.Trim();
        }
    }

    /// <summary>
    /// Brevo (eski adıyla Sendinblue) REST API ile gerçek e-posta gönderimi.
    /// POST https://api.brevo.com/v3/smtp/email  — kimlik doğrulama "api-key" başlığıyla yapılır.
    /// Başarılı yanıt: 201 {"messageId":"..."}
    /// </summary>
    public class BrevoEpostaGonderici : IEpostaGonderici
    {
        private const string ApiAdresi = "https://api.brevo.com/v3/smtp/email";

        // HttpClient bir kez oluşturulup tekrar kullanılır (her istekte yeni bağlantı açmamak için).
        private static readonly HttpClient Http = GuvenliHttp.Olustur(TimeSpan.FromSeconds(10));

        private readonly string _anahtar;
        private readonly string _gonderen;

        public BrevoEpostaGonderici(string anahtar, string gonderen)
        {
            _anahtar = anahtar;
            _gonderen = gonderen;
        }

        public string Ad { get { return "Brevo e-posta API"; } }
        public bool GercekGonderim { get { return true; } }

        public string Gonder(EpostaMesaji mesaj)
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            string govde = json.Serialize(new
            {
                sender = new { name = "Afet Erken Uyarı", email = _gonderen },
                to = new[] { new { email = mesaj.Alici } },
                subject = mesaj.Konu,
                textContent = mesaj.Govde
            });

            HttpRequestMessage istek = new HttpRequestMessage(HttpMethod.Post, ApiAdresi)
            {
                Content = new StringContent(govde, Encoding.UTF8, "application/json")
            };
            istek.Headers.Add("api-key", _anahtar);
            istek.Headers.Add("Accept", "application/json");

            HttpResponseMessage yanit;
            try
            {
                // Arayüz iş parçacığını kilitlememek için istek ayrı bir görevde beklenir.
                yanit = Task.Run(() => Http.SendAsync(istek)).GetAwaiter().GetResult();
            }
            catch (TaskCanceledException)
            {
                throw new BildirimGonderimHatasi("E-posta servisi 10 saniye içinde yanıt vermedi (zaman aşımı).", true);
            }
            catch (HttpRequestException ex)
            {
                throw new BildirimGonderimHatasi("E-posta servisine bağlanılamadı: " + ex.Message, true);
            }

            using (yanit)
            {
                string icerik = yanit.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                int kod = (int)yanit.StatusCode;

                if (yanit.IsSuccessStatusCode)
                {
                    // Yanıtı ayrıştır: {"messageId":"<...>"}
                    Dictionary<string, object> veri = json.Deserialize<Dictionary<string, object>>(icerik);
                    object id;
                    return veri != null && veri.TryGetValue("messageId", out id) ? Convert.ToString(id) : "(numara yok)";
                }
                if (kod == 401 || kod == 403)
                    throw new BildirimGonderimHatasi("API anahtarı geçersiz veya yetkisiz (" + kod + ").", false, true);
                if (kod == 429)
                {
                    TimeSpan? bekle = yanit.Headers.RetryAfter != null ? yanit.Headers.RetryAfter.Delta : null;
                    throw new BildirimGonderimHatasi("Hız sınırı aşıldı (429).", true, false, bekle);
                }
                if (kod >= 500)
                    throw new BildirimGonderimHatasi("E-posta servisi hata verdi (" + kod + ").", true);

                throw new BildirimGonderimHatasi("Gönderim reddedildi (" + kod + "): " + Kisalt(icerik), false);
            }
        }

        private static string Kisalt(string s)
        {
            return s == null ? "" : s.Length > 200 ? s.Substring(0, 200) : s;
        }
    }

    /// <summary>
    /// API anahtarı tanımlı değilken kullanılan deneme modu: e-posta gönderilmez,
    /// .eml dosyası olarak %LOCALAPPDATA%\AfetErkenUyari\GidenEpostalar klasörüne yazılır.
    /// Dosya çift tıklanınca Outlook / Posta uygulamasında açılır.
    /// </summary>
    public class DenemeModuEpostaGonderici : IEpostaGonderici
    {
        public static string Klasor
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                    "AfetErkenUyari", "GidenEpostalar");
            }
        }

        public string Ad { get { return "Deneme modu - dosyaya yazılır"; } }
        public bool GercekGonderim { get { return false; } }

        public string Gonder(EpostaMesaji mesaj)
        {
            try
            {
                Directory.CreateDirectory(Klasor);
                DateTime once = DateTime.Now.AddSeconds(-1);
                using (MailMessage m = new MailMessage("bildirim@afet-uyari.local", mesaj.Alici, mesaj.Konu, mesaj.Govde))
                using (SmtpClient smtp = new SmtpClient())
                {
                    m.BodyEncoding = Encoding.UTF8;
                    m.SubjectEncoding = Encoding.UTF8;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtp.PickupDirectoryLocation = Klasor;
                    smtp.Send(m);
                }
                string dosya = SonDosya(once);
                return dosya == null ? "deneme" : "dosya:" + dosya;
            }
            catch (Exception ex) when (ex is IOException || ex is SmtpException || ex is UnauthorizedAccessException)
            {
                throw new BildirimGonderimHatasi("Deneme e-postası dosyaya yazılamadı: " + ex.Message, false);
            }
            catch (FormatException)
            {
                throw new BildirimGonderimHatasi("Alıcı e-posta adresi geçersiz.", false);
            }
        }

        private static string SonDosya(DateTime sonra)
        {
            string sonuc = null;
            DateTime enYeni = sonra;
            foreach (string f in Directory.GetFiles(Klasor, "*.eml"))
            {
                DateTime t = File.GetCreationTime(f);
                if (t >= enYeni) { enYeni = t; sonuc = Path.GetFileName(f); }
            }
            return sonuc;
        }
    }

    /// <summary>
    /// Basit hız sınırlayıcı: aynı servise art arda giden istekler arasında en az belirli bir süre bırakır.
    /// (Harici servislerin "saniyede en fazla N istek" kuralına uymak için.)
    /// </summary>
    public class HizSinirlayici
    {
        private readonly TimeSpan _enAzAralik;
        private readonly object _kilit = new object();
        private DateTime _sonIstek = DateTime.MinValue;

        public HizSinirlayici(TimeSpan enAzAralik)
        {
            _enAzAralik = enAzAralik;
        }

        public void Bekle()
        {
            lock (_kilit)
            {
                TimeSpan gecen = DateTime.UtcNow - _sonIstek;
                if (gecen < _enAzAralik) Thread.Sleep(_enAzAralik - gecen);
                _sonIstek = DateTime.UtcNow;
            }
        }
    }
}
