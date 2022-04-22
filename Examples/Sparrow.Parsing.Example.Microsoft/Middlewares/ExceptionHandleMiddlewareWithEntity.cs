using Sparrow.Parsing.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Microsoft.Middlewares
{
    internal class ExceptionHandleMiddlewareWithEntity : IExceptionHandleMiddlewareWith<MicrosoftEntity>
    {
        public IEnumerable<Exception> CanHandle => new List<Exception>()
        {
            new ArgumentException(),
            new ArgumentNullException()
        };

        public void Handle(Exception ex, MicrosoftEntity toProcess)
        {
            Console.WriteLine(nameof(ExceptionHandleMiddlewareWithEntity) + " обрабатывает исключение " + ex.GetType());
            Console.WriteLine("Также вносим корректироваки в сущность " + nameof(MicrosoftEntity));
        }

        public Task HandleAsync(Exception ex, MicrosoftEntity toProcess)
        {
            Handle(ex, toProcess);
            return Task.CompletedTask;
        }
    }
}
