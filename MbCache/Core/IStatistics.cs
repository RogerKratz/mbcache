namespace MbCache.Core
{
	public interface IStatistics
	{
		/// <summary>
		/// Clear statistics
		/// </summary>
		void Clear();

		/// <summary>
		/// Number of cache hits to the underlying cache framework.
		/// </summary>
		long CacheHits { get; }

		/// <summary>
		/// Number of cache misses to the underlying cache framework.
		/// </summary>
		long CacheMisses { get; }
	}
}