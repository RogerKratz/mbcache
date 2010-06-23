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
            builder=new CacheBuilder();
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
            Assert.Throws<InvalidOperationException>(() => builder.BuildFactory(new TestCacheFactory(), new ToStringMbCacheKey()));
        }

        [Test]
        public void FactoryReturnsNewInterfaceInstances()
        {
            builder
                .For<ObjectWithIdentifier>()
                .As<IObjectWithIdentifier>(); 
            var factory = builder.BuildFactory(new TestCacheFactory(), new ToStringMbCacheKey());
            Assert.AreNotEqual(factory.Create<IObjectWithIdentifier>().Id, factory.Create<IObjectWithIdentifier>().Id);
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