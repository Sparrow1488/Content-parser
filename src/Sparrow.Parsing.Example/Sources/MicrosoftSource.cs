using Sparrow.Parsing.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Sources
{
    public class MicrosoftSource : ITextParsingSource
    {
        public MicrosoftSource(string endPoint)
        {
            _endPoint = endPoint;
        }

        private readonly string _endPoint;
        private bool _isReceived = false;

        public IEnumerable<IParsingSource> Bindings => Array.Empty<IParsingSource>();
        public bool IsReceived => _isReceived;

        public async Task<Stream> GetAsync()
        {
            MemoryStream sourceStream = new MemoryStream();
            using (var client = new HttpClient())
            {
                var message = new HttpRequestMessage(HttpMethod.Get, _endPoint);
                var response = await client.SendAsync(message);
                var responseStream = await response.Content.ReadAsStreamAsync();
                await responseStream.CopyToAsync(sourceStream);
                _isReceived = true;
            }
            return sourceStream;
        }

        public async Task<string> GetTextAsync()
        {
            string text = string.Empty;
            using (var stream = (MemoryStream)await GetAsync())
            {
                using (var sr = new StreamReader(stream))
                {
                    text = await sr.ReadToEndAsync();
                }
            }
            return text;
        }
    }
}
