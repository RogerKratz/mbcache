using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic
{
	public class ReturnNullTest : FullTest
	{
		private IMbCacheFactory factory;

		public ReturnNullTest(string proxyTypeString) : base(proxyTypeString) {}

		protected override void TestSetup()
		{
			CacheBuilder
				.For<ObjectReturningNull>()
				.CacheMethod(c => c.ReturnNullIfZero(0))
				.As<IObjectReturningNull>();
			factory = CacheBuilder.BuildFactory();
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