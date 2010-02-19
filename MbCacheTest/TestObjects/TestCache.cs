using System.Collections.Generic;
using MbCache.Logic;

namespace MbCacheTest.TestObjects
{
    public class TestCache : ICache
    {
        private readonly IDictionary<string, object> values;

        public TestCache()
        {
            values = new Dictionary<string, object>();
        }

        public object Get(string key)
        {
            object ret;
            if (values.TryGetValue(key, out ret))
                return ret;
            return null;
        }

        public void Put(string key, object value)
        {
            values[key] = value;
        }
    }
}