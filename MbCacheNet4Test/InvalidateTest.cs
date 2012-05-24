using MbCache.Configuration;
using MbCache.ProxyImpl.LinFu;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheNet4Test
{
	[TestFixture]
	public class InvalidateTestLinFu
	{
		[Test]
		public void InvalidateSpecificMethod()
		{
			var cacheBuilder = new CacheBuilder(new ProxyFactory(), new TestCache(), new ToStringMbCacheKey());
			cacheBuilder
				.For<ObjectReturningNewGuids>()
				.CacheMethod(c => c.CachedMethod())
				.CacheMethod(c => c.CachedMethod2())
				.PerInstance()
				.As<IObjectReturningNewGuids>();

			var factory = cacheBuilder.BuildFactory();

			var obj = factory.Create<IObjectReturningNewGuids>();
			var value1 = obj.CachedMethod();

			factory.Invalidate(obj, method => obj.CachedMethod(), false);

			var value2 = obj.CachedMethod();

			value1.Should().Not.Be.EqualTo(value2);
		}
	}
}