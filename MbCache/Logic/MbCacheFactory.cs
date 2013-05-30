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
		private readonly IDictionary<Type, ImplementationAndMethods> _methods;
		private const string isNotARegisteredComponentMessage = "{0} is not a registered MbCache component!";

		public MbCacheFactory(IProxyFactory proxyFactory,
									CacheAdapter cache,
									ICacheKey cacheKey,
									ILockObjectGenerator lockObjectGenerator,
									IDictionary<Type, ImplementationAndMethods> methods)
		{
			_cache = cache;
			_cacheKey = cacheKey;
			_methods = methods;
			proxyFactory.Initialize(_cache, cacheKey, lockObjectGeneratorOrNullObject(lockObjectGenerator));
			_proxyFactory = proxyFactory;
		}

		public T Create<T>(params object[] parameters) where T : class
		{
			var type = typeof (T);
			ImplementationAndMethods methods;
			if (_methods.TryGetValue(type, out methods))
			{
				return _proxyFactory.CreateProxy<T>(_methods[type], parameters);	
			}
			throw new ArgumentException(string.Format(isNotARegisteredComponentMessage, type));
		}

		public void Invalidate<T>()
		{
			var type = typeof (T);
			var cacheKey = _cacheKey.Key(type);
			var deleteInfo = new EventInformation(cacheKey, type, null, null);
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
			ImplementationAndMethods methods;
			if(!_methods.TryGetValue(componentType, out methods))
				throw new ArgumentException(string.Format(isNotARegisteredComponentMessage, componentType.FullName));
			return methods.ConcreteType;
		}

		public void DisableCache<T>(bool evictCacheEntries = true)
		{
			_methods[typeof (T)].EnabledCache = false;
			if (evictCacheEntries)
			{
				Invalidate<T>();				
			}
		}

		public void EnableCache<T>()
		{
			_methods[typeof(T)].EnabledCache = true;
		}

		private static ICachingComponent castToCachingComponentOrThrow(object component)
		{
			var comp = component as ICachingComponent;
			if (comp == null)
				throw new ArgumentException(string.Format(isNotARegisteredComponentMessage, component));
			return comp;
		}

		private static ILockObjectGenerator lockObjectGeneratorOrNullObject(ILockObjectGenerator lockObjectGenerator)
		{
			return lockObjectGenerator ?? new nullLockObjectGenerator();
		}

		[Serializable]
		private class nullLockObjectGenerator : ILockObjectGenerator
		{
			public object GetFor(string key)
			{
				return null;
			}
		}
	}
}