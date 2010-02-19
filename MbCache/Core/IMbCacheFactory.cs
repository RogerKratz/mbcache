namespace MbCache.Core
{
    public interface IMbCacheFactory
    {
        T Get<T>() where T : class;
    }
}