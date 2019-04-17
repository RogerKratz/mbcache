using System;
using MbCache.Configuration;
using MbCache.Logic.Proxy;
using MbCache.ProxyImpl.Castle;
using MbCache.ProxyImpl.LinFu;
using MbCacheTest.Caches;
using NUnit.Framework;

namespace MbCacheTest
{
	[TestFixture(typeof(CastleProxyFactory))]
	[TestFixture(typeof(LinFuProxyFactory))]
	[TestFixture(typeof(ProxyFactory))]
	public abstract class TestCase
	{
		protected TestCase(Type proxyType)
		{
			ProxyFactory = (IProxyFactory) Activator.CreateInstance(proxyType);
		}

		[SetUp]
		public void Setup()
		{
			Tools.ClearMemoryCache();
			CacheBuilder = new CacheBuilder()
				.SetProxyFactory(ProxyFactory)
				.SetCache(CreateCache())
				.SetCacheKey(CreateCacheKey());
			TestSetup();
		}

		protected virtual void TestSetup(){}

		protected IProxyFactory ProxyFactory { get; }
		
		protected CacheBuilder CacheBuilder { get; private set; }

		protected virtual ICache CreateCache()
		{
			return new InMemoryCache(TimeSpan.FromMinutes(20));
		}

		protected virtual ICacheKey CreateCacheKey()
		{
			return new ToStringCacheKey();
		}
	}
}