using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MbCache.Logic;

namespace MbCache.Configuration;

public class FluentBuilder<T>(
	CacheBuilder cacheBuilder,
	IDictionary<Type, ConfigurationForType> cachedMethods,
	ConfigurationForType details)
{
	private const string componentRegisteredMultipleEx =
		"Type {0} is already in CacheBuilder. If you want to cache multiple methods on one type, simply call CacheMethod multiple times instead.";

	/// <summary>
	/// Use caching for specified method
	/// </summary>
	/// <param name="expression">Method to cache as an expression</param>
	/// <param name="returnValuesNotToCache">Return values not to cache</param>
	/// <returns></returns>
	public FluentBuilder<T> CacheMethod<T2>(Expression<Func<T, T2>> expression, IEnumerable<T2> returnValuesNotToCache = null)
	{
		var method = ExpressionHelper.MemberName(expression.Body);
		var valuesNotToCache = returnValuesNotToCache == null ? Enumerable.Empty<object>() : returnValuesNotToCache.OfType<object>();
		details.CachedMethods.Add(new CachedMethod(method, valuesNotToCache));
		return this;
	}

	/// <summary>
	/// Unique cache per component instance?
	/// </summary>
	/// <returns></returns>
	public FluentBuilder<T> PerInstance()
	{
		details.CachePerInstance = true;
		return this;
	}

	/// <summary>
	/// Sets a specific <see cref="ICacheKey"/> for this component.
	/// </summary>
	/// <param name="cacheKey"></param>
	/// <returns></returns>
	public FluentBuilder<T> OverrideCacheKey(ICacheKey cacheKey)
	{
		details.CacheKey = cacheKey;
		return this;
	}
		
	/// <summary>
	/// Sets a specific <see cref="ICache"/> for this component.
	/// </summary>
	public FluentBuilder<T> OverrideCache(ICache cache)
	{
		details.Cache = cache;
		return this;
	}
		
	/// <summary>
	/// Allow having parameters to cached component methods that returns its own type in its <code>ToString()</code> implementation.
	/// Normally this should not be accepted because this will lead to shared cached data for different parameter values.
	/// Instead this parameter type needs to be handled in <see cref="CacheKeyBase.ParameterValue"/>.
	/// </summary>
	public FluentBuilder<T> AllowDifferentArgumentsShareSameCacheKey()
	{
		details.AllowDifferentArgumentsShareSameCacheKey = true;
		return this;
	}


	/// <summary>
	/// Registers the component to specified interface.
	/// </summary>
	/// <typeparam name="TInterface"></typeparam>
	public CacheBuilder As<TInterface>()
	{
		if(typeof(TInterface).IsClass)
			ProxyValidator.Validate(details);
		addToCachedMethods(typeof(TInterface));
		return cacheBuilder;
	}
	/// <summary>
	/// Registers the component on the class itself.
	/// </summary>
	public CacheBuilder AsImplemented()
	{
		ProxyValidator.Validate(details);
		addToCachedMethods(details.ComponentType.ConcreteType);
		return cacheBuilder;
	}		
		
	private void addToCachedMethods(Type type)
	{
		if (cachedMethods.ContainsKey(type))
			throw new ArgumentException(string.Format(componentRegisteredMultipleEx, type));
		cachedMethods[type] = details;
	}
}