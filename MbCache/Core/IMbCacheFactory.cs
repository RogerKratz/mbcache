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
        T Create<T>(params object[] ctorParameters);
        void Invalidate<T>();
        void Invalidate<T>(Expression<Func<T, object>> method);
    }
}