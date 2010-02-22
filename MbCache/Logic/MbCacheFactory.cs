using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.CoreImpl;

namespace MbCache.Logic
{
    public class MbCacheFactory : IMbCacheFactory
    {
        private static readonly ProxyGenerator _generator = new ProxyGenerator();
        private readonly ICache _cache;
        private readonly IMbCacheRegion _cacheRegion;
        private readonly IDictionary<Type, ImplementationAndMethods> _methods;

        public MbCacheFactory(ICache cache, 
                            IMbCacheRegion cacheRegion,
                            IDictionary<Type, ImplementationAndMethods> methods)
        {
            _cache = cache;
            _cacheRegion = cacheRegion;
            _methods = methods;
        }

        public T Create<T>() where T : class
        {
            return createInstance<T>();
        }

        public void Invalidate<T>()
        {
            foreach (var method in _methods[typeof(T)].Methods)
            {
                _cache.Delete(_cacheRegion.Region(method));
            }
        }

        private T createInstance<T>() where T : class
        {
            var cacheInterceptor = new CacheInterceptor(_cache, _cacheRegion, _methods[typeof(T)].Methods);
            var options = new ProxyGenerationOptions(new CacheProxyGenerationHook());
            var type = typeof (T);
            if(type.IsInterface)
            {
                object impl = Activator.CreateInstance(_methods[type].ImplementationType);
                return (T)_generator.CreateInterfaceProxyWithTarget(type, impl, options, cacheInterceptor);
            }
            return (T)_generator.CreateClassProxy(type, options, cacheInterceptor);
        }
    }
}