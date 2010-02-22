using System;
using System.Reflection;
using Castle.DynamicProxy;

namespace MbCache.Logic
{
    public class CacheProxyGenerationHook : IProxyGenerationHook
    {
        public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
        {
            return true;
        }

        public void NonVirtualMemberNotification(Type type, MemberInfo memberInfo)
        {
            throw new ArgumentException("Method " + memberInfo.Name + " on type " + type + " must be virtual.");
        }

        public void MethodsInspected()
        {
        }
    }
}
