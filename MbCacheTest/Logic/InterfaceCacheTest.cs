using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.CacheForTest;
using NUnit.Framework;

namespace MbCacheTest.Logic
{
    [TestFixture]
    public class InterfaceCacheTest
    {
        private IMbCacheFactory factory;

        [SetUp]
        public void Setup()
        {
            var builder = new CacheBuilder();
            builder.UseCacheForInterface<IObjectReturningNumbers, ObjectReturningNumbers>(c => c.CachedMethod());

            factory = builder.BuildFactory(new TestCacheFactory());
        }

        [Test]
        public void CacheWorks()
        {
            var obj = factory.Create<IObjectReturningNumbers>();

            Assert.AreEqual(obj.CachedMethod(), obj.CachedMethod());
            Assert.AreNotEqual(obj.NonCachedMethod(), obj.NonCachedMethod());
        }

        [Test]
        public void OriginalTypeIsNotModified()
        {
            var obj = new ObjectReturningNumbers();

            Assert.AreNotEqual(obj.CachedMethod(), obj.CachedMethod());
            Assert.AreNotEqual(obj.NonCachedMethod(), obj.NonCachedMethod());
        }
    }

    public interface IObjectReturningNumbers
    {
        int CachedMethod();
        int NonCachedMethod();
    }

    public class ObjectReturningNumbers : IObjectReturningNumbers
    {
        private int value = 0;

        public int CachedMethod()
        {
            return value++;
        }

        public int NonCachedMethod()
        {
            return value++;
        }
    }
}
