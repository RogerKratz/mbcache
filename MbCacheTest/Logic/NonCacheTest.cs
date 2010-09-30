using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic
{
    [TestFixture]
    public class NonCacheTest
    {
        private IMbCacheFactory factory;

        [SetUp]
        public void Setup()
        {
            var builder = new CacheBuilder(ConfigurationData.ProxyFactory, new TestCache(), new ToStringMbCacheKey());

            builder
                .For<ObjectReturningNewGuids>()
                .CacheMethod(c => c.CachedMethod())
                .CacheMethod(c => c.CachedMethod2())
                .As<IObjectReturningNewGuids>();

            factory = builder.BuildFactory();
        }

        [Test]
        public void CanAskFactoryIfComponentIsKnownType()
        {
            Assert.IsTrue(factory.IsKnownInstance(factory.Create<IObjectReturningNewGuids>()));
            Assert.IsFalse(factory.IsKnownInstance(new object()));
        }
    }
}