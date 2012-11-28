using System;
using System.Linq;
using System.Runtime.Caching;
using MbCache.Configuration;
using MbCacheTest.CacheForTest;
using NUnit.Framework;

namespace MbCacheTest
{
	public abstract class SimpleTest
	{
		protected SimpleTest(): this("MbCacheTest.ProxyImplThatThrows") 
		{
		}

		protected SimpleTest(string proxyTypeString)
		{
			var proxyType = Type.GetType(proxyTypeString, true);
			ProxyFactory = (IProxyFactory) Activator.CreateInstance(proxyType);
		}

		[SetUp]
		public void Setup()
		{
			var lockObjectGenerator = CreateLockObjectGenerator();
			CacheBuilder = new CacheBuilder(ProxyFactory, CreateCache(), CreateCacheKey());
			if (lockObjectGenerator != null)
				CacheBuilder.SetLockObjectGenerator(lockObjectGenerator);
			TestSetup();
		}

		protected virtual void TestSetup(){}

		protected CacheBuilder CacheBuilder { get; private set; }
		protected IProxyFactory ProxyFactory { get; private set; }

		protected virtual ICache CreateCache()
		{
			clearMemoryCache();
			return new InMemoryCache(1);
		}

		private static void clearMemoryCache()
		{
			foreach (var cacheKey in MemoryCache.Default.Select(kvp => kvp.Key).ToList())
			{
				MemoryCache.Default.Remove(cacheKey);
			}
		}

		protected virtual ICacheKey CreateCacheKey()
		{
			return new ToStringCacheKey();
		}

		protected virtual ILockObjectGenerator CreateLockObjectGenerator()
		{
			return null;
		}
	}
}