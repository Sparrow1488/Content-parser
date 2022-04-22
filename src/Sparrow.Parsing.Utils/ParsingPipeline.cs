using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sparrow.Parsing.Utils.Enums;
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

        private IHost _host;
        private TSource _source;
        private readonly IHostBuilder _hostBuilder;
        private readonly List<Type> _middlewaresTypes;
        private IList<ParsingMiddleware<TResult, TSource>> _middlewares;

        public ParsingPipeline<TResult, TSource> Use<TMiddleware>()
            where TMiddleware : ParsingMiddleware<TResult, TSource>
        {
            _middlewaresTypes.Add(typeof(TMiddleware));
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

        public async Task<PipelineExecutionResult<TResult>> StartAsync()
        {
            var result = new PipelineExecutionResult<TResult>();
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
                context.HostServiceProvider = _host.Services;
                currentMiddleware.SetContext(context);
            }

            var firstMiddleware = _middlewares.First();
            await firstMiddleware.ProcessAsync(resultEntity);

            result.Status = GetStatus();
            result.Content = resultEntity;
            return result;
        }

        private void InitializePipeline()
        {
            _host = _hostBuilder.Build();
            foreach (var type in _middlewaresTypes)
            {
                var instance = (ParsingMiddleware<TResult, TSource>)ActivatorUtilities.CreateInstance(_host.Services, type);
                _middlewares.Add(instance);
            }

            _source = ActivatorUtilities.CreateInstance<TSource>(_host.Services);
        }

        private bool HasNext(int current, int total)
        {
            if (current + 1 < total)
                return true;
            return false;
        }

        private ExecutionStatus GetStatus()
        {
            ExecutionStatus status = ExecutionStatus.NotHandleError;
            if (_middlewares.All(x => x.Context.Status == ExecutionStatus.Ok))
                status = ExecutionStatus.Ok;
            if (_middlewares.Any(x => x.Context.Status == ExecutionStatus.HandleError))
                status = ExecutionStatus.HandleError;
            if (_middlewares.Any(x => x.Context.Status == ExecutionStatus.NotHandleError))
                status = ExecutionStatus.NotHandleError;
            return status;
        }
    }
}
