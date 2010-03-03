using System;
using System.Reflection;
using MbCache.Configuration;

namespace MbCache.DefaultImpl
{
    public class ToStringMbCacheRegion : MbCacheRegionBase
    {
        protected override string RegionPartForParameterValue(Type type, MethodInfo info, object parameter)
        {
            return parameter.ToString();
        }
    }
}