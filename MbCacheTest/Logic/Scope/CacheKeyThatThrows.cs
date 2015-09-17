using System.Collections.Generic;
using System.Reflection;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Logic;

namespace MbCacheTest.Logic.CacheKeyPerComponent
{
	public class CacheKeyThatThrows : ICacheKey
	{
		public IEnumerable<string> UnwrapKey(string key)
		{
			throw new System.NotImplementedException();
		}

		public string Key(ComponentType type)
		{
			throw new System.NotImplementedException();
		}

		public string Key(ComponentType type, ICachingComponent component)
		{
			throw new System.NotImplementedException();
		}

		public string Key(ComponentType type, ICachingComponent component, MethodInfo method)
		{
			throw new System.NotImplementedException();
		}

		public string Key(ComponentType type, ICachingComponent component, MethodInfo method, IEnumerable<object> parameters)
		{
			throw new System.NotImplementedException();
		}
	}
}