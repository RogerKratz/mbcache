using MbCache.Configuration;
using MbCache.Core;
using MbCacheTest.CacheForTest;
using NUnit.Framework;

namespace MbCacheTest.Logic
{
    [TestFixture]
    public class MethodNameTest
    {
        private IMbCacheFactory factory;

        [SetUp]
        public void Setup()
        {
            var builder = new CacheBuilder();
            builder.UseCacheForClass<A>(c => c.DoIt());
            builder.UseCacheForClass<B>(c => c.DoIt());

            factory = builder.BuildFactory(new TestCacheFactory());
        }

        [Test]
        public void MethodNamesShouldBeUniquePerType()
        {
            var valueA = factory.Create<A>().DoIt();
            var valueB = factory.Create<B>().DoIt();
            Assert.AreNotEqual(valueA, valueB);
            factory.Invalidate<A>();
            Assert.AreNotEqual(valueA, factory.Create<A>().DoIt());
            Assert.AreEqual(valueB, factory.Create<B>().DoIt());
        }
    }

    public class A
    {
        private static int value = 1000;
        public virtual int DoIt()
        {
            return value++;   
        }
    }

    public class B
    {
        private static int value;
        public virtual int DoIt()
        {
            return value++;
        }        
    }
}