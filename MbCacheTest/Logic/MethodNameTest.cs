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
            builder.UseCacheForInterface<IA>(new A(), c => c.DoIt());
            builder.UseCacheForInterface<IB>(new B(), c => c.DoIt());

            factory = builder.BuildFactory(new TestCacheFactory());
        }

        [Test]
        public void ClassMethodNamesShouldBeUniquePerType()
        {
            var valueA = factory.Create<A>().DoIt();
            var valueB = factory.Create<B>().DoIt();
            Assert.AreNotEqual(valueA, valueB);
            factory.Invalidate<A>();
            Assert.AreNotEqual(valueA, factory.Create<A>().DoIt());
            Assert.AreEqual(valueB, factory.Create<B>().DoIt());
        }

        [Test]
        public void InterfaceMethodNamesShouldBeUniquePerType()
        {
            var valueA = factory.Create<IA>().DoIt();
            var valueB = factory.Create<IB>().DoIt();
            Assert.AreNotEqual(valueA, valueB);
            factory.Invalidate<IA>();
            Assert.AreNotEqual(valueA, factory.Create<IA>().DoIt());
            Assert.AreEqual(valueB, factory.Create<IB>().DoIt());
        }

    }

    public interface IA
    {
        int DoIt();
    }

    public class A : IA
    {
        private static int value = 1000;
        public virtual int DoIt()
        {
            return value++;   
        }
    }

    public interface IB
    {
        int DoIt();
    }

    public class B : IB
    {
        private static int value;
        public virtual int DoIt()
        {
            return value++;
        }        
    }
}