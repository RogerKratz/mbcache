using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Wrap;

public class ClassComponentTest : TestCase
{
	private IMbCacheFactory factory;

	protected override void TestSetup()
	{
		CacheBuilder.For<ObjectReturningNewGuidsNoInterface>()
			.CacheMethod(c => c.CachedMethod())
			.CacheMethod(c => c.CachedMethod2())
			.AsImplemented();

		factory = CacheBuilder.BuildFactory();
	}

	[Test]
	public void ShouldWrapUncachingComponent()
	{
		var uncached = new ObjectReturningNewGuidsNoInterface();
		uncached.CachedMethod().Should().Not.Be.EqualTo(uncached.CachedMethod());

		var cached = factory.ToCachedComponent(uncached);
		cached.CachedMethod().Should().Be.EqualTo(cached.CachedMethod());
	}
}