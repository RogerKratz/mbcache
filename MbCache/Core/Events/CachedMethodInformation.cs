using System.Collections.Generic;
using System.Reflection;

namespace MbCache.Core.Events
{
	public class CachedMethodInformation
	{
		public CachedMethodInformation(MethodInfo method, IEnumerable<object> arguments)
		{
			Method = method;
			Arguments = arguments;
		}

		public MethodInfo Method { get; }
		public IEnumerable<object> Arguments { get; }
	}
}