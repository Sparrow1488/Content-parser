using Sparrow.Parsing.Utils;
using System;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example
{
    internal class ProductsParsingMiddleware : ParsingMiddleware<MicrosoftEntity, MicrosoftSource>
    {
        public override void Process(MicrosoftEntity toProcess, MicrosoftSource source)
        {
            SomeLogic(toProcess);
            InvokeNext(toProcess, source);
        }

        public override async Task ProcessAsync(MicrosoftEntity toProcess, MicrosoftSource source)
        {
            SomeLogic(toProcess);
            await InvokeNextAsync(toProcess, source);
        }

        private void SomeLogic(MicrosoftEntity toProcess)
        {
            toProcess.Description = "Some description from Microsoft site";
            toProcess.CreatedAt = DateTime.UtcNow;
        }
    }
}
