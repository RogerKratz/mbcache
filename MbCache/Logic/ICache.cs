namespace MbCache.Logic
{
    public interface ICache
    {
        object Get(string key);
        void Put(string key, object value);
        void Delete(string keyStartingWith);
    }
}