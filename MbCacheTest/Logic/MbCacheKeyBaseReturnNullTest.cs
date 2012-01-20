using MbCache.Configuration;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic
{
	[TestFixture]
	public class MbCacheKeyBaseReturnNullTest
	{
		[Test]
		public void ShouldNotAddToCacheUsingParameters()
		{
			var builder = new CacheBuilder(ConfigurationData.ProxyFactory, new TestCache(), new mbCacheStub());

			builder
				 .For<ObjectWithParametersOnCachedMethod>()
				 .CacheMethod(c => c.CachedMethod(null))
				 .As<IObjectWithParametersOnCachedMethod>();

			var factory = builder.BuildFactory();

			var svc = factory.Create<IObjectWithParametersOnCachedMethod>();
			var obj = new object();
			svc.CachedMethod(obj)
				.Should().Not.Be.EqualTo(svc.CachedMethod(obj));
		}

		[Test]
		public void ShouldNotInvalidateMethod()
		{
			var stub = new mbCacheStub { TheKey = "aKey" };
			var builder = new CacheBuilder(ConfigurationData.ProxyFactory, new TestCache(), stub);

			builder
				 .For<ObjectWithParametersOnCachedMethod>()
				 .CacheMethod(c => c.CachedMethod(null))
				 .As<IObjectWithParametersOnCachedMethod>();

			var factory = builder.BuildFactory();

			var svc = factory.Create<IObjectWithParametersOnCachedMethod>();
			var parameter = new object();

			var result = svc.CachedMethod(parameter);

			stub.TheKey = null;
			factory.Invalidate(svc, method => method.CachedMethod(parameter), true);
			stub.TheKey = "aKey";

			svc.CachedMethod(parameter).Should().Be.EqualTo(result);
		}

		private class mbCacheStub : MbCacheKeyBase
		{
			public string TheKey { get; set; }

			protected override string ParameterValue(object parameter)
			{
				return TheKey;
			}
		}
	}
}