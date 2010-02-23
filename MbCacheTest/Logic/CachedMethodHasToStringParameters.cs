using MbCache.Caches;
using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic
{
    public class CachedMethodHasToStringParameters
    {
        private IMbCacheFactory factory;

        [SetUp]
        public void Setup()
        {
            var builder = new CacheBuilder();
            builder.UseCacheForClass<ObjectWithParametersOnCachedMethod>(c => c.CachedMethod(null));

            factory = builder.BuildFactory(new AspNetCacheFactory(1));
        }

        [Test]
        public void VerifySameParameterGivesCacheHit()
        {
            Assert.AreEqual(factory.Create<ObjectWithParametersOnCachedMethod>().CachedMethod("hej"), 
                            factory.Create<ObjectWithParametersOnCachedMethod>().CachedMethod("hej"));
        }

        [Test]
        public void VerifyDifferentParameterGivesNoCacheHit()
        {
            Assert.AreNotEqual(factory.Create<ObjectWithParametersOnCachedMethod>().CachedMethod("roger"),
                            factory.Create<ObjectWithParametersOnCachedMethod>().CachedMethod("moore"));
        }

        [Test]
        public void InvalidateOnTypeWorks()
        {
            var obj = factory.Create<ObjectWithParametersOnCachedMethod>();
            var value = obj.CachedMethod("hej");
            factory.Invalidate<ObjectWithParametersOnCachedMethod>();
            Assert.AreNotEqual(value, obj.CachedMethod("hej"));
        }

        [Test]
        public void InvalidateOnMethodWorks()
        {
            var obj = factory.Create<ObjectWithParametersOnCachedMethod>();
            var value = obj.CachedMethod("hej");
            var value2 = obj.CachedMethod("hej2");
            factory.Invalidate<ObjectWithParametersOnCachedMethod>(c=>c.CachedMethod("hej"));
            Assert.AreNotEqual(value, obj.CachedMethod("hej"));
            Assert.AreNotEqual(value2, obj.CachedMethod("hej2")); //maybe fix this later?
        }
    }
}