using Microsoft.Extensions.DependencyInjection;
using Sparrow.Parsing.Utils.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Utils
{
    public abstract class ParsingMiddleware<TResult, TSource>
    {
        public MiddlewareContext<TResult, TSource> Context { get; private set; }

        public virtual void Process(TResult toProcess) => ProcessAsync(toProcess).ConfigureAwait(false);
        public abstract Task ProcessAsync(TResult toProcess);

        protected void InvokeNext(
            TResult toProcess) => Context.Next?.Process(toProcess);

        protected async Task InvokeNextAsync(
            TResult toProcess)
        {
            if (Context.Next != null)
            {
                await TryToInvokeNextAsync(toProcess);
            }
        }

        private async Task TryToInvokeNextAsync(TResult toProcess)
        {
            try
            {
                Context.Next.Context.Services = Context.Services;
                await Context.Next.ProcessAsync(toProcess);
            }
            catch(Exception ex)
            {
                Context.Status = ExecutionStatus.NotHandleError;
                var exceptionHandler = Context.HostServiceProvider.GetService<IExceptionHandleMiddlewareBase>();
                if(exceptionHandler?.CanHandle.Any(x => x.GetType() == ex.GetType()) ?? false)
                {
                    await StartExceptionHandleMiddlewareAsync(toProcess, exceptionHandler, ex);
                }
            }
        }

        private async Task StartExceptionHandleMiddlewareAsync(
            TResult toProcess, IExceptionHandleMiddlewareBase handler, Exception ex)
        {
            if (handler is IExceptionHandleMiddleware middlewareHandler)
            {
                await middlewareHandler.HandleAsync(ex);
                Context.Status = ExecutionStatus.HandleError;
            }
            else if (handler is IExceptionHandleMiddlewareWith<TResult> middlewareWithEntity)
            {
                await middlewareWithEntity.HandleAsync(ex, toProcess);
                Context.Status = ExecutionStatus.HandleError;
            }
        }

        internal void SetContext(MiddlewareContext<TResult, TSource> context) => Context = context;
    }
}
