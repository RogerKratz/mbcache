using System;
using System.Reflection;
using MbCache.Logic;

namespace MbCache.Configuration
{
    /// <summary>
    /// Base class for users to override to implement
    /// their own logic for building cache keys
    /// </summary>
    /// <remarks>
    /// Created by: rogerkr
    /// Created date: 2010-03-02
    /// </remarks>
    public abstract class MbCacheKeyBase : IMbCacheKey
    {
        protected const string Separator = "|";

        public string CacheKey(Type type, MethodInfo methodInfo)
        {
            var ret = type + Separator + methodInfo.Name + Separator;
            foreach (var parameter in methodInfo.GetParameters())
            {
                ret += parameter.Name + Separator;
            }
            return ret;
        }

        public string AddForParameterValues(Type type, MethodInfo methodInfo, object[] parameters)
        {
            var ret = string.Empty;
            foreach (var parameter in parameters)
            {
                ret += KeyPartForParameterValue(type, methodInfo, parameter) + Separator;
            }
            return ret;  
        }

        protected abstract string KeyPartForParameterValue(Type type, MethodInfo info, object parameter);
    }
}