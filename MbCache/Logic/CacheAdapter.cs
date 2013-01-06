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

		public CacheAdapter(ICache cache)
		{
			_cache = cache;
		}

		public object Get(EventInformation eventInformation)
		{
			var cacheItem = _cache.Get(eventInformation);
			object cacheValue = null;
			if(cacheItem != null)
			{
				cacheValue = cacheItem.CachedValue;
			}
			return cacheValue;
		}

		public void Put(CachedItem cachedItem)
		{
			_cache.Put(cachedItem);
		}

		public void Delete(EventInformation eventInformation)
		{
			if (eventInformation.CacheKey == null) 
				return;
			_cache.Delete(eventInformation.CacheKey);
		}
	}
}