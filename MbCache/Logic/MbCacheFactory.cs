using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MbCache.Configuration;
using MbCache.Core;

namespace MbCache.Logic
{
    public class MbCacheFactory : IMbCacheFactory
    {
        private readonly IProxyFactory _proxyFactory;
        private readonly LogAndStatisticCacheDecorator _cache;
        private readonly IMbCacheKey _cacheKey;
        private readonly IDictionary<Type, ImplementationAndMethods> _methods;

        public MbCacheFactory(IProxyFactory proxyFactory,
                            LogAndStatisticCacheDecorator cache, 
                            IMbCacheKey cacheKey,
                            IDictionary<Type, ImplementationAndMethods> methods)
        {
            _cache = cache;
            _cacheKey = cacheKey;
            _methods = methods;
            _proxyFactory = proxyFactory;
        }

        public T Create<T>(params object[] parameters)
        {
            return _proxyFactory.CreateProxy<T>(_methods[typeof(T)], parameters);
        }

        public void Invalidate<T>()
        {
            _cache.Delete(_cacheKey.Key(typeof(T)));
        }

        public void Invalidate(object component)
        {
            castToCachingComponentOrThrow(component).Invalidate();
        }

        public void Invalidate<T>(T component, Expression<Func<T, object>> method)
        {
            castToCachingComponentOrThrow(component).Invalidate(method);
        }

        public bool IsKnownInstance(object component)
        {
            return component is ICachingComponent;
        }

        public IStatistics Statistics
        {
            get { return _cache; }
        }

        private static ICachingComponent castToCachingComponentOrThrow(object component)
        {
            var comp = component as ICachingComponent;
            if (comp == null)
                throw new ArgumentException(component + " is not an ICachingComponent. Unknown object for MbCache.");
            return comp;
        }

    }
}