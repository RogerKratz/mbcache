using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using MbCache.Core;
using MbCache.Core.Events;

namespace MbCache.Configuration
{
	[Serializable]
	public class InMemoryCache : ICache
	{
		private readonly int _timeoutMinutes;
		private static readonly MemoryCache cache = MemoryCache.Default;
		private static readonly object dependencyValue = new object();
		private static readonly object lockObject = new object();
		private EventListenersCallback _eventListenersCallback;
		private const string mainCacheKey = "MainMbCacheKey";

		public InMemoryCache(int timeoutMinutes)
		{
			_timeoutMinutes = timeoutMinutes;
		}

		public void Initialize(EventListenersCallback eventListenersCallback)
		{
			_eventListenersCallback = eventListenersCallback;
		}

		public object GetAndPutIfNonExisting(KeyAndItsDependingKeys keyAndItsDependingKeys, CachedMethodInformation cachedMethodInformation, Func<object> originalMethod)
		{
			var cachedItem = (CachedItem)cache.Get(keyAndItsDependingKeys.Key);
			if (cachedItem != null)
			{
				_eventListenersCallback.OnCacheHit(cachedItem);
				return cachedItem.CachedValue;
			}

			lock (lockObject)
			{
				var cachedItem2 = (CachedItem)cache.Get(keyAndItsDependingKeys.Key);
				if (cachedItem2 != null)
				{
					_eventListenersCallback.OnCacheHit(cachedItem2);
					return cachedItem2.CachedValue;
				}
				var addedValue = executeAndPutInCache(keyAndItsDependingKeys, cachedMethodInformation, originalMethod);
				_eventListenersCallback.OnCacheMiss(addedValue);
				return addedValue.CachedValue;
			}
		}

		public void Delete(string cacheKey)
		{
			lock (lockObject)
			{
				cache.Remove(cacheKey);
			}
		}

		public void Clear()
		{
			lock (lockObject)
			{
				cache.Remove(mainCacheKey);
			}
		}

		private CachedItem executeAndPutInCache(KeyAndItsDependingKeys keyAndItsDependingKeys, CachedMethodInformation cachedMethodInformation, Func<object> originalMethod)
		{
			var methodResult = originalMethod();
			var cachedItem = new CachedItem(cachedMethodInformation, methodResult);
			var key = keyAndItsDependingKeys.Key;
			var dependedKeys = keyAndItsDependingKeys.DependingRemoveKeys().ToList();
			dependedKeys.Add(mainCacheKey);
			createDependencies(dependedKeys);

			var policy = new CacheItemPolicy
			{
				AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(_timeoutMinutes),
				RemovedCallback = arguments => _eventListenersCallback.OnCacheRemoval(cachedItem)
			};
			policy.ChangeMonitors.Add(cache.CreateCacheEntryChangeMonitor(dependedKeys));
			cache.Set(key, cachedItem, policy);
			return cachedItem;
		}

		private static void createDependencies(IEnumerable<string> unwrappedKeys)
		{
			foreach (var key in unwrappedKeys)
			{
				var policy = new CacheItemPolicy { Priority = CacheItemPriority.NotRemovable };
				cache.Add(key, dependencyValue, policy);
			}
		}
	}
}