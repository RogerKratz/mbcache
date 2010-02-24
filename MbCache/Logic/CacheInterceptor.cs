using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.Core.Interceptor;
using MbCache.Core;

namespace MbCache.Logic
{
    public class CacheInterceptor : IInterceptor, ICacheableSignatures
    {
        private readonly ICache _cache;
        private readonly IMbCacheRegion _cacheRegion;

        public CacheInterceptor(ICache cache, IMbCacheRegion cacheRegion, IEnumerable<MethodInfo> keys)
        {
            _cache = cache;
            _cacheRegion = cacheRegion;
            Keys = keys;
        }

        public IEnumerable<MethodInfo> Keys { get; private set; }

        public void Intercept(IInvocation invocation)
        {
            if(methodIsCached(invocation.Method))
            {
                var typeAndMethodKey = _cacheRegion.Region(invocation.TargetType, invocation.Method);
                var key = typeAndMethodKey + _cacheRegion.AdditionalRegionsForParameterValues(invocation.Arguments);
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

        private bool methodIsCached(MethodInfo key)
        {
            foreach (var methodInfo in Keys)
            {
                if (methodInfo.Equals(key))
                    return true;
            }
            return false;
        }
    }
}
