using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MbCache.Logic;

namespace MbCache.Configuration
{
	public class FluentBuilder<T> : IFluentBuilder<T>
	{
		private readonly CacheBuilder _cacheBuilder;
		private readonly IDictionary<Type, ConfigurationForType> _cachedMethods;
		private readonly ConfigurationForType _details;

		private const string componentRegisteredMultipleEx =
			@"Type {0} is already in CacheBuilder. If you want to cache multiple methods on one type, simply call CacheMethod multiple times instead.";

		public FluentBuilder(CacheBuilder cacheBuilder,
									IDictionary<Type, ConfigurationForType> cachedMethods, 
									ConfigurationForType details)
		{
			_cacheBuilder = cacheBuilder;
			_cachedMethods = cachedMethods;
			_details = details;
		}

		public IFluentBuilder<T> CacheMethod(Expression<Func<T, object>> expression)
		{
			var method = ExpressionHelper.MemberName(expression.Body);
			_details.CachedMethods.Add(method);
			return this;
		}

		public IFluentBuilder<T> PerInstance()
		{
			_details.CachePerInstance = true;
			return this;
		}

		public IFluentBuilder<T> CacheKey(ICacheKey cacheKey)
		{
			_details.CacheKey = cacheKey;
			return this;
		}

		public CacheBuilder As<TInterface>()
		{
			if(typeof(TInterface).IsClass)
				ProxyValidator.Validate(_details);
			addToCachedMethods(typeof(TInterface));
			return _cacheBuilder;
		}

		public CacheBuilder AsImplemented()
		{
			ProxyValidator.Validate(_details);
			addToCachedMethods(_details.ComponentType.ConcreteType);
			return _cacheBuilder;
		}

		private void addToCachedMethods(Type type)
		{
			if (_cachedMethods.ContainsKey(type))
				throw new ArgumentException(string.Format(componentRegisteredMultipleEx, type));
			_cachedMethods[type] = _details;
		}
	}
}