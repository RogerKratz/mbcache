using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic;

public class AllowDifferentArgumentsShareSameCacheKeyTest
{
	private IMbCacheFactory factory;

	[SetUp]
	public void Setup() =>
		factory = new CacheBuilder()
			.For<ObjectWithParametersOnCachedMethod>()
			.CacheMethod(c => c.CachedMethod(null))
			.AllowDifferentArgumentsShareSameCacheKey()
			.As<IObjectWithParametersOnCachedMethod>()
			.BuildFactory();

	[Test]
	public void ShouldNotThrowIfSuspiciousParameterIsUsed()
	{
		var comp = factory.Create<IObjectWithParametersOnCachedMethod>();
		Assert.DoesNotThrow(() => comp.CachedMethod(this));
	}
}