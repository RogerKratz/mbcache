using MbCache.Configuration;
using MbCache.Core;
using MbCache.DefaultImpl;
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
            builder.ForClass<ObjectReturningNewGuids>()
                .CacheMethod(c => c.CachedMethod());

            factory = builder.BuildFactory(new AspNetCacheFactory(1), new ToStringMbCacheKey());
        }

        [Test]
        public void VerifyCache()
        {
            var value = factory.Create<ObjectReturningNewGuids>().CachedMethod();
            Assert.AreEqual(value, factory.Create<ObjectReturningNewGuids>().CachedMethod());
        }

        [Test]
        public void CanInvalidateInvalidatedEntry()
        {
            var value = factory.Create<ObjectReturningNewGuids>().CachedMethod();
            factory.Invalidate<ObjectReturningNewGuids>();
            factory.Invalidate<ObjectReturningNewGuids>();
            Assert.AreNotEqual(value, factory.Create<ObjectReturningNewGuids>().CachedMethod());
        }
    }
}