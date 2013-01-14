using System.Collections.Generic;

namespace MbCache.Configuration
{
	/// <summary>
	/// "Unwraps" a cache key string. Part of the <see cref="ICacheKey"/> implementation.
	/// </summary>
	public interface ICacheKeyUnwrapper
	{
		/// <summary>
		/// If the cache key looks like this (and separator is |)...
		/// MbCache|Something|SomethingElse
		/// ...this method should return...
		/// MbCache
		/// MbCache|Something
		/// This is useful if implementation of <see cref="ICache"/> needs to create dependencies for invalidation.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		IEnumerable<string> UnwrapKey(string key);
	}
}