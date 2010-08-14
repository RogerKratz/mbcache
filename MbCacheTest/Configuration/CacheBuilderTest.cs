using System;
using MbCache.Configuration;
using MbCache.DefaultImpl;
using MbCacheTest.CacheForTest;
using MbCacheTest.TestData;
using NUnit.Framework;

namespace MbCacheTest.Configuration
{
    [TestFixture]
    public class CacheBuilderTest
    {
        private CacheBuilder builder;

        [SetUp]        
        public void Setup()
        {
            builder = new CacheBuilder(ConfigurationData.ProxyImpl, new TestCacheFactory(), new ToStringMbCacheKey());
        }

        [Test]
        public void OnlyDeclareTypeOnce()
        {
            builder
                .For<ObjectReturningNewGuids>()
                .CacheMethod(c => c.CachedMethod())
                .As<IObjectReturningNewGuids>();
            Assert.Throws<ArgumentException>(()
                                             =>
                                            builder
                                                .For<ObjectReturningNewGuids>()
                                                .CacheMethod(c => c.CachedMethod2())
                                                .As<IObjectReturningNewGuids>());

        }

        [Test]
        public void ReturnTypeMustBeDeclared()
        {
            builder
                .For<ObjectReturningNewGuids>()
                .CacheMethod(c => c.CachedMethod());
            Assert.Throws<InvalidOperationException>(() => builder.BuildFactory());
        }

        [Test]
        public void CreatingProxiesOfSameDeclaredTypeShouldReturnIdenticalTypes()
        {
            builder
                .For<ObjectReturningNewGuids>()
                .CacheMethod(c => c.CachedMethod())
                .As<IObjectReturningNewGuids>();

            var factory = builder.BuildFactory();

            Assert.AreEqual(factory.Create<IObjectReturningNewGuids>().GetType(), factory.Create<IObjectReturningNewGuids>().GetType());
        }

        [Test]
        public void FactoryReturnsNewInterfaceInstances()
        {
            builder
                .For<ObjectWithIdentifier>()
                .As<IObjectWithIdentifier>();
            var factory = builder.BuildFactory();
            var obj1 = factory.Create<IObjectWithIdentifier>();
            var obj2 = factory.Create<IObjectWithIdentifier>();
            Assert.AreNotSame(obj1, obj2);
            Assert.AreNotEqual(obj1.Id, obj2.Id);
        }

        [Test]
        public void CachedMethodNeedToBeVirtual()
        {
            Assert.Throws<InvalidOperationException>(() => 
            builder
                .For<HasNonVirtualMethod>()
                .CacheMethod(m => m.DoIt())
                ); 
        }
    }

    public class ObjectWithIdentifier : IObjectWithIdentifier
    {
        public ObjectWithIdentifier()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
    }

    public interface IObjectWithIdentifier
    {
        Guid Id { get; }
    }

    public interface IHasNonVirtualMethod
    {
        int DoIt();
    }

    public class HasNonVirtualMethod : IHasNonVirtualMethod
    {
        public int DoIt()
        {
            return 0;
        }
    }
}