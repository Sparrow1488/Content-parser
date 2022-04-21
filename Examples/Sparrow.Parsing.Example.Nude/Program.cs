using Sparrow.Parsing.Example.Nude.Entities;
using Sparrow.Parsing.Example.Nude.Middlewares;
using Sparrow.Parsing.Example.Nude.Sources;
using Sparrow.Parsing.Utils;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Nude
{
    internal class Program
    {
        private static async Task Main()
        {
            var nudeSource = new NudeSource();
            var pipe = new ParsingPipeline<List<NudeMangaItem>, NudeSource>(nudeSource)
                          .Use<InitializeMiddleware>()
                          .Use<PagesParsingMiddleware>()
                          .Use<PreviewParsingMiddleware>();
            var result = await pipe.StartAsync();
        }

        private static AccessPermission GetAccessPermission()
        {
            var lines = File.ReadAllLines(ConfigurationManager.AppSettings["access"]);
            var login = lines[0];
            var password = lines[1];
            return new AccessPermission(login, password, remember: true);
        }
    }
}
