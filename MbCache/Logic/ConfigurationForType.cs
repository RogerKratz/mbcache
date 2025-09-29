using System;
using System.Collections.Generic;
using MbCache.Configuration;

namespace MbCache.Logic;

public class ConfigurationForType(Type clrType, string typeAsCacheKeyString)
{
	public bool EnabledCache { get; set; } = true;
	public ComponentType ComponentType { get; } = new(clrType, typeAsCacheKeyString);
	public ICollection<CachedMethod> CachedMethods { get; } = new HashSet<CachedMethod>();
	public bool CachePerInstance { get; set; }
	public ICacheKey CacheKey { get; set; }
	public bool AllowDifferentArgumentsShareSameCacheKey { get; set; }
	public ICache Cache {get; set;}
}