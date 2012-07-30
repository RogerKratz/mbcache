using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Core.Events;

namespace MbCache.Logic
{
	public class MbCacheFactory : IMbCacheFactory
	{
		private readonly IProxyFactory _proxyFactory;
		private readonly CacheAdapter _cache;
		private readonly ICacheKey _cacheKey;
		private readonly IDictionary<Type, ImplementationAndMethods> _methods;

		public MbCacheFactory(IProxyFactory proxyFactory,
									ICache cache,
									ICacheKey cacheKey,
									ILockObjectGenerator lockObjectGenerator,
									IDictionary<Type, ImplementationAndMethods> methods)
		{
			_cache = new CacheAdapter(cache);
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
			throw new ArgumentException(type + " is not registered as a MbCache component!");
		}

		public void Invalidate<T>()
		{
			var type = typeof (T);
			var cacheKey = _cacheKey.Key(type);
			var deleteInfo = new DeleteInfo(cacheKey, type, null, null);
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

		public IStatistics Statistics
		{
			get { return _cache; }
		}

		public Type ImplementationTypeFor(Type componentType)
		{
			ImplementationAndMethods methods;
			if(!_methods.TryGetValue(componentType, out methods))
				throw new ArgumentException(componentType.FullName + " is not a registered component.");
			return methods.ConcreteType;
		}

		private static ICachingComponent castToCachingComponentOrThrow(object component)
		{
			var comp = component as ICachingComponent;
			if (comp == null)
				throw new ArgumentException(component + " is not an ICachingComponent. Unknown object for MbCache.");
			return comp;
		}

		private static ILockObjectGenerator lockObjectGeneratorOrNullObject(ILockObjectGenerator lockObjectGenerator)
		{
			return lockObjectGenerator ?? new nullLockObjectGenerator();
		}

		private class nullLockObjectGenerator : ILockObjectGenerator
		{
			public object GetFor(string key)
			{
				return null;
			}
		}
	}
}