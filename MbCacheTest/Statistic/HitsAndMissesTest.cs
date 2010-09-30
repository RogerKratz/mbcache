using MbCache.Configuration;
using MbCache.Core;
using MbCache.DefaultImpl;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Statistic
{
    public class HitsAndMissesTest
    {
        private IMbCacheFactory factory;

        [SetUp]
        public void Setup()
        {
            var builder = new CacheBuilder(ConfigurationData.ProxyFactory, new TestCache(), new ToStringMbCacheKey());

            builder
                .For<ObjectReturningNewGuids>()
                .CacheMethod(c => c.CachedMethod())
                .As<IObjectReturningNewGuids>();

            factory = builder.BuildFactory();
            factory.Statistics.Clear();
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
        }

    }
}