using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace MbCache.Configuration
{
	[Serializable]
	public class InMemoryCache : ICache
	{
		private readonly int _timeoutMinutes;
		private static readonly MemoryCache _cache = MemoryCache.Default;

		public InMemoryCache(int timeoutMinutes)
		{
			_timeoutMinutes = timeoutMinutes;
		}

		public object Get(string key)
		{
			return _cache.Get(key);
		}

		public void Put(string key, object value)
		{
			_cache.Set(key, value, DateTimeOffset.Now.AddMinutes(_timeoutMinutes));
		}

		public void Delete(string keyStartingWith)
		{
			var keyList = new List<string>();
			foreach (var cacheItem in _cache)
			{
				var keyString = cacheItem.Key;
				if(keyString.StartsWith(keyStartingWith, StringComparison.Ordinal))
				{
					keyList.Add(keyString);
				}
			}
			foreach (var key in keyList)
			{
				_cache.Remove(key);
			}
		}
	}
}