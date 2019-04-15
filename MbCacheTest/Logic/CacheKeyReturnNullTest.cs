using System;
using MbCache.Configuration;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic
{
	public class CacheKeyReturnNullTest : TestCase
	{
		private cacheKeyStub cacheKey;


		public CacheKeyReturnNullTest(Type proxyType) : base(proxyType)
		{
		}

		protected override ICacheKey CreateCacheKey()
		{
			cacheKey = new cacheKeyStub();
			return cacheKey;
		}

		[Test]
		public void ShouldNotAddToCacheUsingParametersWhenNull()
		{
			CacheBuilder
				 .For<ObjectWithParametersOnCachedMethod>()
				 .CacheMethod(c => c.CachedMethod(null))
				 .As<IObjectWithParametersOnCachedMethod>();

			var factory = CacheBuilder.BuildFactory();

			var svc = factory.Create<IObjectWithParametersOnCachedMethod>();
			const int param = 1;
			svc.CachedMethod(param)
				.Should().Not.Be.EqualTo(svc.CachedMethod(param));
		}

		[Test]
		public void ShouldNotInvalidateWhenParameterIsNull()
		{
			cacheKey.ParameterReturnsNull = false;
			CacheBuilder
				 .For<ObjectWithParametersOnCachedMethod>()
				 .CacheMethod(c => c.CachedMethod(null))
				 .As<IObjectWithParametersOnCachedMethod>();

			var factory = CacheBuilder.BuildFactory();
			var svc = factory.Create<IObjectWithParametersOnCachedMethod>();
			const int param = 1;

			var result = svc.CachedMethod(param);

			cacheKey.ParameterReturnsNull = true;
			factory.Invalidate(svc, method => method.CachedMethod(param), true);
			cacheKey.ParameterReturnsNull = false;

			svc.CachedMethod(param).Should().Be.EqualTo(result);
		}

		private class cacheKeyStub : CacheKeyBase
		{
			public bool ParameterReturnsNull { get; set; }

			public cacheKeyStub()
			{
				ParameterReturnsNull = true;
			}
			
			protected override string ParameterValue(object parameter)
			{
				return ParameterReturnsNull ? null : parameter.ToString();
			}
		}
	}
}