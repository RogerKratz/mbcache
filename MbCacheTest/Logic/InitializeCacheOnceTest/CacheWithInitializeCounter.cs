using System;
using System.Reflection;
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
			MethodInfo cachedMethod, Func<object> originalMethod)
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