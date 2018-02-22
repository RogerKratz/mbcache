using System;

namespace MbCache.Logic
{
	[Serializable]
	public class ComponentType
	{
		public ComponentType(Type concreteType, string typeAsCacheKeyString)
		{
			ConcreteType = concreteType;
			TypeAsCacheKeyString = typeAsCacheKeyString ?? concreteType.ToString();
		}

		public Type ConcreteType { get; }
		public string TypeAsCacheKeyString { get; }

		public override string ToString()
		{
			return TypeAsCacheKeyString;
		}
	}
}