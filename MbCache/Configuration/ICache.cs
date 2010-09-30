namespace MbCache.Configuration
{
    /// <summary>
    /// Object communicating with 3rd part cache framework
    /// </summary>
    /// <remarks>
    /// Created by: rogerkr
    /// Created date: 2010-03-02
    /// </remarks>
    public interface ICache
    {
        object Get(string key);
        void Put(string key, object value);
        void Delete(string keyStartingWith);
    }
}