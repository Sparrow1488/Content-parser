using Sparrow.Parsing.Utils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example
{
    public class MicrosoftSource : IParsingSource
    {
        public MicrosoftSource(string endPoint)
        {
            _endPoint = endPoint;
        }

        private readonly string _endPoint;

        public IEnumerable<IParsingSource> Bindings => Array.Empty<IParsingSource>();

        public async Task<string> ParseAsync()
        {
            string sourceResponse = string.Empty;
            using (var client = new HttpClient())
            {
                sourceResponse = await client.GetStringAsync(_endPoint);
            }
            return sourceResponse;
        }
    }
}
