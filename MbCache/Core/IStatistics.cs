using MbCache.Configuration;

namespace MbCache.Core
{
	public interface IStatistics
	{
		/// <summary>
		/// Clear statistics
		/// </summary>
		void Clear();

		/// <summary>
		/// Number of cache hits
		/// </summary>
		long CacheHits { get; }

		/// <summary>
		/// Number of attempts to tell MbCache to read from cache when no cache entry was found
		/// </summary>
		long CacheMisses { get; }

		/// <summary>
		/// Number of cache misses to the underlying cache framework.
		/// Depending if locks are used, if <see cref="ILockObjectGenerator"/> is <code>null</code> or not,
		/// multiple cache gets can occur when trying to fetch cached data.
		/// </summary>
		long PhysicalCacheMisses { get; }
	}
}