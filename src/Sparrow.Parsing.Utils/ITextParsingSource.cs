using System.Threading.Tasks;

namespace Sparrow.Parsing.Utils
{
    public interface ITextParsingSource : IParsingSource
    {
        Task<string> GetTextAsync();
    }
}
