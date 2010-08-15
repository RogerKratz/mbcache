namespace MbCache.Logic
{
    public interface ICachingComponent
    {
        string UniqueId { get; }
        void Invalidate();
    }
}