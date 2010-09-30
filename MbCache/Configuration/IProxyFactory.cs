using MbCache.Logic;

namespace MbCache.Configuration
{
    /// <summary>
    /// Creates the proxy. 
    ///  
    /// To implementors
    /// The implementation of this interface needs a ctor with this signature
    /// public ProxyFactory(ICache cache, IMbCacheKey mbCacheKey)
    /// </summary>
    public interface IProxyFactory
    {
        /// <summary>
        /// Called once
        /// </summary>
        void Initialize(ICache cache, IMbCacheKey mbCacheKey);

        /// <summary>
        /// Creates the proxy.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodData">The method data.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        T CreateProxy<T>(ImplementationAndMethods methodData, params object[] parameters);

        /// <summary>
        /// Gets a value indicating whether [allow non virtual member]
        /// for methods not marked for caching.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [allow non virtual member]; otherwise, <c>false</c>.
        /// </value>
        bool AllowNonVirtualMember { get; }
    }
}