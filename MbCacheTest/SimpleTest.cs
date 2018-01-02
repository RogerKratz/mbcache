using System;
using MbCache.Configuration;
using MbCacheTest.Caches;
using NUnit.Framework;

namespace MbCacheTest
{
	public abstract class SimpleTest
	{
		private readonly IProxyFactory _proxyFactory;

		protected SimpleTest(Type proxyType)
		{
			_proxyFactory = (IProxyFactory) Activator.CreateInstance(proxyType);
		}

		[SetUp]
		public void Setup()
		{
			CacheBuilder = new CacheBuilder(_proxyFactory)
					.SetCache(CreateCache())
					.SetCacheKey(CreateCacheKey());
			TestSetup();
		}

		protected virtual void TestSetup(){}

		protected CacheBuilder CacheBuilder { get; private set; }

		protected virtual ICache CreateCache()
		{
			Tools.ClearMemoryCache();
			return new InMemoryCache(20);
		}

		protected virtual ICacheKey CreateCacheKey()
		{
			return new ToStringCacheKey();
		}
	}
}