using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using MbCache.Core;

namespace MbCache.Logic
{
    public class MbCacheFactory : IMbCacheFactory
    {
        private static readonly ProxyGenerator _generator = new ProxyGenerator();
        private readonly ICache _cache;
        private readonly IDictionary<Type, ICollection<string>> _methods;

        public MbCacheFactory(ICache cache, IDictionary<Type, ICollection<string>> methods)
        {
            _cache = cache;
            _methods = methods;
        }

        public T Create<T>() where T : class
        {
            return createInstance<T>();
        }

        private T createInstance<T>() where T : class
        {
            var cacheInterceptor = new CacheInterceptor(_cache, _methods[typeof(T)]);
            var options = new ProxyGenerationOptions(new CacheProxyGenerationHook());
            return (T)_generator.CreateClassProxy(typeof(T), options, cacheInterceptor);
        }
    }
}