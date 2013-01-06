using System;
using System.Collections.Generic;
using MbCache.Core;
using MbCache.Core.Events;
using MbCache.Configuration;
using System.Linq;

namespace MbCache.Logic
{
	/// <summary>
	/// Calls <see cref="ICache"/>, registered <see cref="IEventListener"/>s handles <code>null</code> in cache.
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
			if(cacheItem!=null)
			{
				cacheValue = cacheItem.CachedValue;
			}
			//callEventHandlersGet(eventInformation, cacheValue);
			if (cacheValue is NullValue)
			{
				cacheValue = null;
			}
			return cacheValue;
		}

		public void Put(EventInformation eventInformation, object value)
		{
			var cachedValue = value ?? new NullValue();
			_cache.Put(eventInformation.CacheKey, new CachedItem(eventInformation, cachedValue));
			//callEventHandlersPut(eventInformation, value);
		}

		public void Delete(EventInformation eventInformation)
		{
			if (eventInformation.CacheKey == null) 
				return;
			_cache.Delete(eventInformation.CacheKey);
			//todo: remove eventinfo from this method?
			//callEventHandlersDelete(eventInformation);
		}
	}
}