using System;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Scope;

public class CachePerScopeTest : TestCase
{
	private IMbCacheFactory factory;


	protected override void TestSetup()
	{
		CacheBuilder.For<ReturningRandomNumbers>()
			.CacheMethod(c => c.CachedNumber())
			.OverrideCacheKey(new CacheKeyWithScope())
			.As<IReturningRandomNumbers>();

		factory = CacheBuilder.BuildFactory();
	}


	[Test]
	public void ShouldHaveDifferentCachesPerScope()
	{
		var instance = factory.Create<IReturningRandomNumbers>();
		CacheKeyWithScope.CurrentScope = "1";
		var valueForScope1 = instance.CachedNumber();
		CacheKeyWithScope.CurrentScope = "2";
		instance.CachedNumber()
			.Should().Not.Be.EqualTo(valueForScope1);
	}

	[Test]
	public void ShouldCacheInScope()
	{
		var instance = factory.Create<IReturningRandomNumbers>();
		CacheKeyWithScope.CurrentScope = "1";
		var valueForScope1 = instance.CachedNumber();
		instance.CachedNumber()
			.Should().Be.EqualTo(valueForScope1);
	}
}