using Sparrow.Parsing.Utils;
using System;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example
{
    public class TitlesParsingMiddleware : ParsingMiddleware<MicrosoftEntity, MicrosoftSource>
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
            toProcess.Guid = Guid.NewGuid();
            toProcess.Author = "Microsoft_Man";
        }
    }
}
