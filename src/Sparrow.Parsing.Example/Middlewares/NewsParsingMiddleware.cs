using Sparrow.Parsing.Example.Sources;
using Sparrow.Parsing.Utils;
using System;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Middlewares
{
    internal class NewsParsingMiddleware : ParsingMiddleware<MicrosoftEntity, MicrosoftSource>
    {
        public override void Process(MicrosoftEntity toProcess, MicrosoftSource source) =>
            throw new NotImplementedException();

        public override async Task ProcessAsync(MicrosoftEntity toProcess, MicrosoftSource source)
        {
            toProcess.Guid = Guid.NewGuid();
            toProcess.Author = "Sparrow";
            var html = await source.GetTextAsync();
            await InvokeNextAsync(toProcess, source);
        }
    }
}
