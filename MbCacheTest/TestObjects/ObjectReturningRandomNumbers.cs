using System;

namespace MbCacheTest.TestObjects
{
    public class ObjectReturningRandomNumbers
    {
        private readonly Random r = new Random();

        public virtual int CachedMethod()
        {
            return r.Next();
        }

        public virtual int NonCachedMethod()
        {
            return r.Next();
        }
    }
}