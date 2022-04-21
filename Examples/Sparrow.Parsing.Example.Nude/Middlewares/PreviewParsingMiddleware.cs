using AngleSharp.Html.Dom;
using Sparrow.Parsing.Example.Nude.Entities;
using Sparrow.Parsing.Example.Nude.Sources;
using Sparrow.Parsing.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Nude.Middlewares
{
    internal class PreviewParsingMiddleware : ParsingMiddleware<List<NudeMangaItem>, NudeSource>
    {
        public override async Task ProcessAsync(List<NudeMangaItem> toProcess)
        {
            var document = (IHtmlDocument)Context.ServiceProvider.GetService(typeof(IHtmlDocument));
            var mainBody = document.QuerySelector(".main-body");
            var previewCardsElements = mainBody.QuerySelectorAll("table")
                                               .Where(x => x.QuerySelector("table") != null);
            foreach (var cardElement in previewCardsElements)
            {
                var previewCard = new NudePreviewCard();
                previewCard.Title = cardElement.QuerySelector("h2")?.TextContent;
                var absoluteLink = cardElement.QuerySelector("td.bg_style1 a")?.GetAttribute("href");
                previewCard.MainSource = "https://nude-moon.net" + absoluteLink;
                previewCard.PreviewImage = cardElement.QuerySelector("span.box img.news_pic2")?.GetAttribute("href");

                if (!string.IsNullOrWhiteSpace(previewCard.Title))
                {
                    Console.WriteLine(previewCard.Title);

                    await InvokeNextAsync(toProcess);

                    var lastManagaItem = toProcess.LastOrDefault();
                    if (lastManagaItem != null)
                        lastManagaItem.Preview = previewCard;
                }
            }
        }
    }
}
