using System;
using MbCache.Configuration;
using MbCacheTest.TestCache;
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
            builder.UseCacheFor<ObjectWithNonPublicCtor>(c => c.Calculate());

            var factory = builder.BuildFactory(new TestCacheFactory());

            Assert.IsInstanceOf<ObjectWithNonPublicCtor>(factory.Create<ObjectWithNonPublicCtor>());
        }

        [Test]
        public void MethodMustBeVirtual()
        {
            var builder = new CacheBuilder();
            builder.UseCacheFor<ClassWithNonVirtualMethod>(c => c.Calculate());

            var factory = builder.BuildFactory(new TestCacheFactory());
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