using System;
using log4net;
using MbCache.Core;

namespace MbCache.Logic
{
    public class LogAndStatisticCacheDecorator : ICache, IStatistics
    {
        private ILog log;
        private readonly ICache _cache;
        private long _cacheHits;
        private long _cacheMisses;

        public LogAndStatisticCacheDecorator(ICache cache)
        {
            _cache = cache;
            log = LogManager.GetLogger(cache.GetType());
        }

        public object Get(string key)
        {
            log.Debug("Trying to find cache entry <" + key + ">");
            var ret = _cache.Get(key);
            if(ret == null)
            {
                log.Debug("Cache miss for <" + key + ">");
                _cacheMisses++;
            }
            else
            {
                log.Debug("Cache hit for <" + key + ">");
                _cacheHits++;
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
            _cacheHits = 0;
            _cacheMisses = 0;
        }

        public long CacheHits
        {
            get { return _cacheHits; }
        }

        public long CacheMisses
        {
            get { return _cacheMisses; }
        }
    }
}