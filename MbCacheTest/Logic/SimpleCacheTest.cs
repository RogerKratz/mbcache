using System;
using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.CacheForTest;
using NUnit.Framework;

namespace MbCacheTest.Logic
{
    [TestFixture]
    public class SimpleCacheTest
    {
        private IMbCacheFactory factory;

        [SetUp]
        public void Setup()
        {
            var builder = new CacheBuilder();
            builder.UseCacheForClass<ObjectReturningRandomNumbers>(c => c.CachedMethod());

            factory = builder.BuildFactory(new TestCacheFactory());
        }

        [Test]
        public void CacheWorks()
        {
            var obj = factory.Create<ObjectReturningRandomNumbers>();

            Assert.AreEqual(obj.CachedMethod(), obj.CachedMethod());
            Assert.AreNotEqual(obj.NonCachedMethod(), obj.NonCachedMethod());
        }

        [Test]
        public void CacheWorksWithDifferentObjects()
        {
            var obj = factory.Create<ObjectReturningRandomNumbers>();
            var obj2 = factory.Create<ObjectReturningRandomNumbers>();
            Assert.AreEqual(obj.CachedMethod(), obj2.CachedMethod());
        }

        [Test]
        public void OriginalTypeIsNotModified()
        {
            var obj = new ObjectReturningRandomNumbers();

            Assert.AreNotEqual(obj.CachedMethod(), obj.CachedMethod());
            Assert.AreNotEqual(obj.NonCachedMethod(), obj.NonCachedMethod());
        }


        public class ObjectReturningRandomNumbers
        {
            private readonly Random r = new Random();

            public virtual int CachedMethod()
            {
                return r.Next();
            }

            public virtual int CachedMethod2()
            {
                return r.Next();
            }

            public virtual int NonCachedMethod()
            {
                return r.Next();
            }
        }
    }
}