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
		private readonly IEnumerable<MethodInfo> _methods;

		public CacheProxyGenerationHook(IEnumerable<MethodInfo> methods)
		{
			_methods = methods;
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
			return _methods.Contains(key, new MethodInfoComparer());
		}

		public override bool Equals(object obj)
		{
			var casted = obj as CacheProxyGenerationHook;
			return casted != null && casted._methods.SequenceEqual(_methods, new MethodInfoComparer());
		}

		public override int GetHashCode()
		{
			return _methods.GetHashCode();
		}
	}
}
