using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic
{
    public class CachingPerScopeTest
    {
        private IMbCacheFactory factory;

        [SetUp]
        public void Setup()
        {
            var builder = new CacheBuilder(ConfigurationData.ProxyFactory, new TestCache(), new ToStringMbCacheKey());

            builder
                .For<ObjectReturningNewGuids>()
                .CacheMethod(c => c.CachedMethod())
                .PerInstance()
                .As<IObjectReturningNewGuids>();

            factory = builder.BuildFactory();
        }


        [Test]
        public void DifferentObjectsHasTheirOwnCache()
        {
            var obj = factory.Create<IObjectReturningNewGuids>();
            var obj2 = factory.Create<IObjectReturningNewGuids>();

            Assert.AreEqual(obj.CachedMethod(), obj.CachedMethod());
            Assert.AreNotEqual(obj.CachedMethod(), obj2.CachedMethod());
        }
    }
}