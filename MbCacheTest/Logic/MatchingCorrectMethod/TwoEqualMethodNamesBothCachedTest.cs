using System;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.MatchingCorrectMethod
{
	public class TwoEqualMethodNamesBothCachedTest : FullTest
	{
		private IMbCacheFactory factory;

		public TwoEqualMethodNamesBothCachedTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			CacheBuilder.For<ObjectWithCtorParameters>()
				.CacheMethod(m => m.CachedMethod())
				.As<IObjectWithCtorParameters>();
			CacheBuilder.For<ObjectReturningNewGuids>()
				.CacheMethod(m => m.CachedMethod())
				.As<IObjectReturningNewGuids>();
			factory = CacheBuilder.BuildFactory();
		}

		[Test]
		public void ShouldNotShareCache()
		{
			var one = factory.Create<IObjectReturningNewGuids>();
			var two = factory.Create<IObjectWithCtorParameters>(1, 2);

			one.CachedMethod().Should().Not.Be.EqualTo(two.CachedMethod());
		}
	}
}