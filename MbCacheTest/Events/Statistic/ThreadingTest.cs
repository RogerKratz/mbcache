using System.Threading;
using System.Collections.Generic;
using MbCache.Core;
using MbCache.Core.Events;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Events.Statistic
{
	public class ThreadingTest : FullTest
	{
		private IObjectReturningNewGuids component;
		private const int noOfThreads = 100;
		private IMbCacheFactory factory;
		private StatisticsEventListener eventListener;

		public ThreadingTest(string proxyTypeString) : base(proxyTypeString) { }

		protected override MbCache.Configuration.ICache CreateCache()
		{
			return new NoCache();
		}

		protected override void TestSetup()
		{
			eventListener = new StatisticsEventListener();
			CacheBuilder
				.AddEventListener(eventListener)
				.For<ObjectReturningNewGuids>()
					.CacheMethod(c => c.CachedMethod())
					.As<IObjectReturningNewGuids>();

			factory = CacheBuilder.BuildFactory();
			component = factory.Create<IObjectReturningNewGuids>();
			log4net.LogManager.Shutdown();
		}

		[TearDown]
		public void AfterTest()
		{
			SetupFixtureForAssembly.StartLog4Net();
		}

		[Test]
		public void StatisticConcurrency()
		{
			var ts = new ThreadStart(createCacheHitOrMiss);

			var tColl = new List<Thread>(noOfThreads);
			for (var threadLoop = 0; threadLoop < noOfThreads; threadLoop++)
			{
				tColl.Add(new Thread(ts));
			}
			foreach (var thread in tColl)
			{
				thread.Start();
			}
			for (var i = tColl.Count - 1; i >= 0; i--)
			{
				tColl[i].Join();
			}

			Assert.AreEqual(noOfThreads, eventListener.CacheMisses);
		}

		private void createCacheHitOrMiss()
		{
			component.CachedMethod();
		}
	}
}