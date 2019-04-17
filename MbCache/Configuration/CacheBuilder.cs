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
		private readonly IDictionary<Type, ConfigurationForType> _configuredTypes;
		private readonly ICollection<ConfigurationForType> _details;
		private readonly IProxyFactory _proxyFactory;
		private ICache _cache;
		private ICacheKey _cacheKey;
		private readonly ICollection<IEventListener> _eventListeners;

		public CacheBuilder(IProxyFactory proxyFactory)
		{
			_configuredTypes = new Dictionary<Type, ConfigurationForType>();
			_details = new List<ConfigurationForType>();
			_proxyFactory = proxyFactory;
			_eventListeners = new List<IEventListener>();
		}

		/// <summary>
		/// Builds the <see cref="IMbCacheFactory"/>.
		/// </summary>
		public IMbCacheFactory BuildFactory()
		{
			checkAllImplementationAndMethodsAreOk();
			setCacheAndCacheKeys();
			return new MbCacheFactory(_proxyFactory, _configuredTypes);
		}

		private void setCacheAndCacheKeys()
		{
			var defaultCacheKey = new ToStringCacheKey();
			var events = new EventListenersCallback(_eventListeners);
			var allCaches = new HashSet<ICache>();
			
			foreach (var configurationForType in _configuredTypes.Values)
			{
				if (configurationForType.CacheKey == null)
				{
					configurationForType.CacheKey = _cacheKey ?? defaultCacheKey;
				}
				if (configurationForType.Cache == null)
				{
					configurationForType.Cache = _cache ?? new InMemoryCache(TimeSpan.FromMinutes(20));
				}
				allCaches.Add(configurationForType.Cache);
			}
			foreach (var cache in allCaches)
			{
				cache.Initialize(events);
			}
		}

		/// <summary>
		/// Creates a caching component for <see cref="T"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public FluentBuilder<T> For<T>()
		{
			return For<T>(null);
		}

		/// <summary>
		/// Creates a caching component for <see cref="T"/>.
		/// </summary>
		/// <param name="typeAsCacheKey">
		/// Gives a name to this type to be used as cache key.
		/// </param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public FluentBuilder<T> For<T>(string typeAsCacheKey)
		{
			var concreteType = typeof(T);
			var details = new ConfigurationForType(concreteType, typeAsCacheKey);
			_details.Add(details);
			var fluentBuilder = new FluentBuilder<T>(this, _configuredTypes, details);
			return fluentBuilder;
		}

		private void checkAllImplementationAndMethodsAreOk()
		{
			if (_details.Count > _configuredTypes.Count)
			{
				var excText = new StringBuilder();
				excText.AppendLine("Missing return type (.As<T>() or .AsImplemented()) for");
				var fullyDefined = _configuredTypes.Values;
				foreach (var detail in _details.Except(fullyDefined))
				{
					excText.AppendLine(detail.ComponentType.ConcreteType.ToString());
				}
				throw new InvalidOperationException(excText.ToString());
			}
			var tempHash = new HashSet<string>();
			foreach (var detail in _details)
			{
				var typeAsCacheKey = detail.ComponentType.TypeAsCacheKeyString;
				if (!tempHash.Add(typeAsCacheKey))
				{
					throw new InvalidOperationException("Component [" + typeAsCacheKey + "] registered multiple times!");
				}
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
		/// Sets the <see cref="ICache"/> to be used.
		/// </summary>
		/// <param name="cache"></param>
		/// <returns></returns>
		public CacheBuilder SetCache(ICache cache)
		{
			_cache = cache;
			return this;
		}

		/// <summary>
		/// Sets the <see cref="ICacheKey"/> to be used.
		/// </summary>
		/// <param name="cacheKey"></param>
		/// <returns></returns>
		public CacheBuilder SetCacheKey(ICacheKey cacheKey)
		{
			_cacheKey = cacheKey;
			return this;
		}
	}
}