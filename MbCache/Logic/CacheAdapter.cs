using System;
using MbCache.Core;
using MbCache.Core.Events;
using MbCache.Configuration;

namespace MbCache.Logic
{
	/// <summary>
	/// Calls <see cref="ICache"/>, handles <code>null</code> in cache.
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

		public void Put(EventInformation eventInformation, object value)
		{
			var cachedValue = value;
			_cache.Put(eventInformation.CacheKey, new CachedItem(eventInformation, cachedValue));
		}

		public void Delete(EventInformation eventInformation)
		{
			if (eventInformation.CacheKey == null) 
				return;
			_cache.Delete(eventInformation.CacheKey);
		}
	}
}