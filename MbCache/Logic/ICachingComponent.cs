using System;
using System.Linq.Expressions;

namespace MbCache.Logic
{
    public interface ICachingComponent
    {
        string UniqueId { get; }
        void Invalidate();
        void Invalidate<T>(Expression<Func<T, object>> method);
    }
}