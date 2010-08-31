using System;
using Castle.Core.Interceptor;
using log4net;
using MbCache.Logic;

namespace MbCache.ProxyImpl.Castle
{
    public class CacheInterceptor : IInterceptor
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (CacheInterceptor));

        private readonly ICache _cache;
        private readonly IMbCacheKey _cacheKey;
        private readonly Type _type;

        public CacheInterceptor(ICache cache, IMbCacheKey cacheKey, Type type)
        {
            _cache = cache;
            _cacheKey = cacheKey;
            _type = type;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.Method;
            var typeAndMethodName = "<" + _type + "." + method.Name + "()>";
            log.Debug("Entering " + typeAndMethodName);

            var proxy = (ICachingComponent) invocation.Proxy;
            var arguments = invocation.Arguments;

            var key = string.Concat(_cacheKey.CacheKey(_type, method),
                                    _cacheKey.AddForComponent(proxy),
                                    _cacheKey.AddForParameterValues(_type, method, arguments));

            log.Debug("Trying to find cache entry <" + key +">");
            var cachedValue = _cache.Get(key);
            if (cachedValue != null)
            {
                log.Debug("Cache hit for <" + key + ">");
                invocation.ReturnValue = cachedValue;
            }
            else
            {
                log.Debug("Cache miss for <" + key + ">");
                invocation.Proceed();
                log.Debug("Put in cache entry <" + key + ">");
                _cache.Put(key, invocation.ReturnValue);
            }

            log.Debug("Leaving " + typeAndMethodName);
        }
    }
}
