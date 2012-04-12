using System;
using MbCache.Configuration;

namespace MbCacheTest.CacheForTest
{
	public class NoCache : ICache
	{
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
			get { throw new NotImplementedException(); }
		}
	}
}