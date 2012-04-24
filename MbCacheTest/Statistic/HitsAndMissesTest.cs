using MbCache.Core;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Statistic
{
	public class HitsAndMissesTest : TestBothProxyFactories
	{
		private IMbCacheFactory factory;

		public HitsAndMissesTest(string proxyTypeString) : base(proxyTypeString) {}

		protected override void TestSetup()
		{
			CacheBuilder
				 .For<ObjectReturningNewGuids>()
				 .CacheMethod(c => c.CachedMethod())
				 .As<IObjectReturningNewGuids>();

			factory = CacheBuilder.BuildFactory();
		}

		[Test]
		public void VerifyCacheHits()
		{
			var comp = factory.Create<IObjectReturningNewGuids>();
			comp.CachedMethod();
			Assert.AreEqual(0, factory.Statistics.CacheHits);
			comp.CachedMethod();
			Assert.AreEqual(1, factory.Statistics.CacheHits);
			comp.CachedMethod2();
			Assert.AreEqual(1, factory.Statistics.CacheHits);
			factory.Statistics.Clear();
			Assert.AreEqual(0, factory.Statistics.CacheHits);
		}

		[Test]
		public void VerifyCacheMisses()
		{
			var comp = factory.Create<IObjectReturningNewGuids>();
			comp.CachedMethod();
			Assert.AreEqual(1, factory.Statistics.CacheMisses);
			comp.CachedMethod();
			Assert.AreEqual(1, factory.Statistics.CacheMisses);
			comp.CachedMethod2();
			Assert.AreEqual(1, factory.Statistics.CacheMisses);
			factory.Statistics.Clear();
			Assert.AreEqual(0, factory.Statistics.CacheMisses);
		}
	}
}