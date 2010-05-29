using MbCache.Configuration;
using MbCache.Core;
using MbCache.DefaultImpl;
using MbCacheTest.TestData;
using MbCacheTest.CacheForTest;
using NUnit.Framework;

namespace MbCacheTest.Logic
{
    [TestFixture]
    public class SimpleCacheInterfaceTest
    {
        private IMbCacheFactory factory;

        [SetUp]
        public void Setup()
        {
            var builder = new CacheBuilder();

            builder.For<IReturningRandomNumbers>(() => new ReturningRandomNumbers())
                .CacheMethod(c => c.CachedNumber())
                .CacheMethod(c => c.CachedNumber2());

            factory = builder.BuildFactory(new TestCacheFactory(), new ToStringMbCacheKey());
        }

        [Test]
        public void VerifyCacheIsWorking()
        {
            var obj1 = factory.Create<IReturningRandomNumbers>();
            var obj2 = factory.Create<IReturningRandomNumbers>();
            Assert.AreEqual(obj1.CachedNumber(), obj2.CachedNumber());
            Assert.AreEqual(obj1.CachedNumber2(), obj2.CachedNumber2());
            Assert.AreNotEqual(obj1.CachedNumber(), obj1.CachedNumber2());
            Assert.AreNotEqual(obj1.NonCachedNumber(), obj2.NonCachedNumber());
        }
    }
}