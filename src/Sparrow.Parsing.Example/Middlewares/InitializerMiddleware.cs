using AngleSharp.Html.Parser;
using Sparrow.Parsing.Example.Sources;
using Sparrow.Parsing.Utils;
using System;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Middlewares
{
    internal class InitializerMiddleware : ParsingMiddleware<MicrosoftEntity, MicrosoftSource>
    {
        public override void Process(MicrosoftEntity toProcess, MicrosoftSource source) =>
            throw new NotImplementedException();

        public override async Task ProcessAsync(MicrosoftEntity toProcess, MicrosoftSource source)
        {
            // получить страничку сайта
            // произвести авторизацию - получить кукисы
            // настроить MicrosoftSource, чтобы не было проблем с получением данных
            // получить данные и сохранить их в (абстрактно) контекст для передачи его в другие middlewares
            // короче подготовить почву и смачно вывалить все в контекст

            var sourceText = await source.GetTextAsync();
            var parser = new HtmlParser();
            var document = await parser.ParseDocumentAsync(sourceText);
        }
    }
}
