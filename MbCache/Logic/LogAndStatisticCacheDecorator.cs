using log4net;
using MbCache.Configuration;
using MbCache.Core;

namespace MbCache.Logic
{
    public class LogAndStatisticCacheDecorator : ICache, IStatistics
    {
        private ILog log;
        private readonly ICache _cache;
        private readonly object cacheMissLocker = new object();
        private readonly object cacheHitLocker = new object();

        public LogAndStatisticCacheDecorator(ICache cache)
        {
            _cache = cache;
            log = LogManager.GetLogger(cache.GetType());
        }

        public long CacheHits { get; private set; }
        public long CacheMisses { get; private set; }

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
            _cache.Delete(keyStartingWith);
        }

        public void Clear()
        {
            lock(cacheMissLocker)
            {
                CacheMisses = 0;                
            }
            lock (cacheHitLocker)
            {
                CacheHits = 0;
            }
        }

        private void cacheHit(string key)
        {
            log.Debug("Cache hit for <" + key + ">");
            lock (cacheHitLocker)
            {
                CacheHits++;
            }
        }

        private void cacheMiss(string key)
        {
            log.Debug("Cache miss for <" + key + ">");
            lock (cacheMissLocker)
            {
                CacheMisses++;
            }
        }
    }
}