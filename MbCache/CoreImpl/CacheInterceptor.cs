using System.Collections.Generic;
using Castle.Core.Interceptor;

namespace MbCache.Logic
{
    public class CacheInterceptor : IInterceptor, ICacheableSignatures
    {
        private readonly ICache _cache;

        public CacheInterceptor(ICache cache, IEnumerable<string> methodNames)
        {
            _cache = cache;
            MethodNames = methodNames;
        }

        public IEnumerable<string> MethodNames { get; private set; }

        public void Intercept(IInvocation invocation)
        {
            var methodName = invocation.Method.Name;
            if(matchName(methodName))
            {
                object cachedValue = _cache.Get(methodName);
                if(cachedValue!=null)
                {
                    invocation.ReturnValue = cachedValue;
                }
                else
                {
                    invocation.Proceed();
                    _cache.Put(methodName, invocation.ReturnValue);
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
