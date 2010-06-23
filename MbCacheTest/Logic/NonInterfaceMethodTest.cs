using MbCache.Configuration;
using MbCache.Core;
using MbCache.DefaultImpl;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Logic
{
    [TestFixture]
    public class NonInterfaceMethodTest
    {
        private IMbCacheFactory factory;

        [SetUp]
        public void Setup()
        {
            var builder = new CacheBuilder();

            builder
                .For<ObjectWithNonInterfaceMethod>()
                .As<IObjectWithNonInterfaceMethod>();

            factory = builder.BuildFactory(new TestCacheFactory(), new ToStringMbCacheKey());
        }

        [Test]
        public void CanAccessNonInterfaceMethod()
        {
            Assert.AreEqual(4, ((ObjectWithNonInterfaceMethod)factory.Create<IObjectWithNonInterfaceMethod>()).ReturnsFour());
        }

    }
}