using Sparrow.Parsing.Example.Nude.Providers;
using Sparrow.Parsing.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Nude.Sources
{
    internal class NudeSource : ITextParsingSource, IAuthorizableParsingSource
    {
        public NudeSource(AccessPermission access)
        {
            _httpClient = new HttpClientWrapper();
            _access = access;
        }

        public NudeSource()
        {
            _httpClient = new HttpClientWrapper();
        }

        private readonly AccessPermission _access;
        private readonly HttpClientWrapper _httpClient;
        private readonly string _endPoint = "https://nude-moon.net/all_manga";

        public bool IsReceived => throw new NotImplementedException();

        public IEnumerable<IParsingSource> Bindings => throw new NotImplementedException();

        public async Task AuthorizeAsync()
        {
            // типа тут авторизация
            await GetTextAsync();
            _httpClient.Cookies.GetCookies(new Uri("https://nude-moon.net/"));
        }

        public async Task<string> GetTextAsync() => 
            await SendRequestAsync(_endPoint);

        public async Task<string> GetPageAsStringAsync(int page) =>
            await SendRequestAsync(_endPoint + "?rowstart=" + (page - 1) * 30);

        private async Task<string> SendRequestAsync(string endpoint)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            var response = await _httpClient.SendAsync(request);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encoding = Encoding.GetEncoding("windows-1251");
            var text = encoding.GetString(bytes);
            return text;
        }

        public Task<Stream> GetAsync() =>
            throw new NotImplementedException();

        private IEnumerable<KeyValuePair<string, string>> CreateAuthorizationData()
        {
            var dataList = new List<KeyValuePair<string, string>>();
            dataList.Add(new KeyValuePair<string, string>("user_name", _access.Login));
            dataList.Add(new KeyValuePair<string, string>("user_pass", _access.Password));
            dataList.Add(new KeyValuePair<string, string>("remember_me", _access.Remember ? "y" : "n"));
            return dataList;
        }
    }
}
