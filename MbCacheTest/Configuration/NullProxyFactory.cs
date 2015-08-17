using MbCache.Configuration;
using MbCache.Logic;

namespace MbCacheTest.Configuration
{
	public class NullProxyFactory : IProxyFactory
	{
		public void Initialize(CacheAdapter cache, ICacheKey cacheKey)
		{
		}

		public T CreateProxy<T>(ConfigurationForType configurationForType, params object[] parameters) where T : class
		{
			throw new System.NotImplementedException();
		}

		public bool AllowNonVirtualMember
		{
			get { throw new System.NotImplementedException(); }
		}

		public T CreateProxyWithTarget<T>(T uncachedComponent, ConfigurationForType configurationForType) where T : class
		{
			throw new System.NotImplementedException();
		}
	}
}