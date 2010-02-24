using System;

namespace MbCacheTest.TestData
{
    public interface IObjectReturningRandomNumbers
    {
        int CachedMethod();
        int CachedMethod2();
        int NonCachedMethod();
    }

    public class ObjectReturningRandomNumbers : IObjectReturningRandomNumbers
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