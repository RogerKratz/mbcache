using System.Collections.Generic;

namespace MbCache.Core.Events;

public class EventListenersCallback
{
	private readonly IEnumerable<IEventListener> _eventListeners;

	public EventListenersCallback(IEnumerable<IEventListener> eventListeners)
	{
		_eventListeners = eventListeners;
	}

	public void OnCacheRemoval(CachedItem cachedItem)
	{
		foreach (var eventHandler in _eventListeners)
		{
			eventHandler.OnCacheRemoval(cachedItem);
		}
	}

	public void OnCacheMiss(CachedItem cachedItem)
	{
		foreach (var eventHandler in _eventListeners)
		{
			eventHandler.OnCacheMiss(cachedItem);
		}
	}

	public void OnCacheHit(CachedItem cachedItem)
	{
		foreach (var eventListener in _eventListeners)
		{
			eventListener.OnCacheHit(cachedItem);
		}
	}
}