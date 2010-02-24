using MbCache.Caches;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Logic;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Caches
{
    [TestFixture]
    public class AspNetCacheTest
    {
        private IMbCacheFactory factory;

        [SetUp]
        public void Setup()
        {
            var builder = new CacheBuilder();
            builder.UseCacheForClass<ObjectReturningRandomNumbers>(c => c.CachedMethod());

            factory = builder.BuildFactory(new AspNetCacheFactory(1), new DefaultMbCacheRegion());
        }

        [Test]
        public void VerifyCache()
        {
            var value = factory.Create<ObjectReturningRandomNumbers>().CachedMethod();
            Assert.AreEqual(value, factory.Create<ObjectReturningRandomNumbers>().CachedMethod());
        }

        [Test]
        public void CanInvalidateInvalidatedEntry()
        {
            var value = factory.Create<ObjectReturningRandomNumbers>().CachedMethod();
            factory.Invalidate<ObjectReturningRandomNumbers>();
            factory.Invalidate<ObjectReturningRandomNumbers>();
            Assert.AreNotEqual(value, factory.Create<ObjectReturningRandomNumbers>().CachedMethod());
        }
    }
}