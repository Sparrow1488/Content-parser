using Microsoft.Extensions.Logging;
using Sparrow.Parsing.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Example.Nude.Middlewares
{
    internal class ExceptionHandleMiddleware : IExceptionHandleMiddleware
    {
        public ExceptionHandleMiddleware(ILogger<ExceptionHandleMiddleware> logger)
        {
            _logger = logger;
        }

        private readonly ILogger<ExceptionHandleMiddleware> _logger;

        public IEnumerable<Exception> CanHandle => new List<Exception>()
        {
            new Exception()
        };

        public void Handle(Exception ex)
        {
            _logger.LogCritical("Необработанное исключение парсера", ex);
        }

        public Task HandleAsync(Exception ex)
        {
            Handle(ex);
            return Task.CompletedTask;
        }
    }
}
