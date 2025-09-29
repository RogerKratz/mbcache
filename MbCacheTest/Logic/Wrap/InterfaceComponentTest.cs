using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Wrap;

public class InterfaceComponentTest
{
	private IMbCacheFactory factory;

	[SetUp]
	public void Setup() =>
		factory = new CacheBuilder()
			.For<ReturningRandomNumbers>()
			.CacheMethod(c => c.CachedNumber())
			.CacheMethod(c => c.CachedNumber2())
			.As<IReturningRandomNumbers>()
			.BuildFactory();

	[Test]
	public void ShouldWrapUncachingComponent()
	{
		var uncached = new ReturningRandomNumbers();
		uncached.CachedNumber().Should().Not.Be.EqualTo(uncached.CachedNumber());

		var cached = factory.ToCachedComponent<IReturningRandomNumbers>(uncached);
		cached.CachedNumber().Should().Be.EqualTo(cached.CachedNumber());
	}
}