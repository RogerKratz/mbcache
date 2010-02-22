namespace MbCache.Core
{
    public interface IMbCacheFactory
    {
        T Create<T>() where T : class;
        void Invalidate<T>();
    }
}