using System;
using System.Reflection;

namespace MbCache.Core.Events
{
	public class EventInformation
	{
		public EventInformation(string cacheKeyStartsWith, Type type, MethodInfo method, object[] arguments)
		{
			CacheKey = cacheKeyStartsWith;
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