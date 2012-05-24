namespace MbCache.Configuration
{
	/// <summary>
	/// Generate instances used for locking
	/// </summary>
	public interface ILockObjectGenerator
	{
		/// <summary>
		/// Gets the lock object instance for a spcific <paramref name="key"/>.
		/// Two identical <paramref name="key"/> must return same instance.
		/// Two non identical <paramref name="key"/> can return same instance,
		/// but that could result in more locking than needed.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>Lock object for the key. Returns null if no locking should occur</returns>
		object GetFor(string key);
	}
}