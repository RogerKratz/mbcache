using System;
using MbCache.Configuration;
using MbCache.Logic;

namespace MbCache.ProxyImpl.LinFu
{
    public class ProxyFactory : IProxyFactory
    {
        private readonly ICache _cache;
        private readonly IMbCacheKey _mbCacheKey;

        public ProxyFactory(ICache cache,
                            IMbCacheKey mbCacheKey)
        {
            _cache = cache;
            _mbCacheKey = mbCacheKey;
        }

        public T CreateProxy<T>(ImplementationAndMethods methodData, params object[] parameters)
        {
            var proxyFactory = new global::LinFu.DynamicProxy.ProxyFactory();
            return (T)proxyFactory.CreateProxy(methodData.ConcreteType, new CacheInterceptor(_cache, _mbCacheKey, typeof(T), methodData, parameters), typeof(ICachingComponent));
        }

        public bool AllowNonVirtualMember
        {
            get { return false; }
        }

        public string Name
        {
            get { return "Linfu"; }
        }
    }
}