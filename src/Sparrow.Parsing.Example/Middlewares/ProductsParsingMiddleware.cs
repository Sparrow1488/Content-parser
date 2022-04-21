using Sparrow.Parsing.Example.Sources;
using Sparrow.Parsing.Utils;
using System;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Middlewares
{
    internal class ProductsParsingMiddleware : ParsingMiddleware<MicrosoftEntity, MicrosoftSource>
    {
        public override void Process(MicrosoftEntity toProcess) =>
            throw new NotImplementedException();

        public override async Task ProcessAsync(MicrosoftEntity toProcess)
        {
            SomeLogic(toProcess);
            await InvokeNextAsync(toProcess);
        }

        private void SomeLogic(MicrosoftEntity toProcess)
        {
            toProcess.CreatedAt = DateTime.UtcNow;
        }
    }
}
