using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic
{
	public class InvalidateCachePerInstanceTest
	{
		private IMbCacheFactory factory;
		private IObjectReturningNewGuids obj1;
		private IObjectReturningNewGuids obj2;


		[SetUp]
		public void Setup()
		{
			var builder = new CacheBuilder(ConfigurationData.ProxyFactory, new TestCache(), new ToStringMbCacheKey());

			builder
				.For<ObjectReturningNewGuids>()
				.CacheMethod(c => c.CachedMethod())
				.CacheMethod(c => c.CachedMethod2())
				.PerInstance()
				.As<IObjectReturningNewGuids>();

			builder
				.For<ObjectWithParametersOnCachedMethod>()
				.CacheMethod(c => c.CachedMethod(null))
				.PerInstance()
				.As<IObjectWithParametersOnCachedMethod>();

			factory = builder.BuildFactory();
			obj1 = factory.Create<IObjectReturningNewGuids>();
			obj2 = factory.Create<IObjectReturningNewGuids>();
		}

		[Test]
		public void InvalidateByInstance()
		{
			var value1 = obj1.CachedMethod();
			var value2 = obj2.CachedMethod();

			factory.Invalidate(obj1);

			value1.Should().Not.Be.EqualTo(obj1.CachedMethod());
			value2.Should().Be.EqualTo(obj2.CachedMethod());
		}

		[Test]
		public void InvalidateSpecificMethod()
		{
			var value1 = obj1.CachedMethod();
			var value2 = obj2.CachedMethod();

			factory.Invalidate(obj1, method => obj1.CachedMethod(), false);

			value1.Should().Not.Be.EqualTo(obj1.CachedMethod());
			value2.Should().Be.EqualTo(obj2.CachedMethod());
		}

		[Test]
		public void InvalidateSpecificMethodWithSpecificParameter()
		{
			var objWithParam = factory.Create<IObjectWithParametersOnCachedMethod>();
			var objWithParam2 = factory.Create<IObjectWithParametersOnCachedMethod>();
			var value1 = objWithParam.CachedMethod("roger");
			var value2 = objWithParam2.CachedMethod("roger");
			factory.Invalidate(objWithParam, method => method.CachedMethod("roger"), true);

			value1.Should().Not.Be.EqualTo(objWithParam.CachedMethod("roger"));
			value2.Should().Be.EqualTo(objWithParam2.CachedMethod("roger"));
		}
	}
}