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
		/// <param name="cachedValue">
		/// If <code>null</code>, an unsuccessful Get has occurred.
		/// If value of type <see cref="NullValue"/>, <code>null</code> was cached.
		/// </param>
		void OnGet(EventInformation info, object cachedValue);

		/// <summary>
		/// Called after cache entries has been invalidated.
		/// </summary>
		void OnDelete(EventInformation info);

		/// <summary>
		/// Called after a cache miss and the target's returned value has been put into the cache.
		/// </summary>
		void OnPut(EventInformation info, object cachedValue);
	}
}