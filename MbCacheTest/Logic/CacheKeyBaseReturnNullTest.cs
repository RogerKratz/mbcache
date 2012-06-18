using MbCache.Configuration;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic
{
	public class CacheKeyBaseReturnNullTest : FullTest
	{
		private cacheKeyStub cacheKey;

		public CacheKeyBaseReturnNullTest(string proxyTypeString) : base(proxyTypeString)
		{
		}

		protected override ICacheKey CreateCacheKey()
		{
			cacheKey = new cacheKeyStub();
			return cacheKey;
		}

		[Test]
		public void ShouldNotAddToCacheUsingParameters()
		{
			CacheBuilder
				 .For<ObjectWithParametersOnCachedMethod>()
				 .CacheMethod(c => c.CachedMethod(null))
				 .As<IObjectWithParametersOnCachedMethod>();

			var factory = CacheBuilder.BuildFactory();

			var svc = factory.Create<IObjectWithParametersOnCachedMethod>();
			var obj = new object();
			svc.CachedMethod(obj)
				.Should().Not.Be.EqualTo(svc.CachedMethod(obj));
		}

		[Test]
		public void ShouldNotInvalidateMethod()
		{
			cacheKey.TheKey = "aKey";

			CacheBuilder
				 .For<ObjectWithParametersOnCachedMethod>()
				 .CacheMethod(c => c.CachedMethod(null))
				 .As<IObjectWithParametersOnCachedMethod>();

			var factory = CacheBuilder.BuildFactory();

			var svc = factory.Create<IObjectWithParametersOnCachedMethod>();
			var parameter = new object();

			var result = svc.CachedMethod(parameter);

			cacheKey.TheKey = null;
			factory.Invalidate(svc, method => method.CachedMethod(parameter), true);
			cacheKey.TheKey = "aKey";

			svc.CachedMethod(parameter).Should().Be.EqualTo(result);
		}

		private class cacheKeyStub : CacheKeyBase
		{
			public string TheKey { get; set; }

			protected override string ParameterValue(object parameter)
			{
				return TheKey;
			}
		}
	}
}