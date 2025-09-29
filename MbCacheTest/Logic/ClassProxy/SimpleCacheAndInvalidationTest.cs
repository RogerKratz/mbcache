using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.ClassProxy;

public class SimpleCacheAndInvalidationTest
{
	private IMbCacheFactory factory;


	[SetUp]
	public void Setup() =>
		factory = new CacheBuilder()
			.For<ObjectReturningNewGuidsNoInterface>()
			.CacheMethod(c => c.CachedMethod())
			.CacheMethod(c => c.CachedMethod2())
			.AsImplemented().BuildFactory();

	[Test]
	public void ShouldCacheMethod()
	{
		var obj1 = factory.Create<ObjectReturningNewGuidsNoInterface>();
		var obj2 = factory.Create<ObjectReturningNewGuidsNoInterface>();

		obj1.CachedMethod().Should().Be.EqualTo(obj2.CachedMethod());
	}

	[Test]
	public void CanInvalidate()
	{
		var obj = factory.Create<ObjectReturningNewGuidsNoInterface>();
		var value1 = obj.CachedMethod();
		factory.Invalidate<ObjectReturningNewGuidsNoInterface>();
		obj.CachedMethod().Should().Not.Be.EqualTo(value1);
	}
}