using System;
using System.Collections.Concurrent;
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
		private readonly TimeSpan _timeout;
		private static readonly MemoryCache cache = MemoryCache.Default;
		private static readonly object dependencyValue = new object();
		private readonly ConcurrentDictionary<string, object> lockObjects = new ConcurrentDictionary<string, object>();
		private EventListenersCallback _eventListenersCallback;
		private const string mainCacheKey = "MainMbCacheKey";

		public InMemoryCache(TimeSpan timeout)
		{
			_timeout = timeout;
		}

		public void Initialize(EventListenersCallback eventListenersCallback)
		{
			_eventListenersCallback = eventListenersCallback;
		}

		public object GetAndPutIfNonExisting(KeyAndItsDependingKeys keyAndItsDependingKeys, MethodInfo cachedMethod, Func<object> originalMethod)
		{
			var cachedItem = (CachedItem)cache.Get(keyAndItsDependingKeys.Key);
			if (cachedItem != null)
			{
				_eventListenersCallback.OnCacheHit(cachedItem);
				return cachedItem.CachedValue;
			}

			//this may result in two different locks for same key if old entry was removed at this time.
			//let's see if it's a real problem...
			var locker = lockObjects.GetOrAdd(keyAndItsDependingKeys.Key, key => new object());
			Func<object> actionOutsideLock;
			lock (locker)
			{
				var cachedItem2 = (CachedItem)cache.Get(keyAndItsDependingKeys.Key);
				if (cachedItem2 == null)
				{
					var addedValue = executeAndPutInCache(keyAndItsDependingKeys, cachedMethod, originalMethod);
					actionOutsideLock = () =>
					{
						_eventListenersCallback.OnCacheMiss(addedValue);
						return addedValue.CachedValue;
					};
				}
				else
				{
					actionOutsideLock = () =>
					{
						_eventListenersCallback.OnCacheHit(cachedItem2);
						return cachedItem2.CachedValue;
					};
				}
			}

			return actionOutsideLock();
		}

		public void Delete(string cacheKey)
		{
			cache.Remove(cacheKey);
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
				RemovedCallback = arguments =>
				{
					_eventListenersCallback.OnCacheRemoval(cachedItem);
					lockObjects.TryRemove(key, out _);
				}
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