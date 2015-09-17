using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;
using SharpTestsEx;

namespace MbCacheTest.Logic.CacheKeyPerComponent
{
	public class CacheKeyForComponentOverridingTest : FullTest
	{
		private IMbCacheFactory factory;

		public CacheKeyForComponentOverridingTest(string proxyTypeString) : base(proxyTypeString)
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