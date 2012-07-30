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
		void OnGet(EventInformation info);

		/// <summary>
		/// Called after a cache entry has been invalidated.
		/// </summary>
		void OnDelete(EventInformation info);

		/// <summary>
		/// Called after a cache miss and the target's returned value has been put into the cache.
		/// </summary>
		void OnPut(EventInformation info);
	}
}