using System;
using MbCache.Logic;

namespace MbCacheTest.TestObjects
{
    public class TestCacheFactory : ICacheFactory
    {
        public ICache Create()
        {
            return new TestCache();
        }
    }
}