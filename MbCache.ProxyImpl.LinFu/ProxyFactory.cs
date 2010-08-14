using System;
using MbCache.Configuration;
using MbCache.Logic;

namespace MbCache.ProxyImpl.LinFu
{
    public class ProxyFactory : IProxyFactory
    {
        private ICache _cache;
        private IMbCacheKey _mbCacheKey;

        public ProxyFactory(ICache cache,
                            IMbCacheKey mbCacheKey)
        {
            _cache = cache;
            _mbCacheKey = mbCacheKey;
        }

        public T CreateProxy<T>(ImplementationAndMethods methodData, params object[] parameters)
        {
            var proxyFactory = new global::LinFu.DynamicProxy.ProxyFactory();
            return (T)proxyFactory.CreateProxy(methodData.ConcreteType, new CacheInterceptorAndComponent(_cache, _mbCacheKey, typeof(T), methodData, parameters), typeof(ICachingComponent));
        }
    }
}