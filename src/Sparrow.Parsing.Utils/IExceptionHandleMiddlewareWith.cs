using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Utils
{
    public interface IExceptionHandleMiddlewareWith<TResult> : IExceptionHandleMiddlewareBase
    {
        Task HandleAsync(Exception ex, TResult toProcess);
        void Handle(Exception ex, TResult toProcess);
    }

    public interface IExceptionHandleMiddleware : IExceptionHandleMiddlewareBase
    {
        Task HandleAsync(Exception ex);
        void Handle(Exception ex);
        
    }

    public interface IExceptionHandleMiddlewareBase
    {
        public IEnumerable<Exception> CanHandle { get; }
    }
}