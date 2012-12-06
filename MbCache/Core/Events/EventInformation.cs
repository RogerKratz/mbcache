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

		public string CacheKey { get; private set; }
		public Type Type { get; private set; }
		public MethodInfo Method { get; private set; }
		public IEnumerable<object> Arguments { get; private set; }

		private string _methodName;
		public string MethodName()
		{
			var method = Method == null ? "[all methods]" : Method.ToString();
			return _methodName ?? (_methodName = string.Format("{0} on {1}", method, Type.Name));
		}
	}
}