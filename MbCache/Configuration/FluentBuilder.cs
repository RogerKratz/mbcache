using System;
using System.Collections.Generic;
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

        public IFluentBuilder<T> AddMethod(Expression<Func<T, object>> expression)
        {
            _details.Methods.Add(ExpressionHelper.MemberName(expression.Body));
            return this;
        }
    }
}