using System;
using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.Scope
{
	public class CacheKeyForComponentOverridingTest : FullTest
	{
		private IMbCacheFactory factory;


		public CacheKeyForComponentOverridingTest(Type proxyType) : base(proxyType)
		{
		}

		protected override ICacheKey CreateCacheKey()
		{
			return new CacheKeyThatThrows();
		}

		protected override void TestSetup()
		{
			CacheBuilder.For<ReturningRandomNumbers>()
				 .CacheMethod(c => c.CachedNumber())
				 .CacheKey(new ToStringCacheKey())
				 .As<IReturningRandomNumbers>();

			factory = CacheBuilder.BuildFactory();
		}

		[Test]
		public void ShouldHaveDifferentCachesPerScope()
		{
			var instance = factory.Create<IReturningRandomNumbers>();
			instance.CachedNumber()
				.Should().Be.EqualTo(instance.CachedNumber());
		}

	}
}