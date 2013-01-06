using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using MbCache.Core;
using MbCache.Core.Events;
using MbCache.Logic;

namespace MbCache.Configuration
{
	[Serializable]
	public class InMemoryCache : ICache
	{
		private readonly int _timeoutMinutes;
		private ICacheKey _cacheKey;
		private static readonly MemoryCache cache = MemoryCache.Default;
		private static readonly object dependencyValue = new object();
		private CacheAdapter _cacheAdapter;

		public InMemoryCache(int timeoutMinutes)
		{
			_timeoutMinutes = timeoutMinutes;
		}

		public void Initialize(ICacheKey cacheKey, CacheAdapter cacheAdapter)
		{
			_cacheKey = cacheKey;
			_cacheAdapter = cacheAdapter;
		}

		public CachedItem Get(string key)
		{
			return (CachedItem) cache.Get(key);
		}

		public void Put(string key, CachedItem value)
		{
			var unwrappedKeys = _cacheKey.UnwrapKey(key);
			createDependencies(unwrappedKeys);

			var policy = new CacheItemPolicy
								{
									AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(_timeoutMinutes),
									RemovedCallback = arguments => _cacheAdapter.callEventHandlersDelete(value.EventInformation)
								};
			policy.ChangeMonitors.Add(cache.CreateCacheEntryChangeMonitor(unwrappedKeys));				
			cache.Set(key, value, policy);
		}

		private static void createDependencies(IEnumerable<string> unwrappedKeys)
		{
			foreach (var key in unwrappedKeys)
			{
				var policy = new CacheItemPolicy { Priority = CacheItemPriority.NotRemovable };
				cache.Add(key, dependencyValue, policy);
			}
		}

		public void Delete(string keyStartingWith)
		{
			cache.Remove(keyStartingWith);
		}
	}
}