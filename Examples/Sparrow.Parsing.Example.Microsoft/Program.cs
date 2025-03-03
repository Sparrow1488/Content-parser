﻿using Sparrow.Parsing.Example.Microsoft.Middlewares;
using Sparrow.Parsing.Example.Middlewares;
using Sparrow.Parsing.Example.Sources;
using Sparrow.Parsing.Utils;
using System;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example
{
    internal class Program
    {
        private static async Task Main()
        {
            var source = new MicrosoftSource("https://www.microsoft.com/ru-ru/");
            var pipe = new ParsingPipeline<MicrosoftEntity, MicrosoftSource>(source)
                          //.HandleAll<ExceptionHandleMiddleware>()
                          .HandleAll<ExceptionHandleMiddlewareWithEntity>()
                          .Use<InitializerMiddleware>()
                          .Use<NewsParsingMiddleware>()
                          .Use<ProductsParsingMiddleware>();
            var resultEntity = await pipe.StartAsync();
            Console.WriteLine("Статус выполнения: " + resultEntity.Status);
            Console.WriteLine(resultEntity.Content);
        }
    }
}