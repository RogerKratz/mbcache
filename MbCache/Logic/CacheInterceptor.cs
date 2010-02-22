using System;
using System.Collections.Generic;
using Castle.Core.Interceptor;

namespace MbCache.Logic
{
    public class CacheInterceptor : IInterceptor, ICacheableSignatures
    {
        private readonly ICache _cache;
        private readonly IMbCacheRegion _cacheRegion;

        public CacheInterceptor(ICache cache, IMbCacheRegion cacheRegion, IEnumerable<string> keys)
        {
            _cache = cache;
            _cacheRegion = cacheRegion;
            Keys = keys;
        }

        public IEnumerable<string> Keys { get; private set; }

        public void Intercept(IInvocation invocation)
        {
            Type targetType = invocation.TargetType;
            var key = _cacheRegion.Region(targetType, invocation.Method.Name);
            if(matchName(key))
            {
                object cachedValue = _cache.Get(key);
                if(cachedValue!=null)
                {
                    invocation.ReturnValue = cachedValue;
                }
                else
                {
                    invocation.Proceed();
                    _cache.Put(key, invocation.ReturnValue);
                }
            }
            else
            {
                invocation.Proceed();                
            }
        }

        private bool matchName(string key)
        {
            foreach (var name in Keys)
            {
                if(name.Equals(key))
                    return true;
            }
            return false;
        }
    }
}
