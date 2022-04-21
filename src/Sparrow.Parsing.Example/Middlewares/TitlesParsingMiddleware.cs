using Sparrow.Parsing.Example.Sources;
using Sparrow.Parsing.Utils;
using System;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Middlewares
{
    internal class TitlesParsingMiddleware : ParsingMiddleware<MicrosoftEntity, MicrosoftSource>
    {
        public override void Process(MicrosoftEntity toProcess, MicrosoftSource source) =>
            throw new NotImplementedException();

        public override async Task ProcessAsync(MicrosoftEntity toProcess, MicrosoftSource source)
        {
            SomeLogic(toProcess);
            await InvokeNextAsync(toProcess, source);
        }

        private void SomeLogic(MicrosoftEntity toProcess)
        {
            toProcess.Guid = Guid.NewGuid();
            toProcess.Author = "Microsoft_Man";
        }
    }
}
