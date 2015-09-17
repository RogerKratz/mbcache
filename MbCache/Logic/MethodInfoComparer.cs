using System;
using System.Collections.Generic;
using System.Reflection;

namespace MbCache.Logic
{
	public class MethodInfoComparer : IEqualityComparer<MethodInfo>
	{
		public static readonly IEqualityComparer<MethodInfo> Instance = new MethodInfoComparer();

		private MethodInfoComparer(){}

		public bool Equals(MethodInfo x, MethodInfo y)
		{
			//no need to check what type methodinfo is on - already checked before entering this method

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

		public int GetHashCode(MethodInfo obj)
		{
			throw new NotImplementedException("Not currently used by MbCache");
		}
	}
}