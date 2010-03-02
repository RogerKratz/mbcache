using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Castle.DynamicProxy;
using log4net;
using MbCache.Core;

namespace MbCache.Logic
{
    public class MbCacheFactory : IMbCacheFactory
    {
        private static ILog log = LogManager.GetLogger(typeof(MbCacheFactory));
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
            log.Debug("Invalidating all cache entries for " + type);
            foreach (var method in _methods[type].Methods)
            {
                _cache.Delete(_cacheRegion.Region(type, method));
            }
        }

        public void Invalidate<T>(Expression<Func<T, object>> method)
        {
            var memberInfo = ExpressionHelper.MemberName(method.Body);
            var type = typeof (T);
            log.Debug("Invalidating cache entries for " + type + "." + memberInfo.Name + "()");
            _cache.Delete(_cacheRegion.Region(type, memberInfo));
        }

        private T createInstance<T>()
        {
            var type = typeof(T);
            var data = _methods[type];
            var cacheInterceptor = new CacheInterceptor(_cache, _cacheRegion, type);
            var options = new ProxyGenerationOptions(new CacheProxyGenerationHook(data.Methods));
            if(type.IsInterface)
            {
                return (T)_generator.CreateInterfaceProxyWithTarget(typeof(T), data.Implementation, options, cacheInterceptor);
            }
            return (T)_generator.CreateClassProxy(type, new Type[0], options, data.CtorParameters, cacheInterceptor);
        }
    }
}