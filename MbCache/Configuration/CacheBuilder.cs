using System;
using System.Collections.Generic;
using System.Text;
using MbCache.Core;
using MbCache.Logic;

namespace MbCache.Configuration
{
    public class CacheBuilder
    {
        private readonly string _proxyFactoryClass;
        private readonly ICacheFactory _cacheFactory;
        private readonly IMbCacheKey _keyBuilder;
        private readonly IDictionary<Type, ImplementationAndMethods> _cachedMethods;
        private readonly ICollection<ImplementationAndMethods> _details;


        public CacheBuilder(string proxyFactoryClass,
                                            ICacheFactory cacheFactory,
                                            IMbCacheKey keyBuilder)
        {
            _proxyFactoryClass = proxyFactoryClass;
            _cacheFactory = cacheFactory;
            _keyBuilder = keyBuilder;
            _cachedMethods = new Dictionary<Type, ImplementationAndMethods>();
            _details = new List<ImplementationAndMethods>();
        }

        public IMbCacheFactory BuildFactory()
        {
            checkAllImplementationAndMethodsAreOk();
            return new MbCacheFactory(_proxyFactoryClass, _cacheFactory.Create(), _keyBuilder, _cachedMethods);
        }

        private void checkAllImplementationAndMethodsAreOk()
        {
            if(_details.Count > _cachedMethods.Count)
            {
                var excText = new StringBuilder();
                excText.AppendLine("Missing return type (.As) for");
                var fullyDefined = _cachedMethods.Values;
                foreach (var declared in _details)
                {
                    if (!fullyDefined.Contains(declared))
                        excText.AppendLine(declared.ConcreteType.ToString());
                }   
                throw new InvalidOperationException(excText.ToString());
            }
        }


        public IFluentBuilder<T> For<T>()
        {
            var details = new ImplementationAndMethods(typeof (T));
            _details.Add(details);
            var fluentBuilder = new FluentBuilder<T>(_cachedMethods, details);
            return fluentBuilder;
        }
    }
}