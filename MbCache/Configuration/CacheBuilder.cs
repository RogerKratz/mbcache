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

        public IMbCacheFactory BuildFactory(ICacheFactory cacheFactory, IMbCacheKey keyBuilder)
        {
            return new MbCacheFactory(cacheFactory.Create(), keyBuilder, _cachedMethods);
        }


        public IFluentBuilder<T> ForClass<T>()
        {
            return createFluentBuilder<T>(null);
        }

        public IFluentBuilder<T> ForInterface<T>(object implementation)
        {
            return createFluentBuilder<T>(implementation);
        }

        private IFluentBuilder<T> createFluentBuilder<T>(object implementation)
        {
            var type = typeof (T);
            if (_cachedMethods.ContainsKey(type))
                throw new ArgumentException("Type " + type + " is already in CacheBuilder");
            var implAndDetails = new ImplementationAndMethods(implementation);
            _cachedMethods[type] = implAndDetails;
            var fluentBuilder = new FluentBuilder<T>(implAndDetails);
            return fluentBuilder;
        }
    }
}