using AngleSharp.Html.Parser;
using Microsoft.Extensions.DependencyInjection;
using Sparrow.Parsing.Example.Sources;
using Sparrow.Parsing.Utils;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Middlewares
{
    internal class InitializerMiddleware : ParsingMiddleware<MicrosoftEntity, MicrosoftSource>
    {
        public override async Task ProcessAsync(MicrosoftEntity toProcess)
        {
            var sourceText = await Context.Source.GetTextAsync();
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(sourceText);
            Context.Services.AddSingleton(document);

            await InvokeNextAsync(toProcess);
        }
    }
}