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
        public void OnlyDeclareInterfaceOnce()
        {
            builder.For<IObjectReturningNewGuids>(() => new ObjectReturningNewGuids())
                .CacheMethod(c => c.CachedMethod());
            Assert.Throws<ArgumentException>(()
                                             =>
                                            builder
                                                .For<IObjectReturningNewGuids>(() => new ObjectReturningNewGuids())
                                                .CacheMethod(c => c.CachedMethod2()));

        }

        [Test]
        public void FactoryReturnsNewInterfaceInstances()
        {
            builder
                .For<IObjectWithIdentifier>(() => new ObjectWithIdentifier()); 
            var factory = builder.BuildFactory(new TestCacheFactory(), new ToStringMbCacheKey());
            Assert.AreNotEqual(factory.Create<IObjectWithIdentifier>().Id, factory.Create<IObjectWithIdentifier>().Id);
        }

        [Test]
        public void CannotSkipInterface()
        {
            Assert.Throws<ArgumentException>(() =>
                        builder.For(() => new ObjectWithIdentifier()));
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
}