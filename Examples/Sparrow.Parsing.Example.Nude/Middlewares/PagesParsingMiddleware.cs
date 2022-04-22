using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.DependencyInjection;
using Sparrow.Parsing.Example.Nude.Entities;
using Sparrow.Parsing.Example.Nude.Sources;
using Sparrow.Parsing.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Nude.Middlewares
{
    internal class PagesParsingMiddleware : ParsingMiddleware<List<NudeMangaItem>, NudeSource>
    {
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

                Console.WriteLine($"Обрабатываемс {i}/{totalPages}");
                await InvokeNextAsync(toProcess);
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
    }
}
