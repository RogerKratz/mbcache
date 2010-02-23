using MbCache.Configuration;
using MbCache.Core;
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

            factory = builder.BuildFactory(new TestCacheFactory());
        }

        [Test]
        public void CanReadProps()
        {
            var obj = factory.Create<ObjectWithCtorParameters>();
            Assert.AreEqual(1, obj.Value1);
            Assert.AreEqual(2, obj.Value2);
        }

        [Test]
        public void VerifyCacheWorks()
        {
            Assert.AreEqual(factory.Create<ObjectWithCtorParameters>().CachedMethod(), factory.Create<ObjectWithCtorParameters>().CachedMethod());
        }
    }
}
