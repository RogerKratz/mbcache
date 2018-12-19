using System;
using System.Collections.Generic;
using System.Reflection;
using MbCache.Configuration;

namespace MbCache.Logic
{
	[Serializable]
	public class ConfigurationForType
	{
		private ICache _cache;

		public ConfigurationForType(Type clrType, string typeAsCacheKeyString)
		{
			ComponentType = new ComponentType(clrType, typeAsCacheKeyString);
			CachedMethods = new HashSet<MethodInfo>();
			EnabledCache = true;
		}

		public bool EnabledCache { get; set; }
		public ComponentType ComponentType { get; }
		public ICollection<MethodInfo> CachedMethods { get; }
		public bool CachePerInstance { get; set; }
		public ICacheKey CacheKey { get; set; }
		public bool AllowDifferentArgumentsShareSameCacheKey { get; set; }
		public CacheAdapter CacheAdapter {get; private set;}

		public void CreateCacheAdapter(ICache defaultCache, ISet<ICache> allCaches)
		{
			if (_cache == null)
			{
				_cache = defaultCache;
			}

			allCaches.Add(_cache);
			CacheAdapter = new CacheAdapter(_cache);
		}

		public void SetCache(ICache cache)
		{
			_cache = cache;
		}
	}
}