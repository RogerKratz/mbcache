using System.Collections.Generic;
using Castle.Core.Interceptor;
using MbCache.Configuration;

namespace MbCache.Logic
{
    public class CacheInterceptor : IInterceptor, ICacheableSignatures
    {
        private readonly ICache _cache;
        private readonly IMbCacheRegion _cacheRegion;

        public CacheInterceptor(ICache cache, IMbCacheRegion _cacheRegion, IEnumerable<string> methodNames)
        {
            _cache = cache;
            this._cacheRegion = _cacheRegion;
            MethodNames = methodNames;
        }

        public IEnumerable<string> MethodNames { get; private set; }

        public void Intercept(IInvocation invocation)
        {
            var key = _cacheRegion.Region(invocation.Method.Name);
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

        private bool matchName(string method)
        {
            foreach (var name in MethodNames)
            {
                if(name.Equals(method))
                    return true;
            }
            return false;
        }
    }
}
