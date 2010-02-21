using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using MbCache.Core;
using MbCache.CoreImpl;

namespace MbCache.Logic
{
    public class MbCacheFactory : IMbCacheFactory
    {
        private static readonly ProxyGenerator _generator = new ProxyGenerator();
        private readonly ICache _cache;
        private readonly IDictionary<Type, ImplementationAndMethods> _methods;

        public MbCacheFactory(ICache cache, IDictionary<Type, ImplementationAndMethods> methods)
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
            var cacheInterceptor = new CacheInterceptor(_cache, _methods[typeof(T)].Methods);
            var options = new ProxyGenerationOptions(new CacheProxyGenerationHook());
            if(typeof(T).IsInterface)
            {
                object impl = Activator.CreateInstance(_methods[typeof (T)].ImplementationType);
                return (T)_generator.CreateInterfaceProxyWithTarget(typeof (T), impl, options, cacheInterceptor);
            }
            else
            {
                return (T)_generator.CreateClassProxy(typeof(T), options, cacheInterceptor);        
            }
        }
    }
}