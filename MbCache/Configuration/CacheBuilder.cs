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


        public void UseCacheForClass<T>(Expression<Func<T, object>> expression)
        {
            Type type = typeof (T);
            addMethodToList(type, type, expression);              
        }

        public void UseCacheForInterface<TInterface, TImplementation>(Expression<Func<TInterface, object>> expression)
        {
            Type type = typeof (TInterface);
            addMethodToList(type, typeof(TImplementation), expression);
        }

        private void addMethodToList<TInterface>(Type type, Type implType, Expression<Func<TInterface, object>> expression)
        {
            if (!_cachedMethods.ContainsKey(type))
                _cachedMethods[type] = new ImplementationAndMethods(implType);
            _cachedMethods[type].Methods.Add(ExpressionHelper.MemberName(expression.Body));
        }
    }
}