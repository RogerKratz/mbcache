using System;
using System.Linq.Expressions;

namespace MbCache.Core
{
    /// <summary>
    /// Main type at run time.
    /// Creates proxies with (possibly) cached method logic,
    /// depending on configuration.
    /// 
    /// Also have methods to invalidate types or its specific methods
    /// </summary>
    /// <remarks>
    /// Created by: rogerkr
    /// Created date: 2010-03-02
    /// </remarks>
    public interface IMbCacheFactory
    {
        /// <summary>
        /// Creates the proxy with ctor parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <remarks>
        /// Created by: rogerkr
        /// Created date: 2010-08-17
        /// </remarks>
        T Create<T>(params object[] parameters);

        /// <summary>
        /// Invalidates all cache entrys for instance T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <remarks>
        /// Created by: rogerkr
        /// Created date: 2010-08-17
        /// </remarks>
        void Invalidate<T>();

        /// <summary>
        /// Invalidates all cache entrys for method T.method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method">The method.</param>
        /// <remarks>
        /// Created by: rogerkr
        /// Created date: 2010-08-17
        /// </remarks>
        void Invalidate<T>(Expression<Func<T, object>> method);
    }
}