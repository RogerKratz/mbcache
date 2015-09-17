using System;
using System.Collections.Generic;
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
		void Initialize(EventListenersCallback eventListenersCallback);

		/// <summary>
		/// Gets cache entry from cache. If not exists, runs <paramref name="originalMethod" /> and puts in cache.
		/// </summary>
		/// <param name="eventInformation"></param>
		/// <param name="dependingRemoveKeys"></param>
		/// <param name="originalMethod"></param>
		/// <returns></returns>
		CachedItem GetAndPutIfNonExisting(EventInformation eventInformation, Func<IEnumerable<string>> dependingRemoveKeys, Func<object> originalMethod);

		/// <summary>
		/// Deletes cache entries from cache.
		/// </summary>
		/// <param name="eventInformation"></param>
		void Delete(EventInformation eventInformation);

		/// <summary>
		/// Deletes all cache entries created by this <see cref="ICache"/> instance.
		/// </summary>
		void Clear();
	}
}