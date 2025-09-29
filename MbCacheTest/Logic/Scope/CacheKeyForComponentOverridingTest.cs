using System;
using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Scope;

public class CacheKeyForComponentOverridingTest : TestCase
{
	private IMbCacheFactory factory;

	protected override ICacheKey CreateCacheKey()
	{
		return new CacheKeyThatThrows();
	}

	protected override void TestSetup()
	{
		CacheBuilder.For<ReturningRandomNumbers>()
			.CacheMethod(c => c.CachedNumber())
			.OverrideCacheKey(new ToStringCacheKey())
			.As<IReturningRandomNumbers>();

		factory = CacheBuilder.BuildFactory();
	}

	[Test]
	public void ShouldHaveDifferentCachesPerScope()
	{
		var instance = factory.Create<IReturningRandomNumbers>();
		instance.CachedNumber()
			.Should().Be.EqualTo(instance.CachedNumber());
	}

}