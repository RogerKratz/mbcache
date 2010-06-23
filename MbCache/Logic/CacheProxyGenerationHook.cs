using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy;

namespace MbCache.Logic
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

        public void NonVirtualMemberNotification(Type type, MemberInfo memberInfo)
        {
            var methodInfo = memberInfo as MethodInfo;
            if(methodInfo!=null && isMethodMarkedForCaching(methodInfo))
                throw new ArgumentException("Method " + memberInfo.Name + " on type " + type + " must be virtual.");
        }

        public void MethodsInspected()
        {
        }

        private bool isMethodMarkedForCaching(MethodInfo key)
        {
            foreach (var methodInfo in _methods)
            {
                if (methodInfo.Equals(key))
                    return true;
            }
            return false;
        }
    }
}
