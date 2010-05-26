using MbCache.Configuration;
using MbCache.Core;
using MbCache.DefaultImpl;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic
{
    [TestFixture]
    public class CachingObjectWithParameterCtorTest
    {
        private IMbCacheFactory factory;

        [SetUp]
        public void Setup()
        {
            var builder = new CacheBuilder();
            builder.ForClass<ObjectWithCtorParameters>()
                    .CacheMethod(c => c.CachedMethod());
            builder.ForInterface<IObjectWithCtorParameters, ObjectWithCtorParameters>()
                    .CacheMethod(c => c.CachedMethod());

            factory = builder.BuildFactory(new TestCacheFactory(), new ToStringMbCacheKey());
        }

        [Test]
        public void Class_CanReadProps()
        {
            var obj = factory.Create<ObjectWithCtorParameters>(1, 2);
            Assert.AreEqual(1, obj.Value1);
            Assert.AreEqual(2, obj.Value2);
        }

        [Test]
        public void Interface_CanReadProps()
        {
            var obj = factory.Create<IObjectWithCtorParameters>(1, 2);
            Assert.AreEqual(1, obj.Value1);
            Assert.AreEqual(2, obj.Value2);
        }

        [Test]
        public void Class_VerifyCacheWorks()
        {
            Assert.AreEqual(factory.Create<ObjectWithCtorParameters>(1, 2).CachedMethod(), factory.Create<ObjectWithCtorParameters>(1, 2).CachedMethod());
        }

        [Test]
        public void Interface_VerifyCacheWorks()
        {
            Assert.AreEqual(factory.Create<IObjectWithCtorParameters>(1, 2).CachedMethod(), factory.Create<IObjectWithCtorParameters>(1, 2).CachedMethod());
        }
    }
}
