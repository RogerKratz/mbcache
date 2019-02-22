using System;

namespace MbCache.Logic
{
	public class ComponentType
	{
		public ComponentType(Type concreteType, string typeAsCacheKeyString)
		{
			ConcreteType = concreteType;
			TypeAsCacheKeyString = typeAsCacheKeyString ?? concreteType.ToString();
		}

		public Type ConcreteType { get; }
		public string TypeAsCacheKeyString { get; }
	}
}