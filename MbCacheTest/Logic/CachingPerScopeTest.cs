using System;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.DefaultImpl;
using MbCache.Logic;
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
            var builder = new CacheBuilder();
            builder.ForClass<ObjectReturningNewGuids>()
                    .CacheMethod(c => c.CachedMethod())
                    .PerInstance();
            builder.ForInterface<IObjectReturningNewGuids, ObjectReturningNewGuids>()
                    .CacheMethod(c => c.CachedMethod())
                    .PerInstance();

            factory = builder.BuildFactory(new TestCacheFactory(), new ToStringMbCacheKey());
        }

        [Test]
        public void Class_DifferentObjectsHasTheirOwnCache()
        {
            var obj = factory.Create<ObjectReturningNewGuids>();
            var obj2 = factory.Create<ObjectReturningNewGuids>();

            Assert.AreEqual(obj.CachedMethod(), obj.CachedMethod());
            Assert.AreNotEqual(obj.CachedMethod(), obj2.CachedMethod());
        }

        [Test]
        public void Interface_DifferentObjectsHasTheirOwnCache()
        {
            var obj = factory.Create<IObjectReturningNewGuids>();
            var obj2 = factory.Create<IObjectReturningNewGuids>();

            Assert.AreEqual(obj.CachedMethod(), obj.CachedMethod());
            Assert.AreNotEqual(obj.CachedMethod(), obj2.CachedMethod());
        }
    }
}