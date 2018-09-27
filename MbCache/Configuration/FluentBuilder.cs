using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MbCache.Logic;

namespace MbCache.Configuration
{
	public class FluentBuilder<T>
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

		/// <summary>
		/// Use caching for specified method
		/// </summary>
		/// <param name="expression">Method to cache as an expression</param>
		/// <returns></returns>
		public FluentBuilder<T> CacheMethod(Expression<Func<T, object>> expression)
		{
			var method = ExpressionHelper.MemberName(expression.Body);
			_details.CachedMethods.Add(method);
			return this;
		}

		/// <summary>
		/// Unique cache per component instance?
		/// </summary>
		/// <returns></returns>
		public FluentBuilder<T> PerInstance()
		{
			_details.CachePerInstance = true;
			return this;
		}

		/// <summary>
		/// Sets a specific <see cref="ICacheKey"/> for this component.
		/// </summary>
		/// <param name="cacheKey"></param>
		/// <returns></returns>
		public FluentBuilder<T> CacheKey(ICacheKey cacheKey)
		{
			_details.CacheKey = cacheKey;
			return this;
		}
		
		/// <summary>
		/// Allow having parameters to cached component methods that returns its own type in its <code>ToString()</code> implementation.
		/// Normally this should not be accepted because this will lead to shared cached data for different parameter values.
		/// Instead this parameter type needs to be handled in <see cref="CacheKeyBase.ParameterValue"/>.
		/// </summary>
		public FluentBuilder<T> AllowDifferentArgumentsShareSameCacheKey()
		{
			_details.AllowDifferentArgumentsShareSameCacheKey = true;
			return this;
		}


		/// <summary>
		/// Registers the component to specified interface.
		/// </summary>
		/// <typeparam name="TInterface"></typeparam>
		public CacheBuilder As<TInterface>()
		{
			if(typeof(TInterface).IsClass)
				ProxyValidator.Validate(_details);
			addToCachedMethods(typeof(TInterface));
			return _cacheBuilder;
		}
		/// <summary>
		/// Registers the component on the class itself.
		/// </summary>
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