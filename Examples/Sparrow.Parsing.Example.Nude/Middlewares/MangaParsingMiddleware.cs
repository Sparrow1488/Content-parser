using AngleSharp.Html.Parser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sparrow.Parsing.Example.Nude.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Nude.Middlewares
{
    internal class MangaParsingMiddleware : NudeMiddlewareBase
    {
        public MangaParsingMiddleware(ILogger<NudeMiddlewareBase> logger) : base(logger) { }

        public override async Task ProcessAsync(List<NudeMangaItem> toProcess)
        {
            var currentProcessing = (NudePreview)Context.ServiceProvider.GetService(typeof(NudePreview));
            if (!string.IsNullOrWhiteSpace(currentProcessing.MainSource))
            {
                string baseEndPoint = Context.Source.EndPointBase;
                var html = await Context.Source.SendRequestAsync(currentProcessing.MainSource);
                var parser = (IHtmlParser)Context.ServiceProvider.GetService(typeof(IHtmlParser));
                var document = await parser.ParseDocumentAsync(html);

                var manga = new NudeMangaItem();
                var tags = document.QuerySelectorAll(".tag-links a")?.Select(link => link.TextContent)?.ToList();
                manga.Description = document.QuerySelector(".description")?.TextContent ?? string.Empty;
                manga.Tags = tags;

                Context.Services.AddSingleton(document);

                toProcess.Add(manga);
                await InvokeNextAsync(toProcess);
            }
            else
            {
                logger.LogError("Не удалось получить ссылку на мангу");
            }
        }
    }
}
