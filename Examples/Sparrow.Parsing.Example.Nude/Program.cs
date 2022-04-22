using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Sparrow.Parsing.Example.Nude.Entities;
using Sparrow.Parsing.Example.Nude.Helpers;
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
            ConfigureLogger();
            var nudeSource = new NudeSource();
            var pipe = new ParsingPipeline<List<NudeMangaItem>, NudeSource>(nudeSource)
                          .HandleAll<ExceptionHandleMiddleware>()
                          .Use<InitializeMiddleware>()
                          .Use<PagesParsingMiddleware>()
                          .Use<PreviewsParsingMiddleware>()
                          .Use<MangaParsingMiddleware>()
                          .Use<FilesParsingMiddleware>()
                          .OnHostBuilding(host => host.UseSerilog())
                          .WithServices(services => 
                          {
                              services.AddSingleton(permission => GetAccessPermission());
                              services.AddTransient<QueryHelper>();
                          });
            var result = await pipe.StartAsync();
        }

        private static AccessPermission GetAccessPermission()
        {
            var lines = File.ReadAllLines(ConfigurationManager.AppSettings["access"]);
            var login = lines[0];
            var password = lines[1];
            return new AccessPermission(login, password, remember: true);
        }

        private static void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Console()
                        .CreateLogger();
        }
    }
}
