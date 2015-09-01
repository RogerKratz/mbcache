using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Core.Events;

namespace MbCache.Logic
{
	[Serializable]
	public class MbCacheFactory : IMbCacheFactory
	{
		private readonly IProxyFactory _proxyFactory;
		private readonly CacheAdapter _cache;
		private readonly ICacheKey _cacheKey;
		private readonly IDictionary<Type, ConfigurationForType> _configuredTypes;
		private const string isNotARegisteredComponentMessage = "{0} is not a registered MbCache component!";

		public MbCacheFactory(IProxyFactory proxyFactory,
									CacheAdapter cache,
									ICacheKey cacheKey,
									IDictionary<Type, ConfigurationForType> configuredTypes)
		{
			_cache = cache;
			_cacheKey = cacheKey;
			_configuredTypes = configuredTypes;
			proxyFactory.Initialize(_cache, cacheKey);
			_proxyFactory = proxyFactory;
		}

		public T Create<T>(params object[] parameters) where T : class
		{
			var type = typeof (T);
			ConfigurationForType configurationForType;
			if (_configuredTypes.TryGetValue(type, out configurationForType))
			{
				return _proxyFactory.CreateProxy<T>(configurationForType, parameters);
			}
			throw new ArgumentException(string.Format(isNotARegisteredComponentMessage, type));
		}

		public T ToCachedComponent<T>(T uncachedComponent) where T : class
		{
			var type = typeof(T);
			ConfigurationForType configurationForType;
			if (_configuredTypes.TryGetValue(type, out configurationForType))
			{
				return _proxyFactory.CreateProxyWithTarget(uncachedComponent, configurationForType);
			}
			throw new ArgumentException(string.Format(isNotARegisteredComponentMessage, type));
		}

		public void Invalidate()
		{
			_cache.Clear();
		}

		public void Invalidate<T>()
		{
			var type = typeof (T);
			//todo - kolla först!
			var componentType = _configuredTypes[type].ComponentType;
			var cacheKey = _cacheKey.Key(componentType);
			var deleteInfo = new EventInformation(cacheKey, componentType.ConfiguredType, null, null);
			_cache.Delete(deleteInfo);
		}

		public void Invalidate(object component)
		{
			castToCachingComponentOrThrow(component).Invalidate();
		}

		public void Invalidate<T>(T component, Expression<Func<T, object>> method, bool matchParameterValues)
		{
			castToCachingComponentOrThrow(component).Invalidate(method, matchParameterValues);
		}

		public bool IsKnownInstance(object component)
		{
			return component is ICachingComponent;
		}

		public Type ImplementationTypeFor(Type componentType)
		{
			ConfigurationForType configuredType;
			if (!_configuredTypes.TryGetValue(componentType, out configuredType))
				throw new ArgumentException(string.Format(isNotARegisteredComponentMessage, componentType.FullName));
			return configuredType.ComponentType.ConcreteType;
		}

		public void DisableCache<T>(bool evictCacheEntries = true)
		{
			var type = typeof(T);
			ConfigurationForType methods;
			if (!_configuredTypes.TryGetValue(type, out methods))
			{
				throw new ArgumentException(string.Format(isNotARegisteredComponentMessage, type.FullName));
			}
			methods.EnabledCache = false;
			if (evictCacheEntries)
			{
				Invalidate<T>();
			}
		}

		public void EnableCache<T>()
		{
			var type = typeof (T);
			ConfigurationForType methods;
			if (!_configuredTypes.TryGetValue(type, out methods))
				throw new ArgumentException(string.Format(isNotARegisteredComponentMessage, type.FullName));
			methods.EnabledCache = true;
		}

		private static ICachingComponent castToCachingComponentOrThrow(object component)
		{
			var comp = component as ICachingComponent;
			if (comp == null)
				throw new ArgumentException(string.Format(isNotARegisteredComponentMessage, component));
			return comp;
		}

		public void Dispose()
		{
			Invalidate();
		}
	}
}