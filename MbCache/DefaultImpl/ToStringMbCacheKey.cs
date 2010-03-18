using System;
using System.Reflection;
using MbCache.Configuration;

namespace MbCache.DefaultImpl
{
    public class ToStringMbCacheKey : MbCacheKeyBase
    {
        protected override string KeyPartForParameterValue(Type type, MethodInfo info, object parameter)
        {
            return parameter.ToString();
        }
    }
}