using System;
using System.Collections.Generic;
using System.Reflection;
using MbCache.Core;
using MbCache.CoreImpl;

namespace MbCache.Configuration
{
    public class CacheBuilder
    {
        private readonly ICollection<MethodInfo> _cachedMethods;

        public CacheBuilder()
        {
            _cachedMethods = new HashSet<MethodInfo>();
        }


        public IMbCacheFactory BuildFactory()
        {
            return new MbCacheFactory(_cachedMethods);
        }


        public void UseCacheFor<T>(Func<T, object> func)
        {
            _cachedMethods.Add(func.Method);
        }
    }
}