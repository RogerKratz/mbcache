using System;

namespace MbCacheTest.TestData
{
    public class ObjectWithParametersOnCachedMethod
    {
        private static readonly Random r = new Random();

        public virtual int CachedMethod(object parameter)
        {
            return r.Next();
        }
    }
}
