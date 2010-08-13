using System;
using MbCache.Configuration;

namespace MbCache.Logic
{
    public class ProxyFactoryFactory
    {
        public IProxyFactory CreateProxyFactory(string proxyFactoryClass,
                                                ICache cache,
                                                IMbCacheKey cacheKey)
        {
            var proxyFactoryType = Type.GetType(proxyFactoryClass, true, true);
            var proxyFactory = (IProxyFactory)Activator.CreateInstance(proxyFactoryType, cache, cacheKey);
            return proxyFactory;
        }
    }
}