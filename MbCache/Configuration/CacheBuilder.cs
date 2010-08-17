using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using MbCache.Core;
using MbCache.Logic;

namespace MbCache.Configuration
{
    public class CacheBuilder
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CacheBuilder));
        private readonly string _proxyFactoryClass;
        private readonly ICache _cache;
        private readonly IMbCacheKey _keyBuilder;
        private readonly IDictionary<Type, ImplementationAndMethods> _cachedMethods;
        private readonly ICollection<ImplementationAndMethods> _details;
        private readonly IProxyFactory _proxyFactory;
        private readonly ProxyValidator _proxyValidator;


        public CacheBuilder(string proxyFactoryClass,
                                            ICacheFactory cacheFactory,
                                            IMbCacheKey keyBuilder)
        {
            _proxyFactoryClass = proxyFactoryClass;
            _cache = cacheFactory.Create();
            _keyBuilder = keyBuilder;
            _cachedMethods = new Dictionary<Type, ImplementationAndMethods>();
            _details = new List<ImplementationAndMethods>();
            _proxyFactory = createProxyFactory();
            _proxyValidator = new ProxyValidator(ProxyFactory);
        }

        public IProxyFactory ProxyFactory
        {
            get { return _proxyFactory; }
        }


        public IMbCacheFactory BuildFactory()
        {
            checkAllImplementationAndMethodsAreOk();
            return new MbCacheFactory(ProxyFactory, _cache, _keyBuilder, _cachedMethods);
        }

        private IProxyFactory createProxyFactory()
        {
            var proxyFactoryType = Type.GetType(_proxyFactoryClass, true, true);
            var proxyFactory = (IProxyFactory)Activator.CreateInstance(proxyFactoryType, _cache, _keyBuilder);
            log.Debug("Successfully created type " + proxyFactory + " as IProxyFactory.");
            return proxyFactory;
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
            Type concreteType = typeof(T);
            _proxyValidator.Validate(concreteType);
            var details = new ImplementationAndMethods(concreteType);
            _details.Add(details);
            var fluentBuilder = new FluentBuilder<T>(_cachedMethods, details);
            return fluentBuilder;
        }
    }
}