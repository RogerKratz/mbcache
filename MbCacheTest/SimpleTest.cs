using System;
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
			CacheBuilder = lockObjectGenerator==null ? new CacheBuilder(ProxyFactory, CreateCache(), CreateCacheKey()) : new CacheBuilder(ProxyFactory, CreateCache(), CreateCacheKey(), lockObjectGenerator);
			TestSetup();
		}

		protected virtual void TestSetup(){}

		protected CacheBuilder CacheBuilder { get; private set; }
		protected IProxyFactory ProxyFactory { get; private set; }

		protected virtual ICache CreateCache()
		{
			return new TestCache();
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