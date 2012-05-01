namespace MbCache.Configuration
{
	/// <summary>
	/// Object communicating with 3rd part cache framework
	/// </summary>
	public interface ICache
	{
		object Get(string key);
		void Put(string key, object value);
		void Delete(string keyStartingWith);
		ILockObjectGenerator LockObjectGenerator { get; }
	}
}