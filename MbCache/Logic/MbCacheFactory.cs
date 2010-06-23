using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Castle.DynamicProxy;
using MbCache.Core;

namespace MbCache.Logic
{
    public class MbCacheFactory : IMbCacheFactory
    {
        private static readonly ProxyGenerator _generator = new ProxyGenerator();
        private readonly ICache _cache;
        private readonly IMbCacheKey _cacheKey;
        private readonly IDictionary<Type, ImplementationAndMethods> _methods;

        public MbCacheFactory(ICache cache, 
                            IMbCacheKey cacheKey,
                            IDictionary<Type, ImplementationAndMethods> methods)
        {
            _cache = cache;
            _cacheKey = cacheKey;
            _methods = methods;
        }

        public T Create<T>(params object[] parameters)
        {
            var type = typeof(T);
            var data = _methods[type];
            var cacheInterceptor = new CacheInterceptor(_cache, _cacheKey, type);
            var options = new ProxyGenerationOptions(new CacheProxyGenerationHook(data.Methods));
            options.AddMixinInstance(createCachingComponent(type, data));
            return (T)_generator.CreateClassProxy(data.ConcreteType, options, parameters, cacheInterceptor);
        }

        public void Invalidate<T>()
        {
            Type type = typeof (T);
            foreach (var method in _methods[type].Methods)
            {
                var cacheKey = _cacheKey.CacheKey(type, method);
                _cache.Delete(cacheKey);
            }
        }

        public void Invalidate<T>(Expression<Func<T, object>> method)
        {
            var memberInfo = ExpressionHelper.MemberName(method.Body);
            var type = typeof (T); 
            _cache.Delete(_cacheKey.CacheKey(type, memberInfo));
        }


        private ICachingComponent createCachingComponent(Type type, ImplementationAndMethods details)
        {
            var ret = new CachingComponent(_cache, _cacheKey, type, details)
                          {
                              UniqueId = details.CachePerInstance ? Guid.NewGuid().ToString() : "Global"
                          };
            return ret;
        }
    }
}