using MbCache.Configuration;

namespace MbCacheTest.CacheForTest
{
	public class NoCache : ICache
	{
		public void Initialize(ICacheKey cacheKey)
		{
		}

		public object Get(string key)
		{
			return null;
		}

		public void Put(string key, object value)
		{
		}

		public void Delete(string keyStartingWith)
		{
		}

		public ILockObjectGenerator LockObjectGenerator
		{
			get { return null; }
		}
	}
}