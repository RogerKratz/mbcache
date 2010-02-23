using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using MbCache.Logic;

namespace MbCache.Caches
{
    public class AspNetCache : ICache
    {
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
                if(keyString.StartsWith(keyStartingWith))
                    keyList.Add(keyString);
            }
            foreach (var key in keyList)
            {
                cache.Remove(key);
            }
        }
    }
}