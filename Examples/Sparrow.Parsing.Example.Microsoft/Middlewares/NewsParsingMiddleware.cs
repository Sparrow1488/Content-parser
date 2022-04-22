using AngleSharp.Html.Dom;
using Sparrow.Parsing.Example.Sources;
using Sparrow.Parsing.Utils;
using System;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Middlewares
{
    internal class NewsParsingMiddleware : ParsingMiddleware<MicrosoftEntity, MicrosoftSource>
    {
        public override async Task ProcessAsync(MicrosoftEntity toProcess)
        {
            throw new ArgumentException("Че то пошло не так");

            toProcess.Guid = Guid.NewGuid();

            var document = (IHtmlDocument)Context.ServiceProvider.GetService(typeof(IHtmlDocument));
            var newsElements = document.QuerySelectorAll(".m-content-placement-item");
            foreach (var newsElement in newsElements)
            {
                var microsoftNews = new MicrosoftNews();
                microsoftNews.Title = newsElement.QuerySelector(".c-heading")?.TextContent;
                microsoftNews.Text = newsElement.QuerySelector(".c-paragraph p")?.TextContent;
                toProcess.News.Add(microsoftNews);
            }

            await InvokeNextAsync(toProcess);
        }
    }
}
