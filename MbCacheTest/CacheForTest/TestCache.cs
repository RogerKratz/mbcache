using System.Collections.Generic;
using System.Linq;
using MbCache.Configuration;

namespace MbCacheTest.CacheForTest
{
	public class TestCache : ICache
	{
		private readonly IDictionary<string, object> _values;

		public TestCache()
		{
			_values = new Dictionary<string, object>();
		}

		public object Get(string key)
		{
			object ret;
			if (_values.TryGetValue(key, out ret))
				return ret;
			return null;
		}

		public void Put(string key, object value)
		{
			_values[key] = value;
		}

		public void Delete(string keyStartingWith)
		{
			var keysToRemove = _values.Keys.Where(key => key.StartsWith(keyStartingWith)).ToList();
			foreach (var key in keysToRemove)
			{
				_values.Remove(key);
			}
		}
	}
}