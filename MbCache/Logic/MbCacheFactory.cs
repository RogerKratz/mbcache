using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using log4net;
using MbCache.Configuration;
using MbCache.Core;

namespace MbCache.Logic
{
    public class MbCacheFactory : IMbCacheFactory
    {
        private readonly IProxyFactory _proxyFactory;
        private readonly ICache _cache;
        private readonly IMbCacheKey _cacheKey;
        private readonly IDictionary<Type, ImplementationAndMethods> _methods;

        public MbCacheFactory(IProxyFactory proxyFactory,
                            ICache cache, 
                            IMbCacheKey cacheKey,
                            IDictionary<Type, ImplementationAndMethods> methods)
        {
            _cache = cache;
            _cacheKey = cacheKey;
            _methods = methods;

            _proxyFactory = proxyFactory;
        }

        public T Create<T>(params object[] parameters)
        {
            return _proxyFactory.CreateProxy<T>(_methods[typeof(T)], parameters);
        }

        public void Invalidate<T>()
        {
            _cache.Delete(_cacheKey.Key(typeof(T)));
        }

        public void Invalidate<T>(Expression<Func<T, object>> method)
        {
            var memberInfo = ExpressionHelper.MemberName(method.Body);
            var type = typeof (T); 
            _cache.Delete(_cacheKey.Key(type, memberInfo));
        }
    }
}