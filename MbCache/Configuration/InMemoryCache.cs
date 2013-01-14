using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using MbCache.Core;
using MbCache.Core.Events;

namespace MbCache.Configuration
{
	[Serializable]
	public class InMemoryCache : ICache
	{
		private readonly int _timeoutMinutes;
		private ICacheKey _cacheKey;
		private static readonly MemoryCache cache = MemoryCache.Default;
		private static readonly object dependencyValue = new object();
		private EventListenersCallback _eventListenersCallback;

		public InMemoryCache(int timeoutMinutes)
		{
			_timeoutMinutes = timeoutMinutes;
		}

		public void Initialize(ICacheKey cacheKey, EventListenersCallback eventListenersCallback)
		{
			_cacheKey = cacheKey;
			_eventListenersCallback = eventListenersCallback;
		}

		public CachedItem Get(EventInformation eventInformation)
		{
			var ret = (CachedItem)cache.Get(eventInformation.CacheKey);
			if (ret == null)
			{
				_eventListenersCallback.OnGetUnsuccessful(eventInformation);				
			}
			else
			{
				_eventListenersCallback.OnGetSuccessful(ret);
			}
			return ret;
		}

		public void Put(CachedItem cachedItem)
		{
			var key = cachedItem.EventInformation.CacheKey;
			var unwrappedKeys = _cacheKey.UnwrapKey(key);
			createDependencies(unwrappedKeys);

			var policy = new CacheItemPolicy
								{
									AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(_timeoutMinutes),
									RemovedCallback = arguments => _eventListenersCallback.OnDelete(cachedItem)
								};
			policy.ChangeMonitors.Add(cache.CreateCacheEntryChangeMonitor(unwrappedKeys));
			cache.Set(key, cachedItem, policy);
			_eventListenersCallback.OnPut(cachedItem);
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