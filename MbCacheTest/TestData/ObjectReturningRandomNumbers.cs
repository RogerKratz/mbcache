using System;

namespace MbCacheTest.TestData
{
    public class ObjectReturningRandomNumbers
    {
        private readonly Random r = new Random();

        public virtual int CachedMethod()
        {
            return r.Next();
        }

        public virtual int CachedMethod2()
        {
            return r.Next();
        }

        public virtual int NonCachedMethod()
        {
            return r.Next();
        }
    }
}