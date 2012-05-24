using System.Threading;
using log4net;
using MbCache.Configuration;
using MbCache.Core;

namespace MbCache.Logic
{
	/// <summary>
	/// Adds logging, statistics and null replacement for <see cref="ICache"/>.
	/// </summary>
	public class CacheDecorator : ICache, IStatistics
	{
		private readonly ILog _log;
		private readonly ICache _cache;
		private long _cacheHits;
		private long _physicalCacheMisses;

		public CacheDecorator(ICache cache)
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
				return _physicalCacheMisses;
			}
		}

		public object Get(string key)
		{
			_log.Debug("Trying to find cache entry <" + key + ">");
			var cacheValue = _cache.Get(key);
			if(cacheValue == null)
			{
				cacheMiss(key);
			}
			else
			{
				cacheHit(key);
			}
			return cacheValue is nullValue ? null : cacheValue;
		}

		public void Put(string key, object value)
		{
			_log.Debug("Put in cache entry <" + key + ">");
			//creating new nullValue instance here - not really necessary with current aspnetcache impl
			//but gives a possibility for ICache implementations to use call backs
			_cache.Put(key, value ?? new nullValue());
		}

		public void Delete(string keyStartingWith)
		{
			if (keyStartingWith != null)
			{
				_log.DebugFormat("Removing cache entries starting with {0}", keyStartingWith);
				_cache.Delete(keyStartingWith);				
			}
		}

		public void Clear()
		{
			_physicalCacheMisses = 0;                
			_cacheHits = 0;
		}

		private void cacheHit(string key)
		{
			_log.Debug("Cache hit for <" + key + ">");
			Interlocked.Increment(ref _cacheHits);
		}

		private void cacheMiss(string key)
		{
			_log.Debug("Cache miss for <" + key + ">");
			Interlocked.Increment(ref _physicalCacheMisses);
		}

		private class nullValue { }
	}
}