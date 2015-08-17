using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using MbCache.Logic;

namespace MbCache.ProxyImpl.Castle
{
	public class CacheProxyGenerationHook : IProxyGenerationHook
	{
		private readonly ConfigurationForType _methodData;

		public CacheProxyGenerationHook(ConfigurationForType methodData)
		{
			_methodData = methodData;
		}

		public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
		{
			return isMethodMarkedForCaching(methodInfo);
		}

		public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
		{
		}

		public void MethodsInspected()
		{
		}

		private bool isMethodMarkedForCaching(MethodInfo key)
		{
			//ugly hack for now
			if (key.IsGenericMethod)
				return true;
			return _methodData.CachedMethods.Contains(key, MethodInfoComparer.Instance);
		}

		public override bool Equals(object obj)
		{
			var casted = obj as CacheProxyGenerationHook;
			return casted != null && casted._methodData.CachedMethods.SequenceEqual(_methodData.CachedMethods, MethodInfoComparer.Instance);
		}

		public override int GetHashCode()
		{
			return _methodData.CachedMethods.GetHashCode();
		}
	}
}
