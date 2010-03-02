using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.Core.Interceptor;
using log4net;

namespace MbCache.Logic
{
    public class CacheInterceptor : IInterceptor
    {
        private static ILog log = LogManager.GetLogger(typeof (CacheInterceptor));

        private readonly ICache _cache;
        private readonly IMbCacheRegion _cacheRegion;
        private readonly Type _orgType;

        public CacheInterceptor(ICache cache, IMbCacheRegion cacheRegion, Type orgType)
        {
            _cache = cache;
            _cacheRegion = cacheRegion;
            _orgType = orgType;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.Method;
            var typeAndMethodName = "<" + _orgType + "." + method.Name + "()>";
            log.Debug("Entering " + typeAndMethodName);

            var typeAndMethodKey = _cacheRegion.Region(_orgType, method);
            var key = typeAndMethodKey + _cacheRegion.AdditionalRegionsForParameterValues(_orgType, method, invocation.Arguments);
            log.Debug("Trying to find cache entry <" + key +">");
            var cachedValue = _cache.Get(key);
            if (cachedValue != null)
            {
                log.Debug("Cache hit for " + key);
                invocation.ReturnValue = cachedValue;
            }
            else
            {
                log.Debug("Cache miss for " + key);
                invocation.Proceed();
                log.Debug("Put in cache entry <" + key + ">");
                _cache.Put(key, invocation.ReturnValue);
            }

            log.Debug("Leaving " + typeAndMethodName);
        }
    }
}
