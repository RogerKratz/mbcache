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

            builder
                .For<IObjectWithCtorParameters>(() => new ObjectWithCtorParameters(11,12))
                .CacheMethod(c => c.CachedMethod());

            factory = builder.BuildFactory(new TestCacheFactory(), new ToStringMbCacheKey());
        }

        [Test]
        public void CanReadProps()
        {
            var obj = factory.Create<IObjectWithCtorParameters>();
            Assert.AreEqual(11, obj.Value1);
            Assert.AreEqual(12, obj.Value2);
        }

        [Test]
        public void VerifyCacheWorks()
        {
            Assert.AreEqual(factory.Create<IObjectWithCtorParameters>().CachedMethod(), factory.Create<IObjectWithCtorParameters>().CachedMethod());
        }
    }
}
