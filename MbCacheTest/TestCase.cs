using System;
using MbCache.Configuration;
using MbCache.Logic.Proxy;
using NUnit.Framework;

namespace MbCacheTest;

public abstract class TestCase
{
	[SetUp]
	public void Setup()
	{
		CacheBuilder = new CacheBuilder()
			.SetProxyFactory(new ProxyFactory())
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