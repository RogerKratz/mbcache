namespace MbCache.Logic
{
    /// <summary>
    /// Factory to create an instance to 
    /// communicate with 3rd part cache framework
    /// </summary>
    /// <remarks>
    /// Created by: rogerkr
    /// Created date: 2010-03-02
    /// </remarks>
    public interface ICacheFactory
    {
        ICache Create();
    }
}