using System;
using MbCache.Configuration;
using MbCache.DefaultImpl;
using MbCacheTest.CacheForTest;
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
            builder.UseCacheForClass<ObjectWithNonPublicCtor>(c => c.Calculate());

            var factory = builder.BuildFactory(new TestCacheFactory(), new ToStringMbCacheRegion());

            Assert.IsInstanceOf<ObjectWithNonPublicCtor>(factory.Create<ObjectWithNonPublicCtor>());
        }

        [Test]
        public void MethodMustBeVirtual()
        {
            var builder = new CacheBuilder();
            builder.UseCacheForClass<ClassWithNonVirtualMethod>(c => c.Calculate());

            var factory = builder.BuildFactory(new TestCacheFactory(), new ToStringMbCacheRegion());
            Assert.Throws<ArgumentException>(() => factory.Create<ClassWithNonVirtualMethod>());
        }

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