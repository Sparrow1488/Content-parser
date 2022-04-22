using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sparrow.Parsing.Example.Nude.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Nude.Middlewares
{
    internal class PagesParsingMiddleware : NudeMiddlewareBase
    {
        public PagesParsingMiddleware(ILogger<NudeMiddlewareBase> logger) : base(logger) { }

        public override async Task ProcessAsync(List<NudeMangaItem> toProcess)
        {
            var html = await Context.Source.GetTextAsync();
            var parser = Context.ServiceProvider.GetService<IHtmlParser>();
            var document = await parser.ParseDocumentAsync(html);

            var totalPages = ParsePagesCount(document);
            for (int i = 1; i <= totalPages; i++)
            {
                html = await Context.Source.GetPageAsStringAsync(i);
                document = await parser.ParseDocumentAsync(html);
                Context.Services.AddSingleton(document);

                logger.LogInformation($"Обрабатываем страницу {i}/{totalPages}");
                await InvokeNextAsync(toProcess);
                WriteInfo(toProcess);
                await Task.Delay(TimeSpan.FromSeconds(3));
            }
        }

        private int ParsePagesCount(IHtmlDocument document)
        {
            var pagesElement = document.QuerySelector("td.tbl1");
            var currentAndLastPages = pagesElement.TextContent.Split("из");
            var lastPage = currentAndLastPages.Last();
            lastPage = lastPage.Trim();
            return int.Parse(lastPage);
        }

        private void WriteInfo(List<NudeMangaItem> toProcess)
        {
            Console.WriteLine();
            logger?.LogInformation("Итого: " + toProcess.Count);
            logger?.LogInformation("Можно скачать изображения: " + toProcess.Count(x => x?.Images?.Any() ?? false));
            Console.WriteLine();
        }
    }
}
