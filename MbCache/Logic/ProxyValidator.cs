using System;
using System.Reflection;

namespace MbCache.Logic;

public static class ProxyValidator
{
	public static void Validate(ConfigurationForType configurationForType) => 
		checkAllMethodsAreVirtual(configurationForType);

	private static void checkAllMethodsAreVirtual(ConfigurationForType configurationForType)
	{
		foreach (var memberInfo in configurationForType.ComponentType.ConcreteType.GetMembers())
		{
			checkMethod(configurationForType.ComponentType.ConcreteType, memberInfo);
		}
	}

	private static void checkMethod(Type type, MemberInfo member)
	{
		var method = member as MethodInfo;
		if (method != null && (method.Name != "GetType" || method.DeclaringType != typeof(object)))
		{
			checkMethodIsVirtual(type, method);
		}
	}

	private static void checkMethodIsVirtual(Type type, MethodInfo method)
	{
		if (!canBeProxied(method))
		{
			throw new InvalidOperationException(
				$"{method.Name} on type {type} is not virtual. Either make this method virtual or make component from its interface instead (.As<ComponentInterface>()).");
		}
	}

	private static bool canBeProxied(MethodInfo method)
	{
		// In NET if IsVirtual is false or IsFinal is true, then the method cannot be overridden.
		return !(method.DeclaringType != typeof(object) && !isDisposeMethod(method)
		                                                && (method.IsPublic || method.IsAssembly || method.IsFamilyOrAssembly)
		                                                && (!method.IsVirtual || method.IsFinal || (method.IsVirtual && method.IsAssembly)));
	}

	private static bool isDisposeMethod(MethodBase method) => 
		method.Name.Equals("Dispose") && method.MemberType == MemberTypes.Method && method.GetParameters().Length == 0;
}