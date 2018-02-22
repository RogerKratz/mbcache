using System;
using System.Collections.Generic;
using System.Reflection;
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

		public object GetAndPutIfNonExisting(KeyAndItsDependingKeys keyAndItsDependingKeys, MethodInfo method, IEnumerable<object> arguments, Func<object> originalMethod)
		{
			var eventInformation = new CachedMethodInformation(method, arguments);
			return _cache.GetAndPutIfNonExisting(keyAndItsDependingKeys, eventInformation, originalMethod);
		}

		public void Delete(string cacheKey)
		{
			if (cacheKey == null)
				return;
			_cache.Delete(cacheKey);
		}

		public void Clear()
		{
			_cache.Clear();
		}
	}
}