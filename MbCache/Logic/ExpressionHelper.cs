using System;
using System.Linq.Expressions;

namespace MbCache.Logic
{
    internal static class ExpressionHelper
    {
        internal static string MemberName(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expression;
                    var supername = MemberName(memberExpression.Expression);

                    if (String.IsNullOrEmpty(supername))
                        return memberExpression.Member.Name;

                    return String.Concat(supername, '.', memberExpression.Member.Name);

                case ExpressionType.Call:
                    var callExpression = (MethodCallExpression)expression;
                    return callExpression.Method.Name;

                case ExpressionType.Convert:
                    var unaryExpression = (UnaryExpression)expression;
                    return MemberName(unaryExpression.Operand);

                case ExpressionType.Parameter:
                    return String.Empty;

                default:
                    throw new ArgumentException("The expression is not a member access or method call expression");
            }
        }
    }
}
