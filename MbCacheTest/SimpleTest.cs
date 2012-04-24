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
			CacheBuilder = new CacheBuilder(ProxyFactory, CreateCache(), CreateCacheKey());
			TestSetup();
		}

		protected virtual void TestSetup(){}

		protected CacheBuilder CacheBuilder { get; private set; }
		protected IProxyFactory ProxyFactory { get; private set; }

		protected virtual ICache CreateCache()
		{
			return new TestCache();
		}

		protected virtual IMbCacheKey CreateCacheKey()
		{
			return new ToStringMbCacheKey();
		}
	}
}