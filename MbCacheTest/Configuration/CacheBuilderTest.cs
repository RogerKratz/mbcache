using MbCache.Configuration;
using MbCacheTest.TestObjects;
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

    }


    public class ObjectWithNonPublicCtor 
    {
        protected ObjectWithNonPublicCtor()
        {
        }
        public object Calculate()
        {
            return null;
        }
    }
}