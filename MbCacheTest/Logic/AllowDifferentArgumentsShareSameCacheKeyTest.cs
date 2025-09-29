using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic;

public class AllowDifferentArgumentsShareSameCacheKeyTest : TestCase
{
	private IMbCacheFactory factory;

	protected override void TestSetup()
	{
		CacheBuilder
			.For<ObjectWithParametersOnCachedMethod>()
			.CacheMethod(c => c.CachedMethod(null))
			.AllowDifferentArgumentsShareSameCacheKey()
			.As<IObjectWithParametersOnCachedMethod>();
			
		factory = CacheBuilder.BuildFactory();
	}

	[Test]
	public void ShouldNotThrowIfSuspiciousParameterIsUsed()
	{
		var comp = factory.Create<IObjectWithParametersOnCachedMethod>();
		Assert.DoesNotThrow(() => comp.CachedMethod(this));
	}
}