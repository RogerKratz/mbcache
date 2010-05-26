using MbCache.Configuration;
using MbCache.Core;
using MbCache.DefaultImpl;
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
            var builder = new CacheBuilder();

            builder.ForClass<ObjectReturningNewGuids>()
                    .CacheMethod(c => c.CachedMethod())
                    .CacheMethod(c => c.CachedMethod2());

            builder.ForInterface<IObjectReturningNewGuids, ObjectReturningNewGuids>()
                    .CacheMethod(c => c.CachedMethod())
                    .CacheMethod(c => c.CachedMethod2());

            factory = builder.BuildFactory(new TestCacheFactory(), new ToStringMbCacheKey());
        }

        [Test]
        public void Class_VerifyInvalidate()
        {
            var obj = factory.Create<ObjectReturningNewGuids>();
            var value1 = obj.CachedMethod();
            var value2 = obj.CachedMethod2();
            Assert.AreEqual(value1, obj.CachedMethod());
            Assert.AreEqual(value2, obj.CachedMethod2());
            Assert.AreNotEqual(value1, value2);
            factory.Invalidate<ObjectReturningNewGuids>();
            Assert.AreNotEqual(value1, obj.CachedMethod());
            Assert.AreNotEqual(value2, obj.CachedMethod2());
        }

        [Test]
        public void Interface_VerifyInvalidate()
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
        public void Class_VerifyInvalidateSpecificMethod()
        {
            var obj = factory.Create<ObjectReturningNewGuids>();
            var value1 = obj.CachedMethod();
            var value2 = obj.CachedMethod2();
            factory.Invalidate<ObjectReturningNewGuids>(c => c.CachedMethod());
            Assert.AreNotEqual(value1, obj.CachedMethod());
            Assert.AreEqual(value2, obj.CachedMethod2());
        }

        [Test]
        public void Interface_VerifyInvalidateSpecificMethod()
        {
            var obj = factory.Create<IObjectReturningNewGuids>();
            var value1 = obj.CachedMethod();
            var value2 = obj.CachedMethod2();
            factory.Invalidate<IObjectReturningNewGuids>(c => c.CachedMethod());
            Assert.AreNotEqual(value1, obj.CachedMethod());
            Assert.AreEqual(value2, obj.CachedMethod2());
        }
    }
}
