using MbCache.Configuration;
using MbCache.Core;
using MbCache.DefaultImpl;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic
{
    [TestFixture]
    public class SimpleCacheClassTest
    {
        private IMbCacheFactory factory;

        [SetUp]
        public void Setup()
        {
            var builder = new CacheBuilder();
            builder.ForClass<ObjectReturningRandomNumbers>()
                    .CacheMethod(c => c.CachedMethod());

            factory = builder.BuildFactory(new TestCacheFactory(), new ToStringMbCacheKey());
        }

        [Test]
        public void CacheWorks()
        {
            var obj = factory.Create<ObjectReturningRandomNumbers>();

            Assert.AreEqual(obj.CachedMethod(), obj.CachedMethod());
            Assert.AreNotEqual(obj.NonCachedMethod(), obj.NonCachedMethod());
        }

        [Test]
        public void CacheWorksWithDifferentObjects()
        {
            var obj = factory.Create<ObjectReturningRandomNumbers>();
            var obj2 = factory.Create<ObjectReturningRandomNumbers>();
            Assert.AreEqual(obj.CachedMethod(), obj2.CachedMethod());
        }

        [Test]
        public void OriginalTypeIsNotModified()
        {
            var obj = new ObjectReturningRandomNumbers();

            Assert.AreNotEqual(obj.CachedMethod(), obj.CachedMethod());
            Assert.AreNotEqual(obj.NonCachedMethod(), obj.NonCachedMethod());
        }
    }
}