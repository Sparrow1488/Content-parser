using AngleSharp.Html.Parser;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sparrow.Parsing.Example.Nude.Entities;
using Sparrow.Parsing.Example.Nude.Sources;
using Sparrow.Parsing.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Nude.Middlewares
{
    internal class InitializeMiddleware : ParsingMiddleware<List<NudeMangaItem>, NudeSource>
    {
        public InitializeMiddleware(IConfiguration config) =>
            _config = config;

        private readonly IConfiguration _config;

        public override async Task ProcessAsync(List<NudeMangaItem> toProcess)
        {
            await Context.Source.AuthorizeAsync();

            Context.Services.AddSingleton<IHtmlParser, HtmlParser>();

            await InvokeNextAsync(toProcess);
        }
    }
}
