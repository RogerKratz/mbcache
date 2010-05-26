using System;
using System.Linq.Expressions;

namespace MbCache.Configuration
{
    public interface IFluentBuilder<T>
    {
        IFluentBuilder<T> AddMethod(Expression<Func<T, object>> expression);
    }
}