using System.Collections.Generic;
using System.Reflection;

namespace MbCache.Logic;

public class CachedMethod(MethodInfo info, IEnumerable<object> returnValuesNotToCache)
{
	public IEnumerable<object> ReturnValuesNotToCache { get; } = returnValuesNotToCache;

	public bool SameMethodAs(MethodInfo methodInfo)
	{
		var x = methodInfo;
		var y = info;
		if (!x.Name.Equals(y.Name))
			return false;
		var xParams = x.GetParameters();
		var yParams = y.GetParameters();
		var xParamCount = xParams.Length;
		if (xParamCount != yParams.Length)
			return false;
		for (var i = 0; i < xParamCount; i++)
		{
			if (xParams[i].ParameterType != yParams[i].ParameterType)
				return false;
		}
		return true;
	}
}