using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using MbCache.Core;
using MbCache.Core.Events;

namespace MbCache.Configuration
{
	public class InMemoryCache : ICache
	{
		private const string mainCacheKey = "MainMbCacheKey";
		private static readonly object dependencyValue = new object();
		private static readonly object lockObject = new object();

		private readonly TimeSpan _timeout;
		private EventListenersCallback _eventListenersCallback;

		public InMemoryCache(TimeSpan timeout)
		{
			_timeout = timeout;
			Cache = new MemoryCache("MbCache");
		}
		
		public MemoryCache Cache { get; }

		public void Initialize(EventListenersCallback eventListenersCallback)
		{
			_eventListenersCallback = eventListenersCallback;
		}

		public object GetAndPutIfNonExisting(KeyAndItsDependingKeys keyAndItsDependingKeys, MethodInfo cachedMethod, Func<object> originalMethod)
		{
			var cachedItem = (CachedItem)Cache.Get(keyAndItsDependingKeys.Key);
			if (cachedItem != null)
			{
				_eventListenersCallback.OnCacheHit(cachedItem);
				return cachedItem.CachedValue;
			}

			lock (lockObject)
			{
				var cachedItem2 = (CachedItem)Cache.Get(keyAndItsDependingKeys.Key);
				if (cachedItem2 != null)
				{
					_eventListenersCallback.OnCacheHit(cachedItem2);
					return cachedItem2.CachedValue;
				}
				var addedValue = executeAndPutInCache(keyAndItsDependingKeys, cachedMethod, originalMethod);
				_eventListenersCallback.OnCacheMiss(addedValue);
				return addedValue.CachedValue;
			}
		}

		public void Delete(string cacheKey)
		{
			lock (lockObject)
			{
				Cache.Remove(cacheKey);
			}
		}

		public void Clear()
		{
			Delete(mainCacheKey);
		}

		private CachedItem executeAndPutInCache(KeyAndItsDependingKeys keyAndItsDependingKeys, MethodInfo cachedMethod, Func<object> originalMethod)
		{
			var methodResult = originalMethod();
			var cachedItem = new CachedItem(cachedMethod, methodResult);
			var key = keyAndItsDependingKeys.Key;
			var dependedKeys = keyAndItsDependingKeys.DependingRemoveKeys().ToList();
			dependedKeys.Add(mainCacheKey);
			createDependencies(dependedKeys);

			var policy = new CacheItemPolicy
			{
				AbsoluteExpiration = DateTimeOffset.UtcNow.Add(_timeout),
				RemovedCallback = arguments => _eventListenersCallback.OnCacheRemoval(cachedItem)
			};
			policy.ChangeMonitors.Add(Cache.CreateCacheEntryChangeMonitor(dependedKeys));
			Cache.Set(key, cachedItem, policy);
			return cachedItem;
		}

		private void createDependencies(IEnumerable<string> unwrappedKeys)
		{
			foreach (var key in unwrappedKeys)
			{
				var policy = new CacheItemPolicy { Priority = CacheItemPriority.NotRemovable };
				Cache.Add(key, dependencyValue, policy);
			}
		}
	}
}