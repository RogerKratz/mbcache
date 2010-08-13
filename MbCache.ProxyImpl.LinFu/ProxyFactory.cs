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
            throw new NotImplementedException();
        }
    }
}