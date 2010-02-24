using System;

namespace MbCacheTest.TestData
{
    public interface IObjectWithParametersOnCachedMethod
    {
        int CachedMethod(object parameter);
    }

    public class ObjectWithParametersOnCachedMethod : IObjectWithParametersOnCachedMethod
    {
        private static readonly Random r = new Random();

        public virtual int CachedMethod(object parameter)
        {
            return r.Next();
        }
    }
}
