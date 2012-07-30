using System;
using System.Reflection;

namespace MbCache.Core.Events
{
	public class DeleteInfo
	{
		public DeleteInfo(string cacheKeyStartsWith, Type type, MethodInfo method, object[] arguments)
		{
			CacheKeyStartsWith = cacheKeyStartsWith;
			Type = type;
			Method = method;
			Arguments = arguments;
		}

		public string CacheKeyStartsWith { get; private set; }
		public Type Type { get; private set; }
		public MethodInfo Method { get; private set; }
		public object[] Arguments { get; private set; }
	}
}