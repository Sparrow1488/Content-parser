using Microsoft.Extensions.Logging;
using Sparrow.Parsing.Example.Nude.Entities;
using Sparrow.Parsing.Example.Nude.Sources;
using Sparrow.Parsing.Utils;
using System.Collections.Generic;

namespace Sparrow.Parsing.Example.Nude.Middlewares
{
    internal abstract class NudeMiddlewareBase : ParsingMiddleware<List<NudeMangaItem>, NudeSource>
    {
        public NudeMiddlewareBase(ILogger<NudeMiddlewareBase> logger)
        {
            this.logger = logger;
        }

        protected readonly ILogger<NudeMiddlewareBase> logger;
    }
}
