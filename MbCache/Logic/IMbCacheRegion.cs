using System;
using System.Reflection;

namespace MbCache.Logic
{
    public interface IMbCacheRegion
    {
        string Region(Type type, MethodInfo methodInfo);
        string AdditionalRegionsForParameterValues(Type type, MethodInfo methodInfo, object[] parameters);
    }
}