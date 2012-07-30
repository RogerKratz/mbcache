using System;
using System.Linq.Expressions;

namespace MbCache.Core
{
	/// <summary>
	/// Main type at run time.
	/// Creates proxies with (possibly) cached method logic,
	/// depending on configuration.
	/// 
	/// Also have methods to invalidate types or its specific methods
	/// </summary>
	public interface IMbCacheFactory
	{
		/// <summary>
		/// Creates the proxy with ctor parameters.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		T Create<T>(params object[] parameters)
			where T : class;

		/// <summary>
		/// Invalidates all cached methods for type T.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		void Invalidate<T>();

		/// <summary>
		/// Invalidates all cached methods for instance component.
		/// Note: If PerInstance() not is used, this may include several instances.
		/// </summary>
		/// <param name="component">The component.</param>
		void Invalidate(object component);

		/// <summary>
		/// Invalidates method for component instance
		/// Note: If PerInstance() not is used, this may include several cache entries.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="component">The component</param>
		/// <param name="method">The method</param>
		/// <param name="matchParameterValues">
		/// Determines if parameters sent to <para>method</para> should decide what to invalidate.
		/// If <code>false</code>, all method caches are invalidated, 
		/// if <code>true</code>, only the one matching with matching paramater values are invalidated.
		/// </param>
		void Invalidate<T>(T component, Expression<Func<T, object>> method, bool matchParameterValues);

		/// <summary>
		/// Determines if component instance is created by mbcache
		/// </summary>
		/// <param name="component">The component.</param>
		/// <returns></returns>
		bool IsKnownInstance(object component);

		/// <summary>
		/// Returns the "real" type for a registered interface.
		/// Returns the same type as registered type if AsImplemented (class proxy) is used.
		/// </summary>
		/// <returns></returns>
		Type ImplementationTypeFor(Type componentType);
	}
}