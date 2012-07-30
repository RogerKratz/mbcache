using System;
using System.Reflection;

namespace MbCache.Core.Events
{
	public class PutInfo
	{
		public PutInfo(string cacheKey, Type type, MethodInfo method, object[] arguments)
		{
			CacheKey = cacheKey;
			Type = type;
			Method = method;
			Arguments = arguments;
		}

		public string CacheKey { get; private set; }
		public Type Type { get; private set; }
		public MethodInfo Method { get; private set; }
		public object[] Arguments { get; private set; }
	}
}