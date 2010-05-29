using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using log4net;
using MbCache.Logic;

namespace MbCache.DefaultImpl
{
    public class AspNetCache : ICache
    {
        private static ILog log = LogManager.GetLogger(typeof(AspNetCache));
        private readonly int _timeoutMinutes;
        private readonly Cache cache;

        public AspNetCache(int timeoutMinutes)
        {
            _timeoutMinutes = timeoutMinutes;
            cache = HttpRuntime.Cache;
        }

        public object Get(string key)
        {
            return cache.Get(key);
        }

        public void Put(string key, object value)
        {
            cache.Insert(key,
                         value,
                         null,
                         DateTime.Now.Add(TimeSpan.FromMinutes(_timeoutMinutes)),
                         Cache.NoSlidingExpiration,
                         CacheItemPriority.Default,
                         null);
        }

        public void Delete(string keyStartingWith)
        {
            var keyList = new List<string>();
            var cacheEnum = cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                var keyString = cacheEnum.Key.ToString();
                if(keyString.StartsWith(keyStartingWith, StringComparison.InvariantCulture))
                    keyList.Add(keyString);
            }
            foreach (var key in keyList)
            {
                cache.Remove(key);
            }
        }
    }
}