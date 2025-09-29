using System;

namespace MbCache.Logic;

public class ComponentType(Type concreteType, string typeAsCacheKeyString)
{
	public Type ConcreteType { get; } = concreteType;
	public string TypeAsCacheKeyString { get; } = typeAsCacheKeyString ?? concreteType.ToString();
}