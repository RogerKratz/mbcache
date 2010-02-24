using System;
using System.Reflection;
using MbCache.Core;

namespace MbCache.Logic
{
    public class DefaultMbCacheRegion : MbCacheRegionBase
    {
        protected override string RegionPartForParameterValue(Type type, MethodInfo info, object parameter)
        {
            return parameter.ToString();
        }
    }
}
