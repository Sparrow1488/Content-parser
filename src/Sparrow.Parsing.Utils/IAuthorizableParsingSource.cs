using System.Threading.Tasks;

namespace Sparrow.Parsing.Utils
{
    public interface IAuthorizableParsingSource
    {
        Task AuthorizeAsync();
    }
}