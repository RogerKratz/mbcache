using System.Collections.Generic;
using MbCache.Logic;

namespace MbCacheTest.CacheForTest
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

        public void Delete(string keyStartingWith)
        {
            var keysToRemove = new List<string>();
            foreach (var key in values.Keys)
            {
                if (key.StartsWith(keyStartingWith))
                    keysToRemove.Add(key);
            }
            foreach (var key in keysToRemove)
            {
                values.Remove(key);
            }
        }
    }
}