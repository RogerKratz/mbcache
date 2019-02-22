using System;
using MbCache.Configuration;
using MbCache.ProxyImpl.Castle;
using MbCache.ProxyImpl.LinFu;
using NUnit.Framework;

namespace MbCacheTest
{
	[TestFixture(typeof(CastleProxyFactory))]
	[TestFixture(typeof(LinFuProxyFactory))]
	public abstract class TestCase
	{
		protected TestCase(Type proxyType)
		{
			ProxyFactory = (IProxyFactory) Activator.CreateInstance(proxyType);
		}

		[SetUp]
		public void Setup()
		{
			CacheBuilder = new CacheBuilder(ProxyFactory)
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