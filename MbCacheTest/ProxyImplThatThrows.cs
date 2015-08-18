using System;
using MbCache.Configuration;
using MbCache.Logic;

namespace MbCacheTest
{
	public class ProxyImplThatThrows : IProxyFactory
	{
		public void Initialize(CacheAdapter cache, ICacheKey cacheKey)
		{
		}

		public T CreateProxy<T>(ConfigurationForType configurationForType, params object[] parameters) where T : class
		{
			throw new NotSupportedException("This test creates proxies. Please derive from " + typeof(FullTest).Name + " instead.");
		}

		public T CreateProxyWithTarget<T>(T uncachedComponent, ConfigurationForType configurationForType) where T : class
		{
			throw new NotSupportedException("This test creates proxies. Please derive from " + typeof(FullTest).Name + " instead.");
		}
	}
}