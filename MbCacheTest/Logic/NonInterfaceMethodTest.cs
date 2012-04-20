using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic
{
    [TestFixture, Ignore("fix soon")]
    public class NonInterfaceMethodTest
    {
        private IMbCacheFactory factory;

        [SetUp]
        public void Setup()
        {
            var builder = new CacheBuilder(ConfigurationData.ProxyFactory, new TestCache(), new ToStringMbCacheKey());

            builder
                .For<ObjectWithNonInterfaceMethod>()
                .As<IObjectWithNonInterfaceMethod>();

            factory = builder.BuildFactory();
        }

        [Test]
        public void CanAccessNonInterfaceMethod()
        {
            Assert.AreEqual(4, ((ObjectWithNonInterfaceMethod)factory.Create<IObjectWithNonInterfaceMethod>()).ReturnsFour());
        }

    }
}