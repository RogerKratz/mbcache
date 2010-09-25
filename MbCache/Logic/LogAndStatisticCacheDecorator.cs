using log4net;
using MbCache.Core;

namespace MbCache.Logic
{
    public class LogAndStatisticCacheDecorator : ICache, IStatistics
    {
        private ILog log;
        private readonly ICache _cache;

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
                log.Debug("Cache miss for <" + key + ">");
                CacheMisses++;
            }
            else
            {
                log.Debug("Cache hit for <" + key + ">");
                CacheHits++;
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
            CacheHits = 0;
            CacheMisses = 0;
        }

    }
}