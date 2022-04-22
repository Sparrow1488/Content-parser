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
    internal class MangaParsingMiddleware : ParsingMiddleware<List<NudeMangaItem>, NudeSource>
    {
        public override async Task ProcessAsync(List<NudeMangaItem> toProcess)
        {
            var currentProcessing = (NudePreviewCard)Context.ServiceProvider.GetService(typeof(NudePreviewCard));
            if (!string.IsNullOrWhiteSpace(currentProcessing.MainSource))
            {
                string baseEndPoint = Context.Source.EndPointBase;
                var html = await Context.Source.SendRequestAsync(currentProcessing.MainSource);
                var parser = (IHtmlParser)Context.ServiceProvider.GetService(typeof(IHtmlParser));
                var document = await parser.ParseDocumentAsync(html);

                var manga = new NudeMangaItem();
                var tags = document.QuerySelectorAll(".tag-links a").Select(link => link.TextContent);
                manga.Description = document.QuerySelector(".description")?.TextContent;
                Context.Services.AddSingleton(document);

                toProcess.Add(manga);
                await InvokeNextAsync(toProcess);
            }
            else
            {
                Console.WriteLine("Не удалось получить ссылку на мангу");
            }
        }
    }
}
