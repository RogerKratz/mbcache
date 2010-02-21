using System;
using MbCache.Logic;

namespace MbCacheTest.CacheForTest
{
    public class TestCacheFactory : ICacheFactory
    {
        public ICache Create()
        {
            return new TestCache();
        }
    }
}
