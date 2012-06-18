using System;
using System.Collections.Generic;
using System.Reflection;
using MbCache.Core;

namespace MbCache.Configuration
{
	/// <summary>
	/// Builds the cache keys. 
	/// 
	/// For implementers:
	/// In most cases, derive from CacheKeyBase instead.
	/// 
	/// If you implement this interface directly, 
	/// remember every overload with more params
	/// needs to include the less one.
	/// 
	/// Eg, if Key(Type) returns Roger, 
	/// it's valid for Key(Type, MethodInfo) to return Roger2
	/// but not Rog2.
	/// </summary>
	public interface ICacheKey
	{
		/// <summary>
		/// Creates a cache key for a specific type.
		/// </summary>
		/// <param name="type">Type of the cached component.</param>
		/// <remarks>
		/// Used by MbCache when invalidating all cache entries for a specific type.
		/// </remarks>
		/// <returns>
		/// A string representation of the type.
		/// Null is returned if this type is configured but shouldn't be
		/// invalidated at this time.
		/// </returns>
		string Key(Type type);

		/// <summary>
		/// Creates a cache key for a specific component.
		/// </summary>
		/// <param name="type">Type of the component</param>
		/// <param name="component">
		/// The component instance.
		/// </param>
		/// <remarks>
		/// Used by MbCache when invalidating all cache entries for a specific component.
		/// </remarks>
		/// <returns>
		/// A string representation of the component.
		/// Null is returned if the component is configured but shouldn't be
		/// invalidated at this time.
		/// </returns>
		string Key(Type type, ICachingComponent component);

		/// <summary>
		/// Creates a cache key for a specific method.
		/// </summary>
		/// <param name="type">Type of the component</param>
		/// <param name="component">
		/// The component instance.
		/// </param>
		/// <param name="method">
		/// The method of the component.
		/// </param>
		/// <remarks>
		/// Used by MbCache when invalidating all cache entries for a specific method.
		/// </remarks>
		/// <returns>
		/// A string representation of the method.
		/// Null is returned if the method is configured but shouldn't be 
		/// invalidated at this time.
		/// </returns>
		string Key(Type type, ICachingComponent component, MethodInfo method);

		/// <summary>
		/// Creates a cache key for a specific component with specific parameters.
		/// </summary>
		/// <param name="type">Type of the component</param>
		/// <param name="component">
		/// The component instance.
		/// </param>
		/// <param name="method">
		/// The method of the component.
		/// </param>
		/// <param name="parameters">
		/// The parameters sent to the component.
		/// </param>
		/// <remarks>
		/// Used by MbCache when invalidating all cache entries for a specific method,
		/// and when adding an entry to the cache.
		/// </remarks>
		/// <returns>
		/// A string representation of the method and its parameters.
		/// Null is returned if the method is configured but with these specific parameters
		/// shouldn't be added (or invalidated) from the cache.
		/// </returns>
		string Key(Type type, ICachingComponent component, MethodInfo method, IEnumerable<object> parameters);
	}
}