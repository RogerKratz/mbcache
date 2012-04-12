using System.Threading;
using log4net;
using MbCache.Configuration;
using MbCache.Core;

namespace MbCache.Logic
{
	public class LogAndStatisticCacheDecorator : ICache, IStatistics
	{
		private static readonly ILockObjectGenerator noSharedLocks = new noSharedLocksGenerator();
		private readonly ILog log;
		private readonly ICache _cache;
		private long _cacheHits;
		private long _physicalCacheMisses;

		public LogAndStatisticCacheDecorator(ICache cache)
		{
			_cache = cache;
			log = LogManager.GetLogger(cache.GetType());
			LockObjectGenerator = _cache.LockObjectGenerator ?? noSharedLocks;
		}

		public ILockObjectGenerator LockObjectGenerator { get; private set; }

		public long CacheHits
		{
			get { return _cacheHits; }
		}

		public long CacheMisses
		{
			get
			{
				return _physicalCacheMisses / 2;
			}
		}

		public long PhysicalCacheMisses
		{
			get { return _physicalCacheMisses; }
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

		public void Clear()
		{
			_physicalCacheMisses = 0;                
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
			Interlocked.Increment(ref _physicalCacheMisses);
		}

		//fix this - should probably be a generator that does not produce lock(object) at all...
		private class noSharedLocksGenerator : ILockObjectGenerator
		{
			public object GetFor(string key)
			{
				return new object();
			}
		}
	}
}