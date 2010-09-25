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
        /// Number of cache misses
        /// </summary>
        long CacheMisses { get; }
    }
}