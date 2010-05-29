using System;
using System.Collections.Generic;
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


        public IFluentBuilder<T> For<T>(Func<T> ctorDelegate)
        {
            var type = typeof (T);
            if(!type.IsInterface)
                throw new ArgumentException("You need to explicitly declare an interface for " + type);
            return createFluentBuilder(ctorDelegate);
        }

        private IFluentBuilder<T> createFluentBuilder<T>(Func<T> ctorDelegate)
        {
            var type = typeof (T);
            if (_cachedMethods.ContainsKey(type))
                throw new ArgumentException("Type " + type + " is already in CacheBuilder");
            var implAndDetails = new ImplementationAndMethods(ctorDelegate);
            _cachedMethods[type] = implAndDetails;
            var fluentBuilder = new FluentBuilder<T>(implAndDetails);
            return fluentBuilder;
        }
    }
}