using System;
using Castle.DynamicProxy;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Logic;

namespace MbCache.ProxyImpl.Castle
{
    public class ProxyFactory : IProxyFactory
    {
        private static readonly ProxyGenerator _generator = new ProxyGenerator();
        private ICache _cache;
        private IMbCacheKey _mbCacheKey;

        public void Initialize(ICache cache, IMbCacheKey mbCacheKey)
        {
            _cache = cache;
            _mbCacheKey = mbCacheKey;
        }

        public T CreateProxy<T>(ImplementationAndMethods methodData,
                                params object[] parameters)
        {
            var type = typeof(T);
            var cacheInterceptor = new CacheInterceptor(_cache, _mbCacheKey, type);
            var options = new ProxyGenerationOptions(new CacheProxyGenerationHook(methodData.Methods));
            options.AddMixinInstance(createCachingComponent(type, methodData));
            return (T)_generator.CreateClassProxy(methodData.ConcreteType, options, parameters, cacheInterceptor);

        }

        public bool AllowNonVirtualMember
        {
            get { return true; }
        }

        private ICachingComponent createCachingComponent(Type type, ImplementationAndMethods details)
        {
            return new CachingComponent(_cache, _mbCacheKey, type, details);
        }
    }
}