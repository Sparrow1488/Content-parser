using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Utils
{
    public interface IParsingSource
    {
        Task<Stream> GetAsync();
        public bool IsReceived { get; }
        public IEnumerable<IParsingSource> Bindings { get; }
    }
}