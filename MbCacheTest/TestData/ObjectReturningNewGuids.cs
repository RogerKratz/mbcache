using System;

namespace MbCacheTest.TestData
{
    public interface IObjectReturningNewGuids
    {
        Guid CachedMethod();
        Guid CachedMethod2();
        Guid NonCachedMethod();
    }

    public class ObjectReturningNewGuids : IObjectReturningNewGuids
    {

        public Guid CachedMethod()
        {
            return Guid.NewGuid();
        }

        public Guid CachedMethod2()
        {
            return Guid.NewGuid();
        }

        public Guid NonCachedMethod()
        {
            return Guid.NewGuid();
        }
    }
}