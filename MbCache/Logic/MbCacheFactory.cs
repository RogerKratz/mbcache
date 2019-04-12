using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using MbCache.Configuration;
using MbCache.Core;

namespace MbCache.Logic
{
	public class MbCacheFactory : IMbCacheFactory
	{
		private readonly IProxyFactory _proxyFactory;
		private readonly IDictionary<Type, ConfigurationForType> _configuredTypes;
		private const string isNotARegisteredComponentMessage = "{0} is not a registered MbCache component!";

		public MbCacheFactory(IProxyFactory proxyFactory,
									IDictionary<Type, ConfigurationForType> configuredTypes)
		{
			_configuredTypes = configuredTypes;
			_proxyFactory = proxyFactory;
		}

		public T Create<T>(params object[] parameters) where T : class
		{
			var type = typeof (T);
			if (_configuredTypes.TryGetValue(type, out var configurationForType))
			{
				var target = (T)createTarget(configurationForType.ComponentType.ConcreteType, parameters);
				return _proxyFactory.CreateProxy(target, configurationForType);
			}
			throw new ArgumentException(string.Format(isNotARegisteredComponentMessage, type));
		}

		public T ToCachedComponent<T>(T uncachedComponent) where T : class
		{
			var type = typeof(T);
			if (_configuredTypes.TryGetValue(type, out var configurationForType))
			{
				return _proxyFactory.CreateProxy(uncachedComponent, configurationForType);
			}
			throw new ArgumentException(string.Format(isNotARegisteredComponentMessage, type));
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Invalidate()
		{
			foreach (var configuredType in _configuredTypes.Values)
			{
				configuredType.Cache.Clear();
			}
		}

		public void Invalidate<T>()
		{
			var type = typeof (T);
			if (_configuredTypes.TryGetValue(type, out var configurationForType))
			{
				var componentType = configurationForType.ComponentType;
				var cacheKey = configurationForType.CacheKey.RemoveKey(componentType);
				configurationForType.Cache.Delete(cacheKey);
			}
			else
			{
				throw new ArgumentException(string.Format(isNotARegisteredComponentMessage, type));
			}
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
			if (!_configuredTypes.TryGetValue(componentType, out var configuredType))
			{
				throw new ArgumentException(string.Format(isNotARegisteredComponentMessage, componentType.FullName));
			}

			return configuredType.ComponentType.ConcreteType;
		}

		public void DisableCache<T>(bool evictCacheEntries = true)
		{
			var type = typeof(T);
			if (!_configuredTypes.TryGetValue(type, out var methods))
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
			if (!_configuredTypes.TryGetValue(type, out var methods))
			{
				throw new ArgumentException(string.Format(isNotARegisteredComponentMessage, type.FullName));
			}
			methods.EnabledCache = true;
		}

		private static ICachingComponent castToCachingComponentOrThrow(object component)
		{
			if (!(component is ICachingComponent comp))
			{
				throw new ArgumentException(string.Format(isNotARegisteredComponentMessage, component));
			}

			return comp;
		}
		
		//extract to seperate type
		private static object createTarget(Type type, object[] ctorParameters)
		{
			try
			{
				return Activator.CreateInstance(type, ctorParameters);
			}
			catch (MissingMethodException ex)
			{
				var ctorParamMessage = "Incorrect number of parameters to ctor for type " + type;
				throw new ArgumentException(ctorParamMessage, ex);
			}
		}

		public void Dispose()
		{
			Invalidate();
		}
	}
}