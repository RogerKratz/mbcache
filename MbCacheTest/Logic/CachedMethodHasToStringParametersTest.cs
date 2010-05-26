using MbCache.Configuration;
using MbCache.Core;
using MbCache.DefaultImpl;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic
{
    public class CachedMethodHasToStringParametersTest
    {
        private IMbCacheFactory factory;

        [SetUp]
        public void Setup()
        {
            var builder = new CacheBuilder();

            builder.ForClass<ObjectWithParametersOnCachedMethod>()
                .CacheMethod(c => c.CachedMethod(null));
            builder.ForInterface<IObjectWithParametersOnCachedMethod, ObjectWithParametersOnCachedMethod>()
                .CacheMethod(c => c.CachedMethod(null));

            factory = builder.BuildFactory(new TestCacheFactory(), new ToStringMbCacheKey());
        }

        [Test]
        public void Class_VerifySameParameterGivesCacheHit()
        {
            Assert.AreEqual(factory.Create<ObjectWithParametersOnCachedMethod>().CachedMethod("hej"), 
                            factory.Create<ObjectWithParametersOnCachedMethod>().CachedMethod("hej"));
        }

        [Test]
        public void Interface_VerifySameParameterGivesCacheHit()
        {
            Assert.AreEqual(factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("hej"),
                            factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("hej"));
        }

        [Test]
        public void Class_VerifyDifferentParameterGivesNoCacheHit()
        {
            Assert.AreNotEqual(factory.Create<ObjectWithParametersOnCachedMethod>().CachedMethod("roger"),
                            factory.Create<ObjectWithParametersOnCachedMethod>().CachedMethod("moore"));
        }

        [Test]
        public void Interface_VerifyDifferentParameterGivesNoCacheHit()
        {
            Assert.AreNotEqual(factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("roger"),
                            factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("moore"));
        }

        [Test]
        public void Class_NullAsParameter()
        {
            Assert.AreNotEqual(factory.Create<ObjectWithParametersOnCachedMethod>().CachedMethod(null),
                factory.Create<ObjectWithParametersOnCachedMethod>().CachedMethod("moore"));
            Assert.AreEqual(factory.Create<ObjectWithParametersOnCachedMethod>().CachedMethod(null),
                factory.Create<ObjectWithParametersOnCachedMethod>().CachedMethod(null));
        }

        [Test]
        public void Interface_NullAsParameter()
        {
            Assert.AreNotEqual(factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod(null),
                factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod("moore"));
            Assert.AreEqual(factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod(null),
                factory.Create<IObjectWithParametersOnCachedMethod>().CachedMethod(null));
        }


        [Test]
        public void Class_InvalidateOnTypeWorks()
        {
            var obj = factory.Create<ObjectWithParametersOnCachedMethod>();
            var value = obj.CachedMethod("hej");
            factory.Invalidate<ObjectWithParametersOnCachedMethod>();
            Assert.AreNotEqual(value, obj.CachedMethod("hej"));
        }

        [Test]
        public void Interface_InvalidateOnTypeWorks()
        {
            var obj = factory.Create<IObjectWithParametersOnCachedMethod>();
            var value = obj.CachedMethod("hej");
            factory.Invalidate<IObjectWithParametersOnCachedMethod>();
            Assert.AreNotEqual(value, obj.CachedMethod("hej"));
        }

        [Test]
        public void Class_InvalidateOnMethodWorks()
        {
            var obj = factory.Create<ObjectWithParametersOnCachedMethod>();
            var value = obj.CachedMethod("hej");
            var value2 = obj.CachedMethod("hej2");
            factory.Invalidate<ObjectWithParametersOnCachedMethod>(c=>c.CachedMethod("hej"));
            Assert.AreNotEqual(value, obj.CachedMethod("hej"));
            Assert.AreNotEqual(value2, obj.CachedMethod("hej2")); //todo: fix this later?
        }

        [Test]
        public void Interface_InvalidateOnMethodWorks()
        {
            var obj = factory.Create<IObjectWithParametersOnCachedMethod>();
            var value = obj.CachedMethod("hej");
            var value2 = obj.CachedMethod("hej2");
            factory.Invalidate<IObjectWithParametersOnCachedMethod>(c => c.CachedMethod("hej"));
            Assert.AreNotEqual(value, obj.CachedMethod("hej"));
            Assert.AreNotEqual(value2, obj.CachedMethod("hej2")); //todo: fix this later?
        }
    }
}