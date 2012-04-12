using System.Threading;
using log4net;
using MbCache.Configuration;
using MbCache.Core;

namespace MbCache.Logic
{
	public class LogAndStatisticCacheDecorator : ICache, IStatistics
	{
		private readonly ILog log;
		private readonly ICache _cache;
		private long _cacheHits;
		private long _cacheMisses;

		public LogAndStatisticCacheDecorator(ICache cache)
		{
			_cache = cache;
			log = LogManager.GetLogger(cache.GetType());
		}

		public long CacheHits
		{
			get { return _cacheHits; }
		}

		public long CacheMisses
		{
			get { return _cacheMisses; }
		}

		public object Get(string key)
		{
			log.Debug("Trying to find cache entry <" + key + ">");
			var ret = _cache.Get(key);
			if(ret == null)
			{
				cacheMiss(key);
			}
			else
			{
				cacheHit(key);
			}
			return ret;
		}

		public void Put(string key, object value)
		{
			log.Debug("Put in cache entry <" + key + ">");
			_cache.Put(key, value);
		}

		public void Delete(string keyStartingWith)
		{
			if (keyStartingWith != null)
			{
				log.DebugFormat("Removing cache entries starting with {0}", keyStartingWith);
				_cache.Delete(keyStartingWith);				
			}
		}

		public ILockObjectGenerator LockObjectGenerator
		{
			get { return _cache.LockObjectGenerator; }
		}

		public void Clear()
		{
			_cacheMisses = 0;                
			_cacheHits = 0;
		}

		private void cacheHit(string key)
		{
			log.Debug("Cache hit for <" + key + ">");
			Interlocked.Increment(ref _cacheHits);
		}

		private void cacheMiss(string key)
		{
			log.Debug("Cache miss for <" + key + ">");
			Interlocked.Increment(ref _cacheMisses);
		}
	}
}