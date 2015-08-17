using System;
using MbCache.Core;
using MbCache.Core.Events;
using MbCache.Configuration;

namespace MbCache.Logic
{
	/// <summary>
	/// Calls <see cref="ICache"/>
	/// </summary>
	[Serializable]
	public class CacheAdapter
	{
		private readonly ICache _cache;
		private readonly ILockObjectGenerator _lockObjectGenerator;

		public CacheAdapter(ICache cache, ILockObjectGenerator lockObjectGenerator)
		{
			_cache = cache;
			_lockObjectGenerator = lockObjectGenerator;
		}

		public object GetAndPutIfNonExisting(EventInformation eventInformation, Func<object> originalMethod)
		{
			var cachedValue = getFromCache(eventInformation);
			if (cachedValue != null)
			{
				return cachedValue;
			}

			var locker = lockObject(eventInformation);
			if (locker == null)
			{
				return executeAndPutInCache(eventInformation, originalMethod);
			}
			lock (locker)
			{
				var cachedValue2 = getFromCache(eventInformation);
				return cachedValue2 ?? 
					executeAndPutInCache(eventInformation, originalMethod);
			}
		}

		private object executeAndPutInCache(EventInformation eventInformation, Func<object> originalMethod)
		{
			var methodResult = originalMethod();
			_cache.Put(new CachedItem(eventInformation, methodResult));
			return methodResult;
		}

		private object lockObject(EventInformation eventInformation)
		{
			//TODO: lock object could be optimized
			return _lockObjectGenerator.GetFor(eventInformation.Type.FullName);
		}

		public void Delete(EventInformation eventInformation)
		{
			if (eventInformation.CacheKey == null) 
				return;

			var locker = lockObject(eventInformation);
			if (locker == null)
			{
				_cache.Delete(eventInformation.CacheKey);
			}
			else
			{
				lock (_lockObjectGenerator.GetFor(eventInformation.Type.FullName))
				{
					_cache.Delete(eventInformation.CacheKey);
				}
			}
		}

		private object getFromCache(EventInformation eventInformation)
		{
			var cacheItem = _cache.Get(eventInformation);
			object cacheValue = null;
			if (cacheItem != null)
			{
				cacheValue = cacheItem.CachedValue;
			}
			return cacheValue;
		}
	}
}