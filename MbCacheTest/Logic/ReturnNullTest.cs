using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic
{
	[TestFixture]
	public class ReturnNullTest
	{
		private IMbCacheFactory factory;

		[SetUp]
		public void Setup()
		{
			var builder = new CacheBuilder(ConfigurationData.ProxyFactory, new AspNetCache(1), new ToStringMbCacheKey());
			builder
				.For<ObjectReturningNull>()
				.CacheMethod(c => c.ReturnNullIfZero(0))
				.As<IObjectReturningNull>();
			factory = builder.BuildFactory();
		}

		[Test]
		public void ShouldCacheNullParameterAndReturnValue()
		{
			factory.Statistics.Clear();
			var component = factory.Create<IObjectReturningNull>();
			component.ReturnNullIfZero(0).Should().Be.Null();
			component.ReturnNullIfZero(0).Should().Be.Null();
			factory.Statistics.CacheHits.Should().Be.EqualTo(1);
		}

		[Test]
		public void ShouldCacheNormalForNullableComponent()
		{
			factory.Statistics.Clear();
			var component = factory.Create<IObjectReturningNull>();
			component.ReturnNullIfZero(1).Should().Be.EqualTo(1);
			component.ReturnNullIfZero(1).Should().Be.EqualTo(1);
			factory.Statistics.CacheHits.Should().Be.EqualTo(1);
		}
	}
}