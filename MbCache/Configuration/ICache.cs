using System;
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
		/// Gets cache entry from cache. If not exists, runs <paramref name="originalMethod" /> and puts in cache.
		/// </summary>
		/// <param name="eventInformation"></param>
		/// <param name="originalMethod"></param>
		/// <returns></returns>
		CachedItem GetAndPutIfNonExisting(EventInformation eventInformation, Func<object> originalMethod);

		/// <summary>
		/// Deletes cache entries from cache.
		/// </summary>
		/// <param name="eventInformation"></param>
		void Delete(EventInformation eventInformation);
	}
}