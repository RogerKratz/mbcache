using System;
using System.Collections.Generic;
using MbCache.Core;
using MbCache.Core.Events;
using MbCache.Configuration;
using System.Linq;

namespace MbCache.Logic
{
	/// <summary>
	/// Calls <see cref="ICache"/>, registered <see cref="IEventListener"/>s handles <code>null</code> in cache.
	/// </summary>
	[Serializable]
	public class CacheAdapter
	{
		private readonly ICache _cache;
		private readonly IEnumerable<IEventListener> _eventHandlers;
		private readonly bool _hasEventHandlers; 

		public CacheAdapter(ICache cache, IEnumerable<IEventListener> eventHandlers)
		{
			_cache = cache;
			_eventHandlers = eventHandlers;
			_hasEventHandlers = _eventHandlers.Any();
		}

		public object Get(EventInformation eventInformation)
		{
			var cacheItem = _cache.Get(eventInformation.CacheKey);
			object cacheValue = null;
			if(cacheItem!=null)
			{
				cacheValue = cacheItem.CachedValue;
			}
			callEventHandlersGet(eventInformation, cacheValue);
			if (cacheValue is NullValue)
			{
				cacheValue = null;
			}
			return cacheValue;
		}

		private void callEventHandlersGet(EventInformation eventInformation, object cachedValue)
		{
			if (!_hasEventHandlers)
				return;
			foreach (var eventHandler in _eventHandlers)
			{
				eventHandler.OnGet(eventInformation, cachedValue);
			}
		}

		public void Put(EventInformation eventInformation, object value)
		{
			var cachedValue = value ?? new NullValue();
			_cache.Put(eventInformation.CacheKey, new CachedItem(eventInformation, cachedValue));
			callEventHandlersPut(eventInformation, value);
		}

		private void callEventHandlersPut(EventInformation eventInformation, object cachedValue)
		{
			if (!_hasEventHandlers)
				return;
			foreach (var eventHandler in _eventHandlers)
			{
				eventHandler.OnPut(eventInformation, cachedValue);
			}
		}

		public void Delete(EventInformation eventInformation)
		{
			if (eventInformation.CacheKey == null) 
				return;
			_cache.Delete(eventInformation.CacheKey);
			//callEventHandlersDelete(eventInformation);
		}

		public void callEventHandlersDelete(EventInformation eventInformation)
		{
			if (!_hasEventHandlers)
				return;
			foreach (var eventHandler in _eventHandlers)
			{
				eventHandler.OnDelete(eventInformation);
			}
		}
	}
}