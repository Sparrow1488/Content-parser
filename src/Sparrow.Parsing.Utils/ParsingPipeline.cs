using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Utils
{
    public sealed class ParsingPipeline<TResult, TSource>
    {
        public ParsingPipeline(TSource source)
        {
            _source = source;
            _middlewares = new List<ParsingMiddleware<TResult, TSource>>();
            _middlewaresTypes = new List<Type>();
            _middlewaresInitServices = new ServiceCollection();
        }

        private readonly TSource _source;
        private IList<Type> _middlewaresTypes;
        private IList<ParsingMiddleware<TResult, TSource>> _middlewares;
        private IServiceCollection _middlewaresInitServices;

        public ParsingPipeline<TResult, TSource> Use<TMiddleware>()
            where TMiddleware : ParsingMiddleware<TResult, TSource>
        {
            _middlewaresInitServices.AddSingleton<TMiddleware>();
            _middlewaresTypes.Add(typeof(TMiddleware));
            return this;
        }

        public ParsingPipeline<TResult, TSource> WithServices(Action<IServiceCollection> services)
        {
            services?.Invoke(_middlewaresInitServices);
            return this;
        }

        public TResult Start() => throw new NotImplementedException();

        public async Task<TResult> StartAsync()
        {
            CreateMiddlewares();
            var resultEntity = Activator.CreateInstance<TResult>();
            for (int i = 0; i < _middlewares.Count; i++)
            {
                var currentMiddleware = _middlewares[i];
                MiddlewareContext<TResult, TSource> context;
                if (HasNext(i, _middlewares.Count))
                    context = new MiddlewareContext<TResult, TSource>(_middlewares[i + 1], _source);
                else context = new MiddlewareContext<TResult, TSource>(null, _source);

                context.Services = _middlewaresInitServices;
                currentMiddleware.SetContext(context);
            }

            var firstMiddleware = _middlewares.First();
            await firstMiddleware.ProcessAsync(resultEntity);
            return resultEntity;
        }

        private void CreateMiddlewares()
        {
            var host = Host.CreateDefaultBuilder().ConfigureServices(services => services = _middlewaresInitServices).Build();
            foreach (var type in _middlewaresTypes)
            {
                var instance = (ParsingMiddleware<TResult, TSource>)ActivatorUtilities.CreateInstance(host.Services, type);
                _middlewares.Add(instance);
            }
        }

        private bool HasNext(int current, int total)
        {
            if (current + 1 < total)
                return true;
            return false;
        }
    }
}
