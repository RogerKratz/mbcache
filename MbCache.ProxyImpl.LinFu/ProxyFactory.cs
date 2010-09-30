using MbCache.Configuration;
using MbCache.Core;
using MbCache.Logic;

namespace MbCache.ProxyImpl.LinFu
{
    public class ProxyFactory : IProxyFactory
    {
        private ICache _cache;
        private IMbCacheKey _mbCacheKey;

        public void Initialize(ICache cache, IMbCacheKey mbCacheKey)
        {
            _cache = cache;
            _mbCacheKey = mbCacheKey;
        }

        public T CreateProxy<T>(ImplementationAndMethods methodData, params object[] parameters)
        {
            var proxyFactory = new global::LinFu.DynamicProxy.ProxyFactory();
            return (T)proxyFactory.CreateProxy(methodData.ConcreteType, 
                                new CacheInterceptor(_cache, _mbCacheKey, typeof(T), methodData, parameters), 
                                typeof(ICachingComponent));
        }

        public bool AllowNonVirtualMember
        {
            get { return false; }
        }
    }
}