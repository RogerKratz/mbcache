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
	public class CacheAdapter
	{
		private const string putMessage = "Adding cache entry for <{0}>";
		private const string getMessage = "Trying to find cache entry <{0}>";
		private const string deleteMessage = "Removing cache entries starting with {0}";
		private const string cacheHitLogMessage = "Cache hit for <{0}>";
		private const string cacheMissLogMessage = "Cache miss for <{0}>";

		private readonly ILog _log;
		private readonly ICache _cache;
		private readonly IEnumerable<IEventListener> _eventHandlers;
		private readonly bool _hasEventHandlers; //just for perf reasons - minimize enumerators

		public CacheAdapter(ICache cache, IEnumerable<IEventListener> eventHandlers)
		{
			_cache = cache;
			_eventHandlers = eventHandlers;
			_hasEventHandlers = _eventHandlers.Any();
			_log = LogManager.GetLogger(cache.GetType());
		}

		public object Get(EventInformation eventInformation)
		{
			if (_log.IsDebugEnabled)
			{
				_log.DebugFormat(getMessage, eventInformation.CacheKey);				
			}
			var cacheValue = _cache.Get(eventInformation.CacheKey);
			if(cacheValue == null)
			{
				logCacheMiss(eventInformation.CacheKey);
			}
			else
			{
				if (cacheValue is nullValue)
				{
					cacheValue = null;
				}
				logCacheHit(eventInformation.CacheKey);
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
			if (_log.IsDebugEnabled)
			{
				_log.DebugFormat(putMessage, eventInformation.CacheKey);				
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
			if (_log.IsDebugEnabled)
			{
				_log.DebugFormat(deleteMessage, eventInformation.CacheKey);					
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

		private void logCacheHit(string key)
		{
			if (_log.IsDebugEnabled)
			{
				_log.DebugFormat(cacheHitLogMessage, key);				
			}
		}

		private void logCacheMiss(string key)
		{
			if (_log.IsDebugEnabled)
			{
				_log.DebugFormat(cacheMissLogMessage, key);				
			}
		}

		private class nullValue { }
	}
}