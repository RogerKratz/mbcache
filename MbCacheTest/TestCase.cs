using System;
using MbCache.Configuration;
using MbCache.ProxyImpl.Castle;
using MbCache.ProxyImpl.LinFu;
using MbCacheTest.Caches;
using NUnit.Framework;

namespace MbCacheTest
{
	[TestFixture(typeof(CastleProxyFactory))]
	[TestFixture(typeof(LinFuProxyFactory))]
	public abstract class TestCase
	{
		private readonly IProxyFactory _proxyFactory;

		protected TestCase(Type proxyType)
		{
			_proxyFactory = (IProxyFactory) Activator.CreateInstance(proxyType);
		}

		[SetUp]
		public void Setup()
		{
			Tools.ClearMemoryCache();
			CacheBuilder = new CacheBuilder(_proxyFactory)
					.SetCache(CreateCache())
					.SetCacheKey(CreateCacheKey());
			TestSetup();
		}

		protected virtual void TestSetup(){}

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