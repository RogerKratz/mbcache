using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MbCache.Core;
using MbCache.Logic;

namespace MbCache.Configuration
{
    public class CacheBuilder
    {
        private readonly IDictionary<Type, ImplementationAndMethods> _cachedMethods;

        public CacheBuilder()
        {
            _cachedMethods = new Dictionary<Type, ImplementationAndMethods>();
        }


        public IMbCacheFactory BuildFactory(ICacheFactory cacheFactory)
        {
            return new MbCacheFactory(cacheFactory.Create(), new DefaultMbCacheRegion(), _cachedMethods);
        }


        public void UseCacheForClass<T>(Expression<Func<T, object>> expression, params object[] ctorParameters)
        {
            addMethodToList(typeof(T), null, expression, ctorParameters);              
        }

        public void UseCacheForInterface<T>(object impl, params Expression<Func<T, object>>[] expressions)
        {
            Type proxyType = typeof (T);
            foreach (Expression<Func<T, object>> expression in expressions)
            {
                addMethodToList(proxyType, impl, expression, new object[0]);
            }
        }

        private void addMethodToList<T>(Type proxyType,
                                                object implementation,
                                                Expression<Func<T, object>> expression,
                                                object[] ctorParameters)
        {
            if (!_cachedMethods.ContainsKey(proxyType))
                _cachedMethods[proxyType] = new ImplementationAndMethods(ctorParameters, implementation);
            _cachedMethods[proxyType].Methods.Add(ExpressionHelper.MemberName(expression.Body));
        }
    }
}