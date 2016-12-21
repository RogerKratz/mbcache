using System;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.MatchingCorrectMethod
{
	public class TwoEqualMethodNamesOneCachedTest : FullTest
	{
		private IMbCacheFactory factory;

		public TwoEqualMethodNamesOneCachedTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			CacheBuilder.For<ObjectWithCtorParameters>()
				.CacheMethod(m => m.CachedMethod())
				.As<IObjectWithCtorParameters>();
			CacheBuilder.For<ObjectReturningNewGuids>()
				.As<IObjectReturningNewGuids>(); 
			factory = CacheBuilder.BuildFactory();
		}

		[Test]
		public void ShouldNotCacheNonConfiguredType()
		{
			var one = factory.Create<IObjectReturningNewGuids>();

			one.CachedMethod().Should().Not.Be.EqualTo(one.CachedMethod());
		} 
	}
}