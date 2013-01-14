using MbCache.Core;
using MbCache.Core.Events;

namespace MbCache.Configuration
{
	/// <summary>
	/// Object communicating with 3rd part cache framework
	/// </summary>
	public interface ICache
	{
		/// <summary>
		/// Called once before caching is used.
		/// </summary>
		void Initialize(EventListenersCallback eventListenersCallback, ICacheKeyUnwrapper cacheKeyUnwrapper);

		/// <summary>
		/// Gets the cached object.
		/// </summary>
		/// <param name="eventInformation">The <see cref="EventInformation"/> of this cache entry</param>
		CachedItem Get(EventInformation eventInformation);

		/// <summary>
		/// Puts <paramref name="cachedItem"/> to the cache.
		/// </summary>
		void Put(CachedItem cachedItem);

		/// <summary>
		/// Deletes all cache entries starting with <paramref name="keyStartingWith"/>.
		/// </summary>
		/// <param name="keyStartingWith">The key to search for.</param>
		void Delete(string keyStartingWith);
	}
}