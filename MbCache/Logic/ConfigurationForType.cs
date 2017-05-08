using System;
using System.Collections.Generic;
using System.Reflection;
using MbCache.Configuration;

namespace MbCache.Logic
{
	[Serializable]
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
	}
}