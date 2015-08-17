using System;
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

		public object GetAndPutIfNonExisting(EventInformation eventInformation, Func<object> originalMethod)
		{
			var cachedItem = _cache.GetAndPutIfNonExisting(eventInformation, originalMethod);
			return cachedItem != null ? cachedItem.CachedValue : null;
		}

		public void Delete(EventInformation eventInformation)
		{
			if (eventInformation.CacheKey == null)
				return;
			_cache.Delete(eventInformation);
		}
	}
}