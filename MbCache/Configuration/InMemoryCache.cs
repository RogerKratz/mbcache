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
		private readonly ILockObjectGenerator _lockObjectGenerator;
		private readonly int _timeoutMinutes;
		private ICacheKeyUnwrapper _cacheKeyUnwrapper;
		private static readonly MemoryCache cache = MemoryCache.Default;
		private static readonly object dependencyValue = new object();
		private EventListenersCallback _eventListenersCallback;

		public InMemoryCache(ILockObjectGenerator lockObjectGenerator, int timeoutMinutes)
		{
			_lockObjectGenerator = lockObjectGenerator;
			_timeoutMinutes = timeoutMinutes;
		}

		public void Initialize(EventListenersCallback eventListenersCallback, ICacheKeyUnwrapper cacheKeyUnwrapper)
		{
			_cacheKeyUnwrapper = cacheKeyUnwrapper;
			_eventListenersCallback = eventListenersCallback;
		}

		public CachedItem GetAndPutIfNonExisting(EventInformation eventInformation, Func<object> originalMethod)
		{
			var cachedItem = (CachedItem)cache.Get(eventInformation.CacheKey);
			if (cachedItem != null)
			{
				_eventListenersCallback.OnGetSuccessful(cachedItem);
				return cachedItem;
			}

			var locker = lockObject(eventInformation);
			if (locker == null)
			{
				_eventListenersCallback.OnGetUnsuccessful(eventInformation);
				return executeAndPutInCache(eventInformation, originalMethod);
			}
			lock (locker)
			{
				_eventListenersCallback.OnGetUnsuccessful(eventInformation);
				var cachedValue2 = (CachedItem)cache.Get(eventInformation.CacheKey);
				return cachedValue2 ?? executeAndPutInCache(eventInformation, originalMethod);
			}
		}

		public void Delete(EventInformation eventInformation)
		{
			var locker = lockObject(eventInformation);
			if (locker == null)
			{
				cache.Remove(eventInformation.CacheKey);
			}
			else
			{
				lock (locker)
				{
					cache.Remove(eventInformation.CacheKey);
				}
			}
		}

		private object lockObject(EventInformation eventInformation)
		{
			return _lockObjectGenerator.GetFor(eventInformation.Type.FullName);
		}

		private CachedItem executeAndPutInCache(EventInformation eventInformation, Func<object> originalMethod)
		{
			var methodResult = originalMethod();
			var cachedItem = new CachedItem(eventInformation, methodResult);
			var key = cachedItem.EventInformation.CacheKey;
			var unwrappedKeys = _cacheKeyUnwrapper.UnwrapKey(key);
			createDependencies(unwrappedKeys);

			var policy = new CacheItemPolicy
			{
				AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(_timeoutMinutes),
				RemovedCallback = arguments => _eventListenersCallback.OnDelete(cachedItem)
			};
			policy.ChangeMonitors.Add(cache.CreateCacheEntryChangeMonitor(unwrappedKeys));
			cache.Set(key, cachedItem, policy);
			_eventListenersCallback.OnPut(cachedItem);
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