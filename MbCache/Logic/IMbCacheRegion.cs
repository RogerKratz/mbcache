using System;
namespace MbCache.Logic
{
    public interface IMbCacheRegion
    {
        string Region(Type type, string methodName);
    }
}