using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sparrow.Parsing.Utils
{
    public sealed class ParsingPipeline<TResult, TSource>
    {
        public ParsingPipeline(TSource source)
        {
            _source = source;
            _middlewares = new List<ParsingMiddleware<TResult, TSource>>();
            _middlewaresTypes = new List<Type>();
        }

        private readonly TSource _source;
        private IList<ParsingMiddleware<TResult, TSource>> _middlewares;

        private IList<Type> _middlewaresTypes;

        public ParsingPipeline<TResult, TSource> Use<TMiddleware>()
            where TMiddleware : ParsingMiddleware<TResult, TSource>
        {
            _middlewaresTypes.Add(typeof(TMiddleware));
            return this;
        }

        public TResult Start()
        {
            //CreateMiddlewares();
            //var resultEntity = Activator.CreateInstance<TResult>();
            //for (int i = 0; i < _middlewaresTypes.Count; i++)
            //{
            //    var currentMiddleware = _middlewares[i];
            //    if (HasNext(i, _middlewares.Count))
            //        currentMiddleware.SetNext(_middlewares[i + 1]);

            //    currentMiddleware.Process(resultEntity, _source);
            //}
            //return resultEntity;
            throw new NotImplementedException();
        }

        public async Task<TResult> StartAsync()
        {
            CreateMiddlewares();
            var resultEntity = Activator.CreateInstance<TResult>();
            for (int i = 0; i < _middlewares.Count; i++)
            {
                var currentMiddleware = _middlewares[i];
                if (HasNext(i, _middlewares.Count))
                    currentMiddleware.SetNext(_middlewares[i + 1]);
            }

            var firstMiddleware = _middlewares.First();
            await firstMiddleware.ProcessAsync(resultEntity, _source);
            return resultEntity;
        }

        private void CreateMiddlewares()
        {
            foreach (var type in _middlewaresTypes)
            {
                var instance = (ParsingMiddleware<TResult, TSource>)Activator.CreateInstance(type);
                _middlewares.Add(instance);
            }
        }

        private bool HasNext(int current, int total)
        {
            if (current + 1 < total)
                return true;
            return false;
        }
    }
}
