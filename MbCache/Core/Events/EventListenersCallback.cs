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

		public void callEventHandlersDelete(EventInformation eventInformation)
		{
			foreach (var eventHandler in _eventListeners)
			{
				eventHandler.OnDelete(eventInformation);
			}
		}

		public void callEventHandlersPut(CachedItem cachedItem)
		{
			var cachedItemToUse = cachedItem;
			if (cachedItem.CachedValue is NullValue)
			{
				cachedItemToUse = new CachedItem(cachedItem.EventInformation, null);
			}
			foreach (var eventHandler in _eventListeners)
			{
				eventHandler.OnPut(cachedItemToUse);
			}
		}

		public void callEventHandlersGet(CachedItem cachedItem)
		{
			foreach (var eventHandler in _eventListeners)
			{
				eventHandler.OnGet(cachedItem);
			}
		}
	}
}