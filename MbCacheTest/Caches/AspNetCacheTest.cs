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
            builder
                .For<ObjectReturningNewGuids>()
                .CacheMethod(c => c.CachedMethod())
                .As<IObjectReturningNewGuids>();

            factory = builder.BuildFactory(new AspNetCacheFactory(1), new ToStringMbCacheKey());
        }

        [Test]
        public void VerifyCache()
        {
            var value = factory.Create<IObjectReturningNewGuids>().CachedMethod();
            Assert.AreEqual(value, factory.Create<IObjectReturningNewGuids>().CachedMethod());
        }

        [Test]
        public void CanInvalidateInvalidatedEntry()
        {
            var value = factory.Create<IObjectReturningNewGuids>().CachedMethod();
            factory.Invalidate<IObjectReturningNewGuids>();
            factory.Invalidate<IObjectReturningNewGuids>();
            Assert.AreNotEqual(value, factory.Create<IObjectReturningNewGuids>().CachedMethod());
        }
    }
}