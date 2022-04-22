using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Sparrow.Parsing.Example.Nude.Entities;
using Sparrow.Parsing.Example.Nude.Sources;
using Sparrow.Parsing.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Nude.Middlewares
{
    internal class FilesParsingMiddleware : ParsingMiddleware<List<NudeMangaItem>, NudeSource>
    {
        public override async Task ProcessAsync(List<NudeMangaItem> toProcess)
        {
            var mangaDocument = (IHtmlDocument)Context.ServiceProvider.GetService(typeof(IHtmlDocument));
            var readButton = mangaDocument.QuerySelectorAll("span.button a")
                                          .Where(x => x.TextContent.ToUpper().Contains("ЧИТАТЬ"))
                                          .FirstOrDefault();
            var link = readButton?.GetAttribute("href");
            if (!string.IsNullOrWhiteSpace(link))
            {
                var fullLink = Context.Source.EndPointBase + link + "?row";
                var response = await Context.Source.SendRequestAsync(fullLink);
                var parser = (IHtmlParser)Context.ServiceProvider.GetService(typeof(IHtmlParser));
                var imagesDocument = await parser.ParseDocumentAsync(response);
                var galleryItems = imagesDocument.QuerySelectorAll(".gallery-item");

                var imagesLinks = new List<string>();
                foreach (var item in galleryItems)
                {
                    var imageItem = item.QuerySelector("img");
                    var galleryImage = imageItem?.GetAttribute("data-src");
                    if(!string.IsNullOrWhiteSpace(galleryImage))
                        imagesLinks.Add(galleryImage);
                }
                var lastProcessing = toProcess.Last();
                lastProcessing.Images = imagesLinks;
            }
            await InvokeNextAsync(toProcess);
        }
    }
}
