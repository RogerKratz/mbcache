namespace MbCache.Core.Events
{
	/// <summary>
	/// Can be implemented by users to get events from MbCache
	/// </summary>
	public interface IEventListener
	{
		/// <summary>
		/// Called after a successful Get in cache.
		/// </summary>
		void OnCacheHit(CachedItem cachedItem);

		/// <summary>
		/// Called after cache entries has been invalidated.
		/// </summary>
		void OnCacheRemoval(CachedItem cachedItem);

		/// <summary>
		/// Called after a cache miss and the target's returned value has been put into the cache.
		/// </summary>
		void OnCacheMiss(CachedItem cachedItem);
	}
}