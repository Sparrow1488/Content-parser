using Microsoft.Extensions.DependencyInjection;
using System;

namespace Sparrow.Parsing.Utils
{
    public class MiddlewareContext<TResult, TSource>
    {
        public MiddlewareContext(
            ParsingMiddleware<TResult, TSource> next,
            TSource source)
        {
            Next = next;
            Source = source;
            Services = new ServiceCollection();
        }

        public ParsingMiddleware<TResult, TSource> Next { get; }
        public IServiceCollection Services { get; internal set; }
        public IServiceProvider ServiceProvider => Services.BuildServiceProvider();
        public TSource Source { get; internal set; }
    }
}