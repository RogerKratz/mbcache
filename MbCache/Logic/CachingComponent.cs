namespace MbCache.Logic
{
    public class CachingComponent : ICachingComponent
    {
        public CachingComponent()
        {
            UniqueId = "Global";
        }
        public string UniqueId { get; set; }
    }
}