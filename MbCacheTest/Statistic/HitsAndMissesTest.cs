using MbCache.Core;
using MbCache.Core.Events;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Statistic
{
	public class HitsAndMissesTest : FullTest
	{
		private IMbCacheFactory factory;
		private StatisticsEventListener eventListener;

		public HitsAndMissesTest(string proxyTypeString) : base(proxyTypeString) {}

		protected override void TestSetup()
		{
			eventListener = new StatisticsEventListener();
			CacheBuilder
				 .For<ObjectReturningNewGuids>()
				 .CacheMethod(c => c.CachedMethod())
				 .As<IObjectReturningNewGuids>();

			factory = CacheBuilder.BuildFactory(eventListener);
		}

		[Test]
		public void VerifyCacheHits()
		{
			var comp = factory.Create<IObjectReturningNewGuids>();
			comp.CachedMethod();
			Assert.AreEqual(0, eventListener.CacheHits);
			comp.CachedMethod();
			Assert.AreEqual(1, eventListener.CacheHits);
			comp.CachedMethod2();
			Assert.AreEqual(1, eventListener.CacheHits);
		}

		[Test]
		public void VerifyCacheMisses()
		{
			var comp = factory.Create<IObjectReturningNewGuids>();
			comp.CachedMethod();
			Assert.AreEqual(1, eventListener.CacheMisses);
			comp.CachedMethod();
			Assert.AreEqual(1, eventListener.CacheMisses);
			comp.CachedMethod2();
			Assert.AreEqual(1, eventListener.CacheMisses);
		}
	}
}