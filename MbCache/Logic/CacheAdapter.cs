using System;
using System.Collections.Generic;
using MbCache.Core.Events;
using log4net;
using MbCache.Configuration;
using System.Linq;

namespace MbCache.Logic
{
	/// <summary>
	/// Adds logging, statistics and null replacement for <see cref="ICache"/>.
	/// </summary>
	[Serializable]
	public class CacheAdapter
	{
		private const string putMessage = "Adding cache value {0} for {1} (key: {2})";
		private const string deleteMessage = "Removing cache entries for {0} (key starts with: {1})";
		private const string cacheHitLogMessage = "Cache hit for {0} (key: {1})";
		private const string cacheMissLogMessage = "Cache miss for {0} (key: {1})";

		private static readonly ILog log = LogManager.GetLogger(typeof(CacheAdapter));
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
			var cacheValue = _cache.Get(eventInformation.CacheKey);
			if(cacheValue == null)
			{
				logCacheMiss(eventInformation);
			}
			else
			{
				if (cacheValue is nullValue)
				{
					cacheValue = null;
				}
				logCacheHit(eventInformation);
				callEventHandlersGet(eventInformation, cacheValue);
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
			if (log.IsDebugEnabled)
			{
				log.DebugFormat(putMessage, value, eventInformation.MethodName(), eventInformation.CacheKey);				
			}
			//creating new nullValue instance here - not really necessary with current aspnetcache impl
			//but gives a possibility for ICache implementations to use call backs
			_cache.Put(eventInformation.CacheKey, value ?? new nullValue());
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
			if (log.IsDebugEnabled)
			{
				log.DebugFormat(deleteMessage, eventInformation.MethodName(), eventInformation.CacheKey);					
			}
			_cache.Delete(eventInformation.CacheKey);
			callEventHandlersDelete(eventInformation);
		}

		private void callEventHandlersDelete(EventInformation eventInformation)
		{
			if (!_hasEventHandlers)
				return;
			foreach (var eventHandler in _eventHandlers)
			{
				eventHandler.OnDelete(eventInformation);
			}
		}

		private void logCacheHit(EventInformation eventInfo)
		{
			if (log.IsDebugEnabled)
			{
				log.DebugFormat(cacheHitLogMessage, eventInfo.MethodName(), eventInfo.CacheKey);				
			}
		}

		private void logCacheMiss(EventInformation eventInfo)
		{
			if (log.IsDebugEnabled)
			{
				log.DebugFormat(cacheMissLogMessage, eventInfo.MethodName(), eventInfo.CacheKey);				
			}
		}

		private class nullValue { }
	}
}