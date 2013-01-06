using MbCache.Configuration;
using MbCache.Core;
using MbCache.Logic;

namespace MbCacheTest.CacheForTest
{
	public class NoCache : ICache
	{
		public void Initialize(ICacheKey cacheKey, CacheAdapter cacheAdapter)
		{
		}

		public CachedItem Get(string key)
		{
			return null;
		}

		public void Put(string key, CachedItem value)
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