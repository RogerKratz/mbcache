using System;
using System.Collections.Generic;
using System.Reflection;
using MbCache.Configuration;
using MbCache.Core;
using MbCache.Logic;

namespace MbCacheTest.Logic.Scope
{
	public class CacheKeyThatThrows : ICacheKey
	{
		public string RemoveKey(ComponentType type)
		{
			throw new NotImplementedException();
		}

		public string RemoveKey(ComponentType type, ICachingComponent component)
		{
			throw new NotImplementedException();
		}

		public string RemoveKey(ComponentType type, ICachingComponent component, MethodInfo method)
		{
			throw new NotImplementedException();
		}

		public string RemoveKey(ComponentType type, ICachingComponent component, MethodInfo method, IEnumerable<object> parameters)
		{
			throw new NotImplementedException();
		}

		public KeyAndItsDependingKeys GetAndPutKey(ComponentType type, ICachingComponent component, MethodInfo method, IEnumerable<object> parameters)
		{
			throw new NotImplementedException();
		}
	}
}