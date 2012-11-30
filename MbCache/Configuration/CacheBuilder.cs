using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MbCache.Core;
using MbCache.Core.Events;
using MbCache.Logic;

namespace MbCache.Configuration
{
	public class CacheBuilder
	{
		private readonly IDictionary<Type, ImplementationAndMethods> _cachedMethods;
		private readonly ICollection<ImplementationAndMethods> _details;
		private readonly IProxyFactory _proxyFactory;
		private ICache _cache;
		private readonly ICacheKey _keyBuilder;
		private readonly ProxyValidator _proxyValidator;
		private readonly ICollection<IEventListener> _eventListeners;
		private ILockObjectGenerator _lockObjectGenerator;

		public CacheBuilder(IProxyFactory proxyFactory,
										ICacheKey keyBuilder)
		{
			_cachedMethods = new Dictionary<Type, ImplementationAndMethods>();
			_details = new List<ImplementationAndMethods>();
			_proxyFactory = proxyFactory;
			_keyBuilder = keyBuilder;
			_proxyValidator = new ProxyValidator(_proxyFactory);
			_eventListeners = new List<IEventListener>();
		}

		/// <summary>
		/// Builds the <see cref="IMbCacheFactory"/>.
		/// </summary>
		public IMbCacheFactory BuildFactory()
		{
			checkAllImplementationAndMethodsAreOk();
			if (_cache == null)
			{
				_cache = new InMemoryCache(20);
			}
			var cacheAdapter = new CacheAdapter(_cache, _eventListeners);
			return new MbCacheFactory(_proxyFactory, cacheAdapter, _keyBuilder, _lockObjectGenerator, _cachedMethods);
		}

		/// <summary>
		/// Creates a caching component for <see cref="T"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IFluentBuilder<T> For<T>()
		{
			var concreteType = typeof(T);
			var details = new ImplementationAndMethods(concreteType);
			_details.Add(details);
			var fluentBuilder = new FluentBuilder<T>(this, _cachedMethods, details, _proxyValidator);
			return fluentBuilder;
		}

		private void checkAllImplementationAndMethodsAreOk()
		{
			if (_details.Count > _cachedMethods.Count)
			{
				var excText = new StringBuilder();
				excText.AppendLine("Missing return type (.As<T>() or .AsImplemented()) for");
				var fullyDefined = _cachedMethods.Values;
				foreach (var undefined in _details.Except(fullyDefined))
				{
					excText.AppendLine(undefined.ConcreteType.ToString());
				}
				throw new InvalidOperationException(excText.ToString());
			}
		}

		/// <summary>
		/// Adds an <see cref="IEventListener"/> that will be called in runtime. 
		/// They will be called in the same order as added here.
		/// </summary>
		/// <param name="eventListener"></param>
		public CacheBuilder AddEventListener(IEventListener eventListener)
		{
			_eventListeners.Add(eventListener);
			return this;
		}

		/// <summary>
		/// Sets the <see cref="ILockObjectGenerator"/> to be used.
		/// </summary>
		/// <param name="lockObjectGenerator"></param>
		/// <returns></returns>
		public CacheBuilder SetLockObjectGenerator(ILockObjectGenerator lockObjectGenerator)
		{
			_lockObjectGenerator = lockObjectGenerator;
			return this;
		}

		/// <summary>
		/// Sets the <see cref="ICache"/> to be used.
		/// </summary>
		/// <param name="cache"></param>
		/// <returns></returns>
		public CacheBuilder SetCache(ICache cache)
		{
			_cache = cache;
			return this;
		}
	}
}