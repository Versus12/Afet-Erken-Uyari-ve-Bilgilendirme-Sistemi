using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Anthropic;
using Anthropic.Exceptions;
using Anthropic.Models.Beta.Messages;

namespace afet_erken_uyarı_ve_bilgilendirme_sistemi.Business
{
    /// <summary>Yapay zekâ çağrısı başarısız olduğunda fırlatılır; mesajı kullanıcıya gösterilebilir.</summary>
    public class YapayZekaHatasi : Exception
    {
        public YapayZekaHatasi(string mesaj) : base(mesaj) { }
    }

    /// <summary>
    /// Bir dil modeline sistem + kullanıcı istemi gönderip JSON metni alan servis.
    /// (Testte sahte bir istemci verilebilsin diye arayüz olarak tanımlandı.)
    /// </summary>
    public interface IYapayZekaIstemcisi
    {
        /// <summary>Ekranda ve günlükte görünen ad, örn. "Claude (claude-opus-5-5)".</summary>
        string Ad { get; }

        /// <summary>Modelin döndürdüğü JSON metni. Hata olursa YapayZekaHatasi fırlatır.</summary>
        string Tamamla(string sistemIstemi, string kullaniciIstemi, IDictionary<string, JsonElement> jsonSemasi);
    }

    /// <summary>
    /// Anthropic'in resmî C# SDK'sı ile Claude çağrısı (Hafta 10).
    /// - API anahtarı koda yazılmaz: ANTHROPIC_API_KEY ortam değişkeninden okunur.
    /// - Yapılandırılmış çıktı (JSON şeması) ile cevabın biçimi garanti edilir; içerik yine de doğrulanır.
    /// - Güvenlik sınıflandırıcısı reddederse sunucu tarafı yedek model devreye girer (fallbacks: default).
    /// - SDK geçici hatalarda (429, 5xx, bağlantı) kendisi 2 kez yeniden dener; zaman aşımı 60 sn.
    /// </summary>
    public class ClaudeIstemcisi : IYapayZekaIstemcisi
    {
        public const string AnahtarDegiskeni = "ANTHROPIC_API_KEY";
        public const string Model = "claude-opus-5-5";

        private readonly AnthropicClient _istemci;

        public ClaudeIstemcisi(string apiAnahtari)
        {
            // .NET Framework'te TLS 1.2 açıkça etkinleştirilir (bkz. GuvenliHttp, Hafta 9).
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            _istemci = new AnthropicClient
            {
                ApiKey = apiAnahtari,
                Timeout = TimeSpan.FromSeconds(60),
                MaxRetries = 2
            };
        }

        public string Ad { get { return "Claude (" + Model + ")"; } }

        /// <summary>Anahtar tanımlıysa istemci oluşturur, değilse null döner (kural tabanlı yöntem kullanılır).</summary>
        public static ClaudeIstemcisi OrtamdanOlustur()
        {
            string anahtar = Environment.GetEnvironmentVariable(AnahtarDegiskeni);
            if (string.IsNullOrWhiteSpace(anahtar))
                anahtar = Environment.GetEnvironmentVariable(AnahtarDegiskeni, EnvironmentVariableTarget.User);
            return string.IsNullOrWhiteSpace(anahtar) ? null : new ClaudeIstemcisi(anahtar.Trim());
        }

        public string Tamamla(string sistemIstemi, string kullaniciIstemi, IDictionary<string, JsonElement> jsonSemasi)
        {
            var istek = new MessageCreateParams
            {
                Model = Model,
                MaxTokens = 16000,
                System = sistemIstemi,
                Thinking = new BetaThinkingConfigAdaptive(),
                OutputConfig = new BetaOutputConfig
                {
                    Effort = Effort.Medium,
                    Format = new BetaJsonOutputFormat { Schema = new Dictionary<string, JsonElement>(jsonSemasi) }
                },
                // Güvenlik sınıflandırıcısı reddederse istek sunucuda uygun yedek modelle yeniden yanıtlanır.
                Betas = ["server-side-fallback-2026-07-01"],
                Fallbacks = new Default(),
                Messages = new List<BetaMessageParam> { new BetaUserMessageParam { Content = kullaniciIstemi } }
            };

            BetaMessage yanit;
            try
            {
                // WinForms arayüz iş parçacığını kilitlememek için çağrı ayrı görevde beklenir.
                yanit = Task.Run(() => _istemci.Beta.Messages.Create(istek)).GetAwaiter().GetResult();
            }
            catch (AnthropicUnauthorizedException)
            {
                throw new YapayZekaHatasi("Yapay zekâ API anahtarı geçersiz (401).");
            }
            catch (AnthropicRateLimitException)
            {
                throw new YapayZekaHatasi("Yapay zekâ servisinin hız sınırı aşıldı (429). Biraz sonra tekrar deneyiniz.");
            }
            catch (Anthropic5xxException)
            {
                throw new YapayZekaHatasi("Yapay zekâ servisi şu an yanıt veremiyor (5xx).");
            }
            catch (AnthropicApiException ex)
            {
                throw new YapayZekaHatasi("Yapay zekâ isteği reddedildi: " + ex.Message);
            }
            catch (AnthropicIOException)
            {
                throw new YapayZekaHatasi("Yapay zekâ servisine bağlanılamadı. İnternet bağlantınızı kontrol ediniz.");
            }
            catch (TaskCanceledException)
            {
                throw new YapayZekaHatasi("Yapay zekâ servisi 60 saniye içinde yanıt vermedi (zaman aşımı).");
            }

            string durma = yanit.StopReason == null ? "" : yanit.StopReason.ToString();
            if (durma.Contains("refusal"))
                throw new YapayZekaHatasi("Yapay zekâ bu metin için yanıt üretmeyi reddetti.");
            if (durma.Contains("max_tokens"))
                throw new YapayZekaHatasi("Yapay zekâ yanıtı yarıda kesildi (uzunluk sınırı).");

            StringBuilder metin = new StringBuilder();
            foreach (BetaContentBlock blok in yanit.Content)
            {
                BetaTextBlock t;
                if (blok.TryPickText(out t)) metin.Append(t.Text);
            }
            if (metin.Length == 0) throw new YapayZekaHatasi("Yapay zekâ boş yanıt döndürdü.");
            return metin.ToString();
        }
    }
}
