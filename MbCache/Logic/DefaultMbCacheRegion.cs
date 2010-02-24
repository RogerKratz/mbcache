using System;
using System.Reflection;
using MbCache.Core;

namespace MbCache.Logic
{
    public class DefaultMbCacheRegion : IMbCacheRegion
    {
        public string Region(Type type, MethodInfo methodInfo)
        {
            var ret = type + "|" + methodInfo.Name + "|";
            foreach (var parameter in methodInfo.GetParameters())
            {
                ret += parameter.Name + "|";
            }
            return ret;
        }

        public string AdditionalRegionsForParameterValues(object[] parameters)
        {
            string ret = string.Empty;
            foreach (var parameter in parameters)
            {
                ret += parameter + "|";
            }
            return ret;  
        }
    }
}
