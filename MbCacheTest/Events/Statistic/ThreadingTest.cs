using System;
using System.Threading;
using System.Collections.Generic;
using MbCache.Core;
using MbCache.Core.Events;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Events.Statistic
{
	public class ThreadingTest : FullTest
	{
		private IObjectWithParametersOnCachedMethod component;
		private const int noOfThreads = 100;
		private IMbCacheFactory factory;
		private StatisticsEventListener eventListener;

		public ThreadingTest(Type proxyType) : base(proxyType)
		{
		}

		protected override void TestSetup()
		{
			eventListener = new StatisticsEventListener();
			CacheBuilder
				.AddEventListener(eventListener)
				.For<ObjectWithParametersOnCachedMethod>()
					.CacheMethod(c => c.CachedMethod(null))
					.As<IObjectWithParametersOnCachedMethod>();

			factory = CacheBuilder.BuildFactory();
			component = factory.Create<IObjectWithParametersOnCachedMethod>();
		}

		[Test]
		public void StatisticConcurrency()
		{
			var tColl = new List<Thread>(noOfThreads);
			for (var threadLoop = 0; threadLoop < noOfThreads; threadLoop++)
			{
				var param = threadLoop;
				var ts = new ThreadStart(() => createCacheHitOrMiss(param));
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

		private void createCacheHitOrMiss(int param)
		{
			component.CachedMethod(param);
		}
	}
}