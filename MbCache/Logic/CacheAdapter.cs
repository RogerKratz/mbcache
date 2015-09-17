using System;
using System.Collections.Generic;
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

		public object GetAndPutIfNonExisting(EventInformation eventInformation, Func<IEnumerable<string>> dependingRemoveKeys, Func<object> originalMethod)
		{
			var cachedItem = _cache.GetAndPutIfNonExisting(eventInformation, dependingRemoveKeys, originalMethod);
			return cachedItem?.CachedValue;
		}

		public void Delete(EventInformation eventInformation)
		{
			if (eventInformation.CacheKey == null)
				return;
			_cache.Delete(eventInformation);
		}

		public void Clear()
		{
			_cache.Clear();
		}
	}
}