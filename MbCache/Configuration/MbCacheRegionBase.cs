using System;
using System.Reflection;
using MbCache.Logic;

namespace MbCache.Configuration
{
    /// <summary>
    /// Base class for users to override to implement
    /// their own logic for building cache regions
    /// </summary>
    /// <remarks>
    /// Created by: rogerkr
    /// Created date: 2010-03-02
    /// </remarks>
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
            var ret = string.Empty;
            foreach (var parameter in parameters)
            {
                ret += RegionPartForParameterValue(type, methodInfo, parameter) + Separator;
            }
            return ret;  
        }

        protected abstract string RegionPartForParameterValue(Type type, MethodInfo info, object parameter);
    }
}