using System;
using System.Reflection;

namespace MbCache.Logic
{
    public interface IMbCacheKey
    {
        string CacheKey(Type type, MethodInfo methodInfo);
        string AddForParameterValues(Type type, MethodInfo methodInfo, object[] parameters);
        string AddForComponent(ICachingComponent component); 
    }
}