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
        [Test]
        public void CanUseNonpublicCtorOnCachedObject()
        {
            var builder = new CacheBuilder();
            builder.ForClass<ObjectWithNonPublicCtor>()
                .CacheMethod(c => c.Calculate());

            var factory = builder.BuildFactory(new TestCacheFactory(), new ToStringMbCacheKey());

            Assert.IsInstanceOf<ObjectWithNonPublicCtor>(factory.Create<ObjectWithNonPublicCtor>());
        }

        [Test]
        public void MethodMustBeVirtual()
        {
            var builder = new CacheBuilder();
            builder.ForClass<ClassWithNonVirtualMethod>()
                .CacheMethod(c => c.Calculate());

            var factory = builder.BuildFactory(new TestCacheFactory(), new ToStringMbCacheKey());
            Assert.Throws<ArgumentException>(() => factory.Create<ClassWithNonVirtualMethod>());
        }

        [Test]
        public void OnlyDeclareInterfaceOnce()
        {
            var builder = new CacheBuilder();
            builder.ForInterface<IObjectReturningRandomNumbers>(new ObjectReturningRandomNumbers())
                .CacheMethod(c => c.CachedMethod());
            Assert.Throws<ArgumentException>(()
                                             =>
                                             builder.ForInterface<IObjectReturningRandomNumbers>(new ObjectReturningRandomNumbers())
                                                 .CacheMethod(c => c.CachedMethod2()));

        }

        [Test]
        public void OnlyDeclareClassOnce()
        {
            var builder = new CacheBuilder();
            builder.ForClass<ObjectReturningRandomNumbers>()
                .CacheMethod(c => c.CachedMethod());
            Assert.Throws<ArgumentException>(()
                                             =>
                                             builder.ForClass<ObjectReturningRandomNumbers>()
                                                 .CacheMethod(c => c.CachedMethod2()));

        }

        [Test, Explicit("Fix soon")]
        public void FactoryReturnsNewClassInstances()
        {
            var builder = new CacheBuilder();
            builder.ForClass<ObjectWithIdentifier>();
            var factory = builder.BuildFactory(new TestCacheFactory(), new ToStringMbCacheKey());
            Assert.AreNotEqual(factory.Create<ObjectWithIdentifier>().Id, factory.Create<ObjectWithIdentifier>().Id);
        }

        [Test, Explicit("Fix soon")]
        public void FactoryReturnsNewInterfaceInstances()
        {
            var builder = new CacheBuilder();
            builder.ForInterface<IObjectWithIdentifier>(new ObjectWithIdentifier());
            var factory = builder.BuildFactory(new TestCacheFactory(), new ToStringMbCacheKey());
            Assert.AreNotEqual(factory.Create<IObjectWithIdentifier>().Id, factory.Create<IObjectWithIdentifier>().Id);
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

    public class ObjectWithNonPublicCtor 
    {
        protected ObjectWithNonPublicCtor()
        {
        }
        public virtual object Calculate()
        {
            return null;
        }
    }

    public class ClassWithNonVirtualMethod
    {
        public object Calculate()
        {
            return null;
        }
    }
}