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

		public CachedItem Get(EventInformation key)
		{
			var ret = (CachedItem) cache.Get(key.CacheKey);
			if (ret == null)
			{
				_eventListenersCallback.callEventHandlersGet(new CachedItem(key, null), false);				
			}
			else
			{
				_eventListenersCallback.callEventHandlersGet(ret, true);
			}
			return ret;
		}

		public void Put(string key, CachedItem value)
		{
			var unwrappedKeys = _cacheKey.UnwrapKey(key);
			createDependencies(unwrappedKeys);

			var policy = new CacheItemPolicy
								{
									AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(_timeoutMinutes),
									RemovedCallback = arguments => _eventListenersCallback.callEventHandlersDelete(value)
								};
			policy.ChangeMonitors.Add(cache.CreateCacheEntryChangeMonitor(unwrappedKeys));				
			cache.Set(key, value, policy);
			_eventListenersCallback.callEventHandlersPut(value);
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