using Sparrow.Parsing.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Microsoft.Middlewares
{
    internal class ExceptionHandleMiddleware : IExceptionHandleMiddleware
    {
        public IEnumerable<Exception> CanHandle => new List<Exception>()
        {
            new ArgumentException(),
            new NullReferenceException(),
            new ArgumentNullException()
        };

        public void Handle(Exception ex)
        {
            Console.WriteLine(nameof(ExceptionHandleMiddleware) + " обрабатывает исключение " + ex.GetType());
        }

        public Task HandleAsync(Exception ex)
        {
            Handle(ex);
            return Task.CompletedTask;
        }
    }
}
