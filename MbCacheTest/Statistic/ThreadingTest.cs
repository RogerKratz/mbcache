using System.Threading;
using System.Collections.Generic;
using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Statistic
{
    [TestFixture]
    public class ThreadingTest
    {
        private IObjectReturningNewGuids component;
        private const int noOfThreads = 100;
        private IMbCacheFactory factory;

        [SetUp]
        public void Setup()
        {
            var builder = new CacheBuilder(ConfigurationData.ProxyFactory, new NoCache(), new ToStringMbCacheKey());

            builder
                .For<ObjectReturningNewGuids>()
                .CacheMethod(c => c.CachedMethod())
                .As<IObjectReturningNewGuids>();

            factory = builder.BuildFactory();
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

            Assert.AreEqual(noOfThreads, factory.Statistics.CacheMisses);
            factory.Statistics.Clear();
        }

        private void createCacheHitOrMiss()
        {
            component.CachedMethod();
        }
    }
}