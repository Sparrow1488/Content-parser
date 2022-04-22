using AngleSharp.Html.Parser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sparrow.Parsing.Example.Nude.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Nude.Middlewares
{
    internal class InitializeMiddleware : NudeMiddlewareBase
    {
        public InitializeMiddleware(ILogger<NudeMiddlewareBase> logger) : base(logger) { }

        public override async Task ProcessAsync(List<NudeMangaItem> toProcess)
        {
            logger.LogInformation(nameof(InitializeMiddleware) + " начал инициализацию");
            await Context.Source.AuthorizeAsync();
            Context.Services.AddSingleton<IHtmlParser, HtmlParser>();
            logger.LogInformation(nameof(InitializeMiddleware) + " инициализацию закончил");

            await InvokeNextAsync(toProcess);
        }
    }
}
