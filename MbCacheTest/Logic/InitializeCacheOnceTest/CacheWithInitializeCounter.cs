using System;
using MbCache.Configuration;
using MbCache.Core.Events;

namespace MbCacheTest.Logic.InitializeCacheOnceTest
{
	public class CacheWithInitializeCounter : ICache
	{
		public int InitializeCounter { get; private set; }
			
		public void Initialize(EventListenersCallback eventListenersCallback)
		{
			InitializeCounter++;
		}

		public object GetAndPutIfNonExisting(KeyAndItsDependingKeys keyAndItsDependingKeys,
			CachedMethodInformation cachedMethodInformation, Func<object> originalMethod)
		{
			return originalMethod();
		}

		public void Delete(string cacheKey)
		{
		}

		public void Clear()
		{
		}
	}
}