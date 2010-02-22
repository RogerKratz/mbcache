using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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

        public T Create<T>()
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

        public void Invalidate<T>(Expression<Func<T, object>> method)
        {
            _cache.Delete(_cacheRegion.Region(ExpressionHelper.MemberName(method.Body)));
        }

        private T createInstance<T>()
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