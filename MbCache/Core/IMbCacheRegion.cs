using System;
using System.Reflection;

namespace MbCache.Core
{
    public interface IMbCacheRegion
    {
        string Region(Type type, MethodInfo methodInfo);
        string AdditionalRegionsForParameterValues(object[] parameters);
    }
}