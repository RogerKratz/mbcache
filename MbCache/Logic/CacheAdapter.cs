using System.Threading;
using MbCache.Core.Events;
using log4net;
using MbCache.Configuration;
using MbCache.Core;

namespace MbCache.Logic
{
	/// <summary>
	/// Adds logging, statistics and null replacement for <see cref="ICache"/>.
	/// </summary>
	public class CacheAdapter : IStatistics
	{
		private const string putMessage = "Adding cache entry for <{0}>";
		private const string getMessage = "Trying to find cache entry <{0}>";
		private const string deleteMessage = "Removing cache entries starting with {0}";
		private const string cacheHitLogMessage = "Cache hit for <{0}>";
		private const string cacheMissLogMessage = "Cache miss for <{0}>";

		private readonly ILog _log;
		private readonly ICache _cache;
		private long _cacheHits;
		private long _cacheMisses;

		public CacheAdapter(ICache cache)
		{
			_cache = cache;
			_log = LogManager.GetLogger(cache.GetType());
		}

		public long CacheHits
		{
			get { return _cacheHits; }
		}

		public long CacheMisses
		{
			get
			{
				return _cacheMisses;
			}
		}

		public object Get(GetInfo getInfo)
		{
			if (_log.IsDebugEnabled)
			{
				_log.DebugFormat(getMessage, getInfo.CacheKey);				
			}
			var cacheValue = _cache.Get(getInfo.CacheKey);
			if(cacheValue == null)
			{
				cacheMiss(getInfo.CacheKey);
			}
			else
			{
				cacheHit(getInfo.CacheKey);
			}
			return cacheValue is nullValue ? null : cacheValue;
		}

		public void Put(PutInfo putInfo, object value)
		{
			if (_log.IsDebugEnabled)
			{
				_log.DebugFormat(putMessage, putInfo.CacheKey);				
			}
			//creating new nullValue instance here - not really necessary with current aspnetcache impl
			//but gives a possibility for ICache implementations to use call backs
			_cache.Put(putInfo.CacheKey, value ?? new nullValue());
		}

		public void Delete(DeleteInfo deleteInfo)
		{
			if (deleteInfo.CacheKeyStartsWith == null) 
				return;
			if (_log.IsDebugEnabled)
			{
				_log.DebugFormat(deleteMessage, deleteInfo.CacheKeyStartsWith);					
			}
			_cache.Delete(deleteInfo.CacheKeyStartsWith);
		}

		public void Clear()
		{
			_cacheMisses = 0;                
			_cacheHits = 0;
		}

		private void cacheHit(string key)
		{
			if (_log.IsDebugEnabled)
			{
				_log.DebugFormat(cacheHitLogMessage, key);				
			}
			Interlocked.Increment(ref _cacheHits);
		}

		private void cacheMiss(string key)
		{
			if (_log.IsDebugEnabled)
			{
				_log.DebugFormat(cacheMissLogMessage, key);				
			}
			Interlocked.Increment(ref _cacheMisses);
		}

		private class nullValue { }
	}
}