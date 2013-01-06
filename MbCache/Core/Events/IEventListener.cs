using MbCache.Configuration;

namespace MbCache.Core.Events
{
	/// <summary>
	/// Can be implemented by users to get events from MbCache
	/// </summary>
	public interface IEventListener
	{
		/// <summary>
		/// Called after a successful <see cref="ICache.Get"/> has occured.
		/// Will not fire after a cache miss (use <see cref="OnPut"/> instead)
		/// </summary>
		/// <param name="cachedItem">
		/// If cachedItem.CachedValue equals <code>null</code>, an unsuccessful Get has occurred.
		/// If cachedItem.CachedValue is of type <see cref="NullValue"/>, <code>null</code> was cached.
		/// </param>
		void OnGet(CachedItem cachedItem);

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