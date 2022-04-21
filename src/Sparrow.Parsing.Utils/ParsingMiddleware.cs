using System.Threading.Tasks;

namespace Sparrow.Parsing.Utils
{
    public abstract class ParsingMiddleware<TResult, TSource>
    {
        protected MiddlewareContext<TResult, TSource> Context { get; private set; }

        public virtual void Process(TResult toProcess) => ProcessAsync(toProcess).ConfigureAwait(false);
        public abstract Task ProcessAsync(TResult toProcess);

        protected void InvokeNext(
            TResult toProcess) => Context.Next?.Process(toProcess);

        protected async Task InvokeNextAsync(
            TResult toProcess)
        {
            if (Context.Next != null)
            {
                Context.Next.Context.Services = Context.Services;
                await Context.Next.ProcessAsync(toProcess);
            }
        }

        internal void SetContext(MiddlewareContext<TResult, TSource> context) => Context = context;
    }
}
