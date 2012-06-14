using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace MbCache.ProxyImpl.Castle
{
	public class CacheProxyGenerationHook : IProxyGenerationHook
	{
		private readonly IEnumerable<string> _methodNames;

		public CacheProxyGenerationHook(IEnumerable<MethodInfo> methods)
		{
			_methodNames = methods.Select(methodInfo => methodInfo.Name).ToArray();
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
			return _methodNames.Contains(key.Name);
		}

		public override bool Equals(object obj)
		{
			var casted = obj as CacheProxyGenerationHook;
			return casted != null && casted._methodNames.SequenceEqual(_methodNames);
		}

		public override int GetHashCode()
		{
			return _methodNames.GetHashCode();
		}
	}
}
