using System;
namespace MbCache.Logic
{
    public class DefaultMbCacheRegion : IMbCacheRegion
    {
        public string Region(Type type, string methodName)
        {
            return type.ToString() + "|" + methodName;
        }
    }
}
