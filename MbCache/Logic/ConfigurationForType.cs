using System;
using System.Collections.Generic;
using System.Reflection;
using MbCache.Configuration;

namespace MbCache.Logic
{
	public class ConfigurationForType
	{
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
		public ICache Cache {get; private set;}

		public void CreateCacheAdapter(ICache defaultCache, ISet<ICache> allCaches)
		{
			if (Cache == null)
			{
				Cache = defaultCache;
			}

			allCaches.Add(Cache);
		}

		public void SetCache(ICache cache)
		{
			Cache = cache;
		}
	}
}