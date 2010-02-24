using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Castle.DynamicProxy;
using MbCache.Core;
using System.Linq;

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
            Type type = typeof (T);
            foreach (var method in _methods[type].Methods)
            {
                _cache.Delete(_cacheRegion.Region(type, method));
            }
        }

        public void Invalidate<T>(Expression<Func<T, object>> method)
        {  
            _cache.Delete(_cacheRegion.Region(typeof(T), ExpressionHelper.MemberName(method.Body)));
        }

        private T createInstance<T>()
        {
            var type = typeof(T);
            var data = _methods[type];
            var cacheInterceptor = new CacheInterceptor(_cache, _cacheRegion, data.Methods);
            var options = new ProxyGenerationOptions(new CacheProxyGenerationHook());
            if(type.IsInterface)
            {
                return _generator.CreateInterfaceProxyWithTarget<T>(data.Implementation, options, cacheInterceptor);
            }
            return (T)_generator.CreateClassProxy(type, new Type[0], options, data.CtorParameters, cacheInterceptor);
        }
    }
}