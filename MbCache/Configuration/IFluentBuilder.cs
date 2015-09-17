using System;
using System.Linq.Expressions;

namespace MbCache.Configuration
{
	public interface IFluentBuilder<T>
	{
		/// <summary>
		/// Use caching for specified method
		/// </summary>
		/// <param name="expression">Method to cache as an expression</param>
		/// <returns></returns>
		IFluentBuilder<T> CacheMethod(Expression<Func<T, object>> expression);

		/// <summary>
		/// Unique cache per component instance?
		/// </summary>
		/// <returns></returns>
		IFluentBuilder<T> PerInstance();

		/// <summary>
		/// Sets a specific <see cref="ICacheKey"/> for this component.
		/// </summary>
		/// <param name="cacheKey"></param>
		/// <returns></returns>
		IFluentBuilder<T> CacheKey(ICacheKey cacheKey);

		/// <summary>
		/// Registers the component to specified interface.
		/// </summary>
		/// <typeparam name="TInterface"></typeparam>
		CacheBuilder As<TInterface>();

		/// <summary>
		/// Registers the component on the class itself.
		/// </summary>
		CacheBuilder AsImplemented();
	}
}