using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Nude.Providers
{
    internal class HttpClientWrapper
    {
        public HttpClientWrapper() 
        {
            Cookies = new CookieContainer();
            Cookies.Add(new Cookie("fusion_visited", "yes", "/", "nude-moon.net"));
            Cookies.Add(new Cookie("_ym_uid", "1650557551", "/", "nude-moon.net"));
            Cookies.Add(new Cookie("_ym_d", "1650557161", "/", "nude-moon.net"));
            Cookies.Add(new Cookie("_ym_isad", "1", "/", "nude-moon.net"));
            Cookies.Add(new Cookie("_ym_visorc", "b", "/", "nude-moon.net"));
            Cookies.Add(new Cookie("_gid", "GA1.2.1615588149.1650557161", "/", "nude-moon.net"));
            Cookies.Add(new Cookie("_ga", "GA1.2.1334179186.1650557161", "/", "nude-moon.net"));
            Cookies.Add(new Cookie("_gat_gtag_UA_51634583_1", "1", "/", "nude-moon.net"));
            Cookies.Add(new Cookie("fusion_user", "446221.ad21f16c541e88e36b390a7b3b863086", "/", "nude-moon.net"));
            Cookies.Add(new Cookie("_ga_4Y96K6THGH", "GS1.1.1650557160.1.1.1650557561.0", "/", "nude-moon.net"));
        }

        public CookieContainer Cookies { get; private set; }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage message = default;
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.127 Safari/537.36");
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Accept-Language", "ru-RU,ru;q=0.9");
            request.Headers.Add("Referer", "https://www.google.com/");

            using (var client = CreateHttpClient())
            {
                System.Console.WriteLine("GET => " + request.RequestUri.ToString());
                message = await client.SendAsync(request, cancellationToken);
            }
            return message;
        }

        private HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler();
            handler.CookieContainer = Cookies;
            handler.UseCookies = true;
            return new HttpClient(handler);
        }
    }
}
