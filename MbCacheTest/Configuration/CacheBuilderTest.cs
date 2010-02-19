using MbCache.Configuration;
using NUnit.Framework;

namespace MbCacheTest.Configuration
{
    [TestFixture]
    public class CacheBuilderTest
    {
        [Test]
        public void CanUseNonPublicCtorOnCachedObject()
        {
            var builder = new CacheBuilder();
            builder.UseCacheFor<ObjectWithNonPublicCtor>(c => c.Calculate());

            var factory = builder.BuildFactory();

            Assert.IsAssignableFrom<ObjectWithNonPublicCtor>(factory.Get<ObjectWithNonPublicCtor>());
        }

    }


    public class ObjectWithNonPublicCtor 
    {
        private ObjectWithNonPublicCtor()
        {
        }
        public object Calculate()
        {
            return null;
        }
    }
}