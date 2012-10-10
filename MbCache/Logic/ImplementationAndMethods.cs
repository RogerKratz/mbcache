using System;
using System.Collections.Generic;
using System.Reflection;

namespace MbCache.Logic
{
	[Serializable]
	public class ImplementationAndMethods
	{
		public ImplementationAndMethods(Type concreteType)
		{
			ConcreteType = concreteType;
			Methods = new HashSet<MethodInfo>();
		}

		public Type ConcreteType { get; private set; }
		public ICollection<MethodInfo> Methods { get; private set; }
		public bool CachePerInstance { get; set; }
	}
}