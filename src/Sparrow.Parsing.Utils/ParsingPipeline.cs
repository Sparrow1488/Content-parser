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
        public ParsingPipeline(TSource source = default)
        {
            _source = source;
            _hostBuilder = Host.CreateDefaultBuilder();
            _middlewaresTypes = new List<Type>();
            _middlewares = new List<ParsingMiddleware<TResult, TSource>>();
        }

        private TSource _source;
        private IHost _host;
        private readonly IHostBuilder _hostBuilder;
        private readonly List<Type> _middlewaresTypes;
        private IList<ParsingMiddleware<TResult, TSource>> _middlewares;

        public ParsingPipeline<TResult, TSource> Use<TMiddleware>()
            where TMiddleware : ParsingMiddleware<TResult, TSource>
        {
            _middlewaresTypes.Add(typeof(TMiddleware));
            //_hostBuilder.ConfigureServices(services => services.AddSingleton<TMiddleware>());
            return this;
        }

        public ParsingPipeline<TResult, TSource> OnHostBuilding(Action<IHostBuilder> host)
        {
            host?.Invoke(_hostBuilder);
            return this;
        }

        public ParsingPipeline<TResult, TSource> WithServices(Action<IServiceCollection> services)
        {
            _hostBuilder.ConfigureServices(hostServices =>
            {
                services?.Invoke(hostServices);
            });
            return this;
        }

        public ParsingPipeline<TResult, TSource> HandleAll<THandler>()
            where THandler : IExceptionHandleMiddlewareBase
        {
            _hostBuilder.ConfigureServices(services => 
                    services.AddSingleton(typeof(IExceptionHandleMiddlewareBase), typeof(THandler)));
            return this;
        }

        public TResult Start() => throw new NotImplementedException();

        public async Task<TResult> StartAsync()
        {
            InitializePipeline();
            var resultEntity = Activator.CreateInstance<TResult>();
            for (int i = 0; i < _middlewares.Count; i++)
            {
                var currentMiddleware = _middlewares[i];
                MiddlewareContext<TResult, TSource> context;
                if (HasNext(i, _middlewares.Count))
                    context = new MiddlewareContext<TResult, TSource>(_middlewares[i + 1], _source);
                else context = new MiddlewareContext<TResult, TSource>(null, _source);

                context.Services = new ServiceCollection();
                currentMiddleware.SetContext(context);
            }

            var firstMiddleware = _middlewares.First();
            await firstMiddleware.ProcessAsync(resultEntity);
            return resultEntity;
        }

        private void InitializePipeline()
        {
            var host = _hostBuilder.Build();
            foreach (var type in _middlewaresTypes)
            {
                var instance = (ParsingMiddleware<TResult, TSource>)ActivatorUtilities.CreateInstance(host.Services, type);
                _middlewares.Add(instance);
            }

            _source = ActivatorUtilities.CreateInstance<TSource>(host.Services);
        }

        private bool HasNext(int current, int total)
        {
            if (current + 1 < total)
                return true;
            return false;
        }
    }
}
