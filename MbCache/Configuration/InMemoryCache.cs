using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace MbCache.Configuration
{
	[Serializable]
	public class InMemoryCache : ICache
	{
		private readonly int _timeoutMinutes;
		private ICacheKey _cacheKey;
		private static readonly MemoryCache cache = MemoryCache.Default;
		private static readonly object dependencyValue = new object();

		public InMemoryCache(int timeoutMinutes)
		{
			_timeoutMinutes = timeoutMinutes;
		}

		public void Initialize(ICacheKey cacheKey)
		{
			_cacheKey = cacheKey;
		}

		public object Get(string key)
		{
			return cache.Get(key);
		}

		public void Put(string key, object value)
		{
			var unwrappedKeys = _cacheKey.UnwrapKey(key);
			createDependencies(unwrappedKeys);

			var policy = new CacheItemPolicy
			             	{
			             		AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(_timeoutMinutes)
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