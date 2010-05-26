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
        public virtual Guid CachedMethod()
        {
            return Guid.NewGuid();
        }

        public virtual Guid CachedMethod2()
        {
            return Guid.NewGuid();
        }

        public virtual Guid NonCachedMethod()
        {
            return Guid.NewGuid();
        }
    }
}