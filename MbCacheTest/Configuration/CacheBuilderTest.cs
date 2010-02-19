using MbCache.Configuration;
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

            var factory = builder.BuildFactory();

            Assert.IsInstanceOf<ObjectWithNonPublicCtor>(factory.Get<ObjectWithNonPublicCtor>());
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