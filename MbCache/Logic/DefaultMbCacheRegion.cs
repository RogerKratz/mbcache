namespace MbCache.Configuration
{
    public class DefaultMbCacheRegion : IMbCacheRegion
    {
        public string Region(string methodName)
        {
            return methodName;
        }
    }
}
