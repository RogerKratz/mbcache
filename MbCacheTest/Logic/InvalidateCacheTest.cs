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
    [TestFixture]
    public class InvalidateCacheTest
    {
        private IMbCacheFactory factory;

        [SetUp]
        public void Setup()
        {
            var builder = new CacheBuilder(ConfigurationData.ProxyFactory, new TestCache(), new ToStringMbCacheKey());

            builder
                .For<ObjectReturningNewGuids>()
                .CacheMethod(c => c.CachedMethod())
                .CacheMethod(c => c.CachedMethod2())
                .As<IObjectReturningNewGuids>();

            factory = builder.BuildFactory();
        }


        [Test]
        public void VerifyInvalidateByType()
        {
            var obj = factory.Create<IObjectReturningNewGuids>();
            var value1 = obj.CachedMethod();
            var value2 = obj.CachedMethod2();
            Assert.AreEqual(value1, obj.CachedMethod());
            Assert.AreEqual(value2, obj.CachedMethod2());
            Assert.AreNotEqual(value1, value2);
            factory.Invalidate<IObjectReturningNewGuids>();
            Assert.AreNotEqual(value1, obj.CachedMethod());
            Assert.AreNotEqual(value2, obj.CachedMethod2());
        }


        [Test]
        public void VerifyInvalidateByInstance()
        {
            var obj = factory.Create<IObjectReturningNewGuids>();
            var value1 = obj.CachedMethod();
            var value2 = obj.CachedMethod2();
            Assert.AreEqual(value1, obj.CachedMethod());
            Assert.AreEqual(value2, obj.CachedMethod2());
            Assert.AreNotEqual(value1, value2);
            factory.Invalidate(obj);
            Assert.AreNotEqual(value1, obj.CachedMethod());
            Assert.AreNotEqual(value2, obj.CachedMethod2());
        }

        [Test]
        public void InvalidatingNonCachingComponentThrows()
        {
            Assert.Throws<ArgumentException>(() => factory.Invalidate(3));
        }

        [Test]
        public void InvalidatingNonCachingComponentAndMethodThrows()
        {
            Assert.Throws<ArgumentException>(() => factory.Invalidate(3, theInt => theInt.CompareTo(44)));
        }

        [Test]
        public void InvalidateByCachingComponent()
        {
            var obj = factory.Create<IObjectReturningNewGuids>();
            var value = obj.CachedMethod();

            ((ICachingComponent) obj).Invalidate();
            Assert.AreNotEqual(value, obj.CachedMethod());
        }

        [Test]
        public void InvalidateSpecificMethod()
        {
            var obj = factory.Create<IObjectReturningNewGuids>();
            var value1 = obj.CachedMethod();
            var value2 = obj.CachedMethod2();
            Assert.AreEqual(value1, obj.CachedMethod());
            Assert.AreEqual(value2, obj.CachedMethod2());
            Assert.AreNotEqual(value1, value2);
            factory.Invalidate(obj, method => obj.CachedMethod());
            Assert.AreNotEqual(value1, obj.CachedMethod());
            Assert.AreEqual(value2, obj.CachedMethod2());
        }
    }
}
