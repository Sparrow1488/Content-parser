using System.Threading.Tasks;

namespace Sparrow.Parsing.Utils
{
    public abstract class ParsingMiddleware<TResult, TSource>
    {
        protected MiddlewareContext<TResult, TSource> Context { get; private set; }

        public abstract void Process(TResult toProcess, TSource source);
        public abstract Task ProcessAsync(TResult toProcess, TSource source);

        protected void InvokeNext(
            TResult toProcess,
            TSource source) => Context.Next?.Process(toProcess, source);

        protected async Task InvokeNextAsync(
            TResult toProcess,
            TSource source)
        {
            if (Context.Next != null)
            {
                await Context.Next.ProcessAsync(toProcess, source);
            }
        }

        internal void SetContext(MiddlewareContext<TResult, TSource> context) => Context = context;
    }
}
