using System;
using System.Reflection;
using MbCache.Configuration;

namespace MbCache.Logic
{
	/// <summary>
	/// Calls <see cref="ICache"/>
	/// </summary>
	public class CacheAdapter
	{
		private readonly ICache _cache;

		public CacheAdapter(ICache cache)
		{
			_cache = cache;
		}

		public object GetAndPutIfNonExisting(KeyAndItsDependingKeys keyAndItsDependingKeys, MethodInfo method, Func<object> originalMethod)
		{
			return _cache.GetAndPutIfNonExisting(keyAndItsDependingKeys, method, originalMethod);
		}

		public void Delete(string cacheKey)
		{
			_cache.Delete(cacheKey);
		}

		public void Clear()
		{
			_cache.Clear();
		}
	}
}