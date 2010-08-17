using System;
using System.Linq.Expressions;
using System.Reflection;

namespace MbCache.Logic
{
    internal static class ExpressionHelper
    {
        internal static MethodInfo MemberName(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Call:
                    var callExpression = (MethodCallExpression)expression;
                    return callExpression.Method;

                case ExpressionType.Convert:
                    var unaryExpression = (UnaryExpression)expression;
                    return MemberName(unaryExpression.Operand);

                default:
                    throw new ArgumentException("The expression is not a member access or method call expression");
            }
        }
    }
}
