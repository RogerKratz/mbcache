using MbCache.Logic;

namespace MbCache.Configuration
{
    /// <summary>
    /// Creates the proxies. Handled in external dll to make
    /// mbcache unaware of underlying proxy framework.
    /// 
    /// To implementors
    /// The implementation of this interface needs a ctor with this signature
    /// public ProxyFactory(ICache cache, IMbCacheKey mbCacheKey)
    /// 
    /// </summary>
    public interface IProxyFactory
    {
        T CreateProxy<T>(ImplementationAndMethods methodData,
                            params object[] parameters);
    }
}