using System;
using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic;

[Ignore("#34 To be fixed")]
public class CachingPerMbCacheFactoryTest : TestCase
{
	private IMbCacheFactory factory1;
	private IMbCacheFactory factory2;
	
		
	protected override void TestSetup()
	{
		CacheBuilder
			.For<ObjectReturningNewGuids>()
			.CacheMethod(c => c.CachedMethod())
			.As<IObjectReturningNewGuids>();
			
		var cacheBuilder2 = new CacheBuilder()
			.SetProxyFactory(ProxyFactory)
			.SetCache(CreateCache())
			.SetCacheKey(CreateCacheKey())
			.For<ObjectReturningNewGuids>()
			.CacheMethod(c => c.CachedMethod())
			.As<IObjectReturningNewGuids>();

		factory1 = CacheBuilder.BuildFactory();
		factory2 = cacheBuilder2.BuildFactory();
	}

	[Test]
	public void DifferentFactoriesHaveTheirOwnCache()
	{
		var obj = factory1.Create<IObjectReturningNewGuids>();
		var obj2 = factory2.Create<IObjectReturningNewGuids>();

		obj.CachedMethod()
			.Should().Not.Be.EqualTo(obj2.CachedMethod());
	}
}