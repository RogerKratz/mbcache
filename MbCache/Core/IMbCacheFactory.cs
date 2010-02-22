using System;
using System.Linq.Expressions;

namespace MbCache.Core
{
    public interface IMbCacheFactory
    {
        T Create<T>() where T : class;
        void Invalidate<T>();
        void Invalidate<T>(Expression<Func<T, object>> method);
    }
}