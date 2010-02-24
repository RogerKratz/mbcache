using System;
using System.Reflection;
using MbCache.Logic;

namespace MbCache.Core
{
    public abstract class MbCacheRegionBase : IMbCacheRegion
    {
        protected const string Separator = "|";

        public string Region(Type type, MethodInfo methodInfo)
        {
            var ret = type + Separator + methodInfo.Name + Separator;
            foreach (var parameter in methodInfo.GetParameters())
            {
                ret += parameter.Name + Separator;
            }
            return ret;
        }

        public string AdditionalRegionsForParameterValues(Type type, MethodInfo methodInfo, object[] parameters)
        {
            string ret = string.Empty;
            foreach (var parameter in parameters)
            {
                ret += RegionPartForParameterValue(type, methodInfo, parameter) + Separator;
            }
            return ret;  
        }

        protected abstract string RegionPartForParameterValue(Type type, MethodInfo info, object parameter);
    }
}
