using System.Threading.Tasks;

namespace Sparrow.Parsing.Utils
{
    public abstract class ParsingMiddleware<TResult, TSource>
    {
        protected ParsingMiddleware<TResult, TSource> Next;

        public abstract void Process(
            TResult toProcess, 
            TSource source);

        public abstract Task ProcessAsync(
            TResult toProcess,
            TSource source);

        internal void SetNext(ParsingMiddleware<TResult, TSource> next)
        {
            Next = next;
        }

        protected void InvokeNext(
            TResult toProcess,
            TSource source) => Next?.Process(toProcess, source);

        protected async Task InvokeNextAsync(
            TResult toProcess,
            TSource source)
        {
            if (Next != null)
            {
                await Next.ProcessAsync(toProcess, source);
            }
        }
    }
}
