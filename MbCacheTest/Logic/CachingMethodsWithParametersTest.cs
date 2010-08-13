using MbCache.Configuration;
using MbCache.Core;
using MbCache.DefaultImpl;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic
{
    public class CachingMethodsWithParametersTest
    {
        private IMbCacheFactory factory;

        [SetUp]
        public void Setup()
        {
            var builder = new CacheBuilder();

            builder
                .For<ObjectWithParametersOnCachedMethod>()
                .CacheMethod(c => c.CachedMethod(null))
                .As<IObjectWithParametersOnCachedMethod>();

            factory = builder.BuildFactory(ConfigurationData.ProxyImpl, new TestCacheFactory(), new ToStringMbCacheKey());
        }

        [Test]
        public void VerifySameParameterGivesCacheHit()
        {
            Assert.AreEqual(factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("hej"),
                            factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("hej"));
        }

        [Test]
        public void VerifyDifferentParameterGivesNoCacheHit()
        {
            Assert.AreNotEqual(factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("roger"),
                            factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("moore"));
        }

        [Test]
        public void NullAsParameter()
        {
            Assert.AreNotEqual(factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod(null),
                factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("moore"));
            Assert.AreEqual(factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod(null),
                factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod(null));
        }

        [Test]
        public void InvalidateOnTypeWorks()
        {
            var obj = factory.Create<IObjectWithParametersOnCachedMethod>();
            var value = obj.CachedMethod("hej");
            factory.Invalidate<IObjectWithParametersOnCachedMethod>();
            Assert.AreNotEqual(value, obj.CachedMethod("hej"));
        }

        [Test]
        public void InvalidateOnMethodWorks()
        {
            var obj = factory.Create<IObjectWithParametersOnCachedMethod>();
            var value = obj.CachedMethod("hej");
            var value2 = obj.CachedMethod("hej2");
            factory.Invalidate<IObjectWithParametersOnCachedMethod>(c => c.CachedMethod("hej"));
            Assert.AreNotEqual(value, obj.CachedMethod("hej"));
            Assert.AreNotEqual(value2, obj.CachedMethod("hej2")); //todo: fix this later
        }
    }
}