using System;
using System.Collections.Generic;

namespace MbCache.Core.Events
{
	[Serializable]
	public class EventListenersCallback
	{
		private readonly IEnumerable<IEventListener> _eventListeners;

		public EventListenersCallback(IEnumerable<IEventListener> eventListeners)
		{
			_eventListeners = eventListeners;
		}

		public void OnDelete(CachedItem cachedItem)
		{
			foreach (var eventHandler in _eventListeners)
			{
				eventHandler.OnDelete(cachedItem);
			}
		}

		public void OnPut(CachedItem cachedItem)
		{
			foreach (var eventHandler in _eventListeners)
			{
				eventHandler.OnPut(cachedItem);
			}
		}

		public void OnGet(CachedItem cachedItem, bool successful)
		{
			foreach (var eventHandler in _eventListeners)
			{
				eventHandler.OnGet(cachedItem, successful);
			}
		}
	}
}