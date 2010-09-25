namespace MbCache.Core
{
    public interface IStatistics
    {
        void Clear();
        long CacheHits { get; }
        long CacheMisses { get; }
    }
}