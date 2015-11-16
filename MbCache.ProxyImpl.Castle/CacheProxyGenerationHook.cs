using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using MbCache.Logic;

namespace MbCache.ProxyImpl.Castle
{
	public class CacheProxyGenerationHook : IProxyGenerationHook
	{
		private readonly IEnumerable<MethodInfo> _cachedMethods;

		public CacheProxyGenerationHook(IEnumerable<MethodInfo> cachedMethods)
		{
			_cachedMethods = cachedMethods;
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
			return _cachedMethods.Contains(key, MethodInfoComparer.Instance);
		}

		public override bool Equals(object obj)
		{
			var casted = obj as CacheProxyGenerationHook;
			return casted != null && casted._cachedMethods.SequenceEqual(_cachedMethods, MethodInfoComparer.Instance);
		}

		public override int GetHashCode()
		{
			return _cachedMethods.GetHashCode();
		}
	}
}
