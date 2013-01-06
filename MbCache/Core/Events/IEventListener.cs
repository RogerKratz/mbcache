namespace MbCache.Core.Events
{
	/// <summary>
	/// Can be implemented by users to get events from MbCache
	/// </summary>
	public interface IEventListener
	{
		/// <summary>
		/// Called after a Get in cache.
		/// </summary>
		void OnGet(CachedItem cachedItem, bool successful);

		/// <summary>
		/// Called after cache entries has been invalidated.
		/// </summary>
		void OnDelete(CachedItem cachedItem);

		/// <summary>
		/// Called after a cache miss and the target's returned value has been put into the cache.
		/// </summary>
		void OnPut(CachedItem cachedItem);
	}
}