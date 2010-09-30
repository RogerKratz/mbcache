using System;
using System.Linq.Expressions;

namespace MbCache.Core
{
    public interface ICachingComponent
    {
        string UniqueId { get; }
        void Invalidate();
        void Invalidate<T>(Expression<Func<T, object>> method);
    }
}