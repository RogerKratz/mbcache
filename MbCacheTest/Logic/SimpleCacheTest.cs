using System;
using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.TestObjects;
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
            builder.UseCacheFor<objectToCache>(c => c.CachedMethod());

            factory = builder.BuildFactory(new TestCacheFactory());
        }

        [Test]
        public void CacheWorks()
        {
            var obj = factory.Create<objectToCache>();

            Assert.AreEqual(obj.CachedMethod(), obj.CachedMethod());
            Assert.AreNotEqual(obj.NonCachedMethod(), obj.NonCachedMethod());
        }

        [Test]
        public void OriginalTypeIsNotModified()
        {
            var obj = new objectToCache();

            Assert.AreNotEqual(obj.CachedMethod(), obj.CachedMethod());
            Assert.AreNotEqual(obj.NonCachedMethod(), obj.NonCachedMethod());
        }
    }

    public class objectToCache
    {
        private Random r = new Random();

        public virtual int CachedMethod()
        {
            return r.Next();
        }

        public virtual int NonCachedMethod()
        {
            return r.Next();
        }
    }
}