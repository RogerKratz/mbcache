using System;
using System.Reflection;
using MbCache.Core.Events;

namespace MbCache.Configuration;

/// <summary>
/// Object communicating with 3rd part cache framework
/// </summary>
public interface ICache
{
	/// <summary>
	/// Called once before caching is used.
	/// </summary>
	void Initialize(EventListenersCallback eventListenersCallback);

	/// <summary>
	/// Gets cache entry from cache.
	/// If not exists, runs <paramref name="originalMethod" /> and puts the result in cache if originalMethod.ShouldBeCached is true
	/// </summary>
	object GetAndPutIfNonExisting(KeyAndItsDependingKeys keyAndItsDependingKeys, MethodInfo cachedMethod, Func<OriginalMethodResult> originalMethod);

	/// <summary>
	/// Deletes cache entries from cache.
	/// </summary>
	/// <param name="cacheKey"></param>
	void Delete(string cacheKey);

	/// <summary>
	/// Deletes all cache entries created by this <see cref="ICache"/> instance.
	/// </summary>
	void Clear();
}