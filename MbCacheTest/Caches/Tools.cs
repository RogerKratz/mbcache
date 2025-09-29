using System.Linq;
using System.Runtime.Caching;

namespace MbCacheTest.Caches;

public static class Tools
{
	public static void ClearMemoryCache()
	{
		foreach (var cacheKey in MemoryCache.Default.Select(kvp => kvp.Key).ToList())
		{
			MemoryCache.Default.Remove(cacheKey);
		}
	}
}