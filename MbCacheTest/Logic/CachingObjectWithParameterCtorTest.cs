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
            builder.UseCacheForClass<ObjectWithCtorParameters>(c => c.CachedMethod(), new object[]{1,2});
            builder.UseCacheForInterface<IObjectWithCtorParameters>(new ObjectWithCtorParameters(1, 2),
                                                                    c => c.CachedMethod());

            factory = builder.BuildFactory(new TestCacheFactory(), new ToStringMbCacheRegion());
        }

        [Test]
        public void Class_CanReadProps()
        {
            var obj = factory.Create<ObjectWithCtorParameters>();
            Assert.AreEqual(1, obj.Value1);
            Assert.AreEqual(2, obj.Value2);
        }

        [Test]
        public void Interface_CanReadProps()
        {
            var obj = factory.Create<IObjectWithCtorParameters>();
            Assert.AreEqual(1, obj.Value1);
            Assert.AreEqual(2, obj.Value2);
        }

        [Test]
        public void Class_VerifyCacheWorks()
        {
            Assert.AreEqual(factory.Create<ObjectWithCtorParameters>().CachedMethod(), factory.Create<ObjectWithCtorParameters>().CachedMethod());
        }

        [Test]
        public void Interface_VerifyCacheWorks()
        {
            Assert.AreEqual(factory.Create<IObjectWithCtorParameters>().CachedMethod(), factory.Create<IObjectWithCtorParameters>().CachedMethod());
        }
    }
}
