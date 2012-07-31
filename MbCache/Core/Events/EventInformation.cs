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

		private string _methodName;
		public string MethodName()
		{
			var method = Method == null ? "[all methods]" : Method.ToString();
			return _methodName ?? (_methodName = string.Format("{0} on {1}", method, Type.Name));
		}
	}
}