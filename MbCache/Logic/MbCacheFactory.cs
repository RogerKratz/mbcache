using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MbCache.Configuration;
using MbCache.Core;

namespace MbCache.Logic
{
	public class MbCacheFactory : IMbCacheFactory
	{
		private readonly IProxyFactory _proxyFactory;
		private readonly CacheDecorator _cache;
		private readonly IMbCacheKey _cacheKey;
		private readonly IDictionary<Type, ImplementationAndMethods> _methods;

		public MbCacheFactory(IProxyFactory proxyFactory,
									ICache cache,
									IMbCacheKey cacheKey,
									ILockObjectGenerator lockObjectGenerator,
									IDictionary<Type, ImplementationAndMethods> methods)
		{
			_cache = new CacheDecorator(cache);
			_cacheKey = cacheKey;
			_methods = methods;
			proxyFactory.Initialize(_cache, cacheKey, lockObjectGeneratorOrNullObject(lockObjectGenerator));
			_proxyFactory = proxyFactory;
		}

		public T Create<T>(params object[] parameters) where T : class
		{
			return _proxyFactory.CreateProxy<T>(_methods[typeof(T)], parameters);
		}

		public void Invalidate<T>()
		{
			_cache.Delete(_cacheKey.Key(typeof(T)));
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

		private ILockObjectGenerator lockObjectGeneratorOrNullObject(ILockObjectGenerator lockObjectGenerator)
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