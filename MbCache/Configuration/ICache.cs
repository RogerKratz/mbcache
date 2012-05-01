namespace MbCache.Configuration
{
	/// <summary>
	/// Object communicating with 3rd part cache framework
	/// </summary>
	public interface ICache
	{
		/// <summary>
		/// Gets the cached object.
		/// </summary>
		/// <param name="key">The key of this cache entry.</param>
		object Get(string key);

		/// <summary>
		/// Puts <paramref name="value"/> to the cache.
		/// </summary>
		/// <param name="key">The key of this cache entry.</param>
		/// <param name="value">The object to cache.</param>
		void Put(string key, object value);

		/// <summary>
		/// Deletes all cache entries starting with <paramref name="keyStartingWith"/>.
		/// </summary>
		/// <param name="keyStartingWith">The key to search for.</param>
		void Delete(string keyStartingWith);

		/// <summary>
		/// An implementation of <see cref="ILockObjectGenerator"/>.
		/// This only needs to be set if two simultanious threads must
		/// not modify cache at the same time.
		/// </summary>
		ILockObjectGenerator LockObjectGenerator { get; }
	}
}