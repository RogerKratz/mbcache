using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MbCache.Core;
using MbCache.Logic;

namespace MbCache.Configuration
{
	public class CacheBuilder
	{
		private readonly IDictionary<Type, ImplementationAndMethods> _cachedMethods;
		private readonly ICollection<ImplementationAndMethods> _details;
		private readonly IProxyFactory _proxyFactory;
		private readonly ICache _cache;
		private readonly IMbCacheKey _keyBuilder;
		private readonly ILockObjectGenerator _lockObjectGenerator;

		public CacheBuilder(IProxyFactory proxyFactory,
										ICache cache,
										IMbCacheKey keyBuilder)
		{
			_cachedMethods = new Dictionary<Type, ImplementationAndMethods>();
			_details = new List<ImplementationAndMethods>();
			_proxyFactory = proxyFactory;
			_cache = cache;
			_keyBuilder = keyBuilder;
		}

		public CacheBuilder(IProxyFactory proxyFactory,
								ICache cache,
								IMbCacheKey keyBuilder,
								ILockObjectGenerator lockObjectGenerator)
		{
			_cachedMethods = new Dictionary<Type, ImplementationAndMethods>();
			_details = new List<ImplementationAndMethods>();
			_proxyFactory = proxyFactory;
			_cache = cache;
			_keyBuilder = keyBuilder;
			_lockObjectGenerator = lockObjectGenerator;
		}

		public IMbCacheFactory BuildFactory()
		{
			checkAllImplementationAndMethodsAreOk();
			return new MbCacheFactory(_proxyFactory, _cache, _keyBuilder, _lockObjectGenerator, _cachedMethods);
		}

		public IFluentBuilder<T> For<T>()
		{
			var concreteType = typeof(T);
			var details = new ImplementationAndMethods(concreteType);
			_details.Add(details);
			var fluentBuilder = new FluentBuilder<T>(_cachedMethods, details, new ProxyValidator(_proxyFactory));
			return fluentBuilder;
		}

		private void checkAllImplementationAndMethodsAreOk()
		{
			if (_details.Count > _cachedMethods.Count)
			{
				var excText = new StringBuilder();
				excText.AppendLine("Missing return type (.As) for");
				var fullyDefined = _cachedMethods.Values;
				foreach (var undefined in _details.Except(fullyDefined))
				{
					excText.AppendLine(undefined.ConcreteType.ToString());
				}
				throw new InvalidOperationException(excText.ToString());
			}
		}
	}
}