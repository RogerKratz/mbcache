using System;
using System.Linq.Expressions;
using MbCache.Configuration;

namespace MbCache.Core;

/// <summary>
/// Implemented by all components created by MbCache.
/// </summary>
public interface ICachingComponent
{
	/// <summary>
	/// A unique string representation per component.
	/// </summary>
	string UniqueId { get; }

	/// <summary>
	/// Invalidates all cache entries for this component.
	/// </summary>
	void Invalidate();

	/// <summary>
	/// Invalidated cache entries for a specific <paramref name="method"/>.
	/// If <paramref name="matchParameterValues"/> is <code>true</code>,
	/// only the entry for the method with specified parameter 
	/// values is invalidated.
	/// If <paramref name="matchParameterValues"/> is <code>false</code>,
	/// all entries for the specified method are invalidated.
	/// </summary>
	void Invalidate<T>(Expression<Func<T, object>> method, bool matchParameterValues);
		
	/// <summary>
	/// Throws if parameter's parameter key equals its <see cref="Type"/>.
	/// This can be overriden by setting <see cref="FluentBuilder{T}.AllowDifferentArgumentsShareSameCacheKey"/>.
	/// </summary>
	void CheckIfSuspiciousParameter(object parameter, string parameterKey);
}