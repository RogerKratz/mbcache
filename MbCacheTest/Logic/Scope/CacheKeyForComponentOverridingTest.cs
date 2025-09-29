using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Scope;

public class CacheKeyForComponentOverridingTest
{
	private IMbCacheFactory factory;
	
	[SetUp]
	public void Setup()
	{
		factory = new CacheBuilder()
			.SetCacheKey(new CacheKeyThatThrows())
			.For<ReturningRandomNumbers>()
				.CacheMethod(c => c.CachedNumber())
				.OverrideCacheKey(new ToStringCacheKey())
				.As<IReturningRandomNumbers>()
			.BuildFactory();
	}

	[Test]
	public void ShouldHaveDifferentCachesPerScope()
	{
		var instance = factory.Create<IReturningRandomNumbers>();
		instance.CachedNumber()
			.Should().Be.EqualTo(instance.CachedNumber());
	}
}