using System;
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
                .For<ObjectWithCtorParameters>()
                .CacheMethod(c => c.CachedMethod())
                .As<IObjectWithCtorParameters>();

            factory = builder.BuildFactory(ConfigurationData.ProxyImpl, new TestCacheFactory(), new ToStringMbCacheKey());
        }

        [Test]
        public void CanReadProps()
        {
            var obj = factory.Create<IObjectWithCtorParameters>(11, 12);
            Assert.AreEqual(11, obj.Value1);
            Assert.AreEqual(12, obj.Value2);
        }

        [Test]
        public void InvalidParametersThrows()
        {
            Assert.Throws<ArgumentException>(() => factory.Create<IObjectWithCtorParameters>());
        }

        [Test]
        public void VerifyCacheWorks()
        {
            Assert.AreEqual(factory.Create<IObjectWithCtorParameters>(4, 3).CachedMethod(),
                            factory.Create<IObjectWithCtorParameters>(4, 3).CachedMethod());
        }

        [Test]
        public void VerifyCacheDifferentParameters()
        {
            Assert.AreEqual(factory.Create<IObjectWithCtorParameters>(4, 4).CachedMethod(),
                            factory.Create<IObjectWithCtorParameters>(4, 3).CachedMethod());
        }
    }
}
