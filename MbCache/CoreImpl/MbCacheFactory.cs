using System;
using System.Collections.Generic;
using System.Reflection;
using MbCache.Core;

namespace MbCache.CoreImpl
{
    public class MbCacheFactory : IMbCacheFactory
    {
        private readonly IEnumerable<MethodInfo> _methods;

        public MbCacheFactory(IEnumerable<MethodInfo> methods)
        {
            _methods = methods;
        }

        public T Get<T>()
        {
            T ret = createInstance<T>();
            return ret;
        }

        private static T createInstance<T>()
        {
            return (T)Activator.CreateInstance(typeof(T), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
        }
    }
}