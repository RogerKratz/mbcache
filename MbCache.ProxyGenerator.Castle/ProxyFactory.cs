using System;
using Castle.DynamicProxy;
using MbCache.Configuration;
using MbCache.Logic;

namespace MbCache.ProxyImpl.Castle
{
    public class ProxyFactory : IProxyFactory
    {
        private readonly ICache _cache;
        private readonly IMbCacheKey _mbCacheKey;
        private static readonly ProxyGenerator _generator = new ProxyGenerator();

        public ProxyFactory(ICache cache,
                            IMbCacheKey mbCacheKey)
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

        public string Name
        {
            get { return "Castle"; }
        }

        private ICachingComponent createCachingComponent(Type type, ImplementationAndMethods details)
        {
            return new CachingComponent(_cache, _mbCacheKey, type, details);
        }
    }
}