using Sparrow.Parsing.Utils;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example
{
    internal class Program
    {
        private static async Task Main()
        {
            var source = new MicrosoftSource("https://www.microsoft.com/ru-ru/");
            var pipe = new ParsingPipeline<MicrosoftEntity, MicrosoftSource>(source)
                          .Use<TitlesParsingMiddleware>()
                          .Use<ProductsParsingMiddleware>();
            var resultEntity = await pipe.StartAsync();
        }
    }
}
