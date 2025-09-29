using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Wrap;

public class ClassComponentTest
{
	private IMbCacheFactory factory;

	[SetUp]
	public void Setup() =>
		factory = new CacheBuilder()
			.For<ObjectReturningNewGuidsNoInterface>()
			.CacheMethod(c => c.CachedMethod())
			.CacheMethod(c => c.CachedMethod2())
			.AsImplemented()
			.BuildFactory();

	[Test]
	public void ShouldWrapUncachingComponent()
	{
		var uncached = new ObjectReturningNewGuidsNoInterface();
		uncached.CachedMethod().Should().Not.Be.EqualTo(uncached.CachedMethod());

		var cached = factory.ToCachedComponent(uncached);
		cached.CachedMethod().Should().Be.EqualTo(cached.CachedMethod());
	}
}