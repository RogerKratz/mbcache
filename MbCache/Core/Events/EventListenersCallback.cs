using System.Collections.Generic;

namespace MbCache.Core.Events;

public class EventListenersCallback(IEnumerable<IEventListener> eventListeners)
{
	public void OnCacheRemoval(CachedItem cachedItem)
	{
		foreach (var eventHandler in eventListeners)
		{
			eventHandler.OnCacheRemoval(cachedItem);
		}
	}

	public void OnCacheMiss(CachedItem cachedItem)
	{
		foreach (var eventHandler in eventListeners)
		{
			eventHandler.OnCacheMiss(cachedItem);
		}
	}

	public void OnCacheHit(CachedItem cachedItem)
	{
		foreach (var eventListener in eventListeners)
		{
			eventListener.OnCacheHit(cachedItem);
		}
	}
}