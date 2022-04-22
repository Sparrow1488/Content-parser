using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Sparrow.Parsing.Example.Nude.Entities;
using Sparrow.Parsing.Example.Nude.Middlewares;
using Sparrow.Parsing.Example.Nude.Providers;
using Sparrow.Parsing.Example.Nude.Sources;
using Sparrow.Parsing.Utils;
using Sparrow.Parsing.Utils.Enums;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Nude
{
    internal class Program
    {
        private static async Task<int> Main()
        {
            ConfigureLogger();
            var pipe = new ParsingPipeline<List<NudeMangaItem>, NudeSource>()
                          .HandleAll<ExceptionHandleMiddleware>()
                          .Use<InitializeMiddleware>()
                          .Use<PagesParsingMiddleware>()
                          .Use<PreviewsParsingMiddleware>()
                          .Use<MangaParsingMiddleware>()
                          .Use<FilesParsingMiddleware>()
                          .OnHostBuilding(host => host.UseSerilog())
                          .WithServices(services =>
                          {
                              services.AddSingleton<HttpClientWrapper>();
                              services.AddSingleton(permission => GetAccessPermission());
                          });
            var result = await pipe.StartAsync();
            if (result.Status == ExecutionStatus.Ok)
                return 0;
            else return 1;
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
                        .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                        .CreateLogger();
        }
    }
}
