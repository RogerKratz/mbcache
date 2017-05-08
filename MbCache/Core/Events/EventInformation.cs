using System;
using System.Collections.Generic;
using System.Reflection;

namespace MbCache.Core.Events
{
	public class EventInformation
	{
		public EventInformation(string cacheKey, Type type, MethodInfo method, IEnumerable<object> arguments)
		{
			CacheKey = cacheKey;
			Type = type;
			Method = method;
			Arguments = arguments;
		}

		public string CacheKey { get; }
		public Type Type { get; }
		public MethodInfo Method { get; }
		public IEnumerable<object> Arguments { get; }
	}
}