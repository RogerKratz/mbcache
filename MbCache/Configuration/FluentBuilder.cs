using System;
using System.Linq.Expressions;
using MbCache.Logic;

namespace MbCache.Configuration
{
    public class FluentBuilder<T> : IFluentBuilder<T>
    {
        private readonly ImplementationAndMethods _details;

        public FluentBuilder(ImplementationAndMethods details)
        {
            _details = details;
        }

        public IFluentBuilder<T> CacheMethod(Expression<Func<T, object>> expression)
        {
            _details.Methods.Add(ExpressionHelper.MemberName(expression.Body));
            return this;
        }

        public IFluentBuilder<T> PerInstance()
        {
            _details.CachePerInstance = true;
            return this;
        }
    }
}