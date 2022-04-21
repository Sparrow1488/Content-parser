using AngleSharp.Html.Dom;
using Sparrow.Parsing.Example.Sources;
using Sparrow.Parsing.Utils;
using System;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Middlewares
{
    internal class ProductsParsingMiddleware : ParsingMiddleware<MicrosoftEntity, MicrosoftSource>
    {
        public override async Task ProcessAsync(MicrosoftEntity toProcess)
        {
            toProcess.CreatedAt = DateTime.UtcNow;
            var document = (IHtmlDocument)Context.ServiceProvider.GetService(typeof(IHtmlDocument));
            var productsMenu = document.QuerySelector(".f-multi-column");
            var productsLinks = productsMenu.QuerySelectorAll("a.js-subm-uhf-nav-link");
            foreach (var productLink in productsLinks)
            {
                var product = new MicrosoftProduct();
                product.Title = productLink.TextContent;
                product.Link = productLink.GetAttribute("href");
                toProcess.Products.Add(product);
            }
            await InvokeNextAsync(toProcess);
        }
    }
}
