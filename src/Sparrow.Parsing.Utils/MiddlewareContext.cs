namespace Sparrow.Parsing.Utils
{
    public class MiddlewareContext<TResult, TSource>
    {
        public MiddlewareContext(
            ParsingMiddleware<TResult, TSource> next,
            TSource source)
        {
            Next = next;
            Source = source;
        }

        public ParsingMiddleware<TResult, TSource> Next { get; }
        public TSource Source { get; }
    }
}
