using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.ClassProxy
{
	[TestFixture]
	public class SimpleCacheAndInvalidationTest
	{
		private IMbCacheFactory factory;

		[SetUp]
		public void Setup()
		{
			var builder = new CacheBuilder(ConfigurationData.ProxyFactory, new TestCache(), new ToStringMbCacheKey());

			builder.For<ObjectReturningNewGuidsNoInterface>()
				 .CacheMethod(c => c.CachedMethod())
				 .CacheMethod(c => c.CachedMethod2())
				 .AsImplemented();

			factory = builder.BuildFactory();
		}

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
}