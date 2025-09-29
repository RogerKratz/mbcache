using System.Collections.Generic;
using MbCache.Core;
using MbCache.Core.Events;

namespace MbCacheTest;

public class EventListenerForTest : IEventListener
{
	public IList<CachedItem> CacheHits { get; } = new List<CachedItem>();
	public void OnCacheHit(CachedItem cachedItem)
	{
		CacheHits.Add(cachedItem);
	}

	public IList<CachedItem> CacheRemovals { get; } = new List<CachedItem>();
	public void OnCacheRemoval(CachedItem cachedItem)
	{
		CacheRemovals.Add(cachedItem);
	}

	public IList<CachedItem> CacheMisses { get; } = new List<CachedItem>();
	public void OnCacheMiss(CachedItem cachedItem)
	{
		CacheMisses.Add(cachedItem);
	}
}