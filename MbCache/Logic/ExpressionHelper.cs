using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

		internal static IEnumerable<object> ExtractArguments(Expression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Call:
					var callExpression = (MethodCallExpression)expression;
					return extractConstants(callExpression);

				case ExpressionType.Convert:
					var unaryExpression = (UnaryExpression)expression;
					return ExtractArguments(unaryExpression.Operand);

				default:
					throw new ArgumentException("The expression is not a member access or method call expression");
			}
		}

		private static IEnumerable<object> extractConstants(MethodCallExpression methodCallExpression)
		{
			foreach (var arg in methodCallExpression.Arguments)
			{
				foreach (var constant in extractConstants(arg))
					yield return constant;
			}

			foreach (var constant in extractConstants(methodCallExpression.Object))
				yield return constant;
		}

		private static IEnumerable<object> extractConstants(Expression expression)
		{
			if (expression == null || expression is ParameterExpression)
				return new object[0];

			var memberExpression = expression as MemberExpression;
			if (memberExpression != null)
				return extractConstants(memberExpression).ToArray();

			var constantExpression = expression as ConstantExpression;
			if (constantExpression != null)
				return extractConstants(constantExpression);

			var newArrayExpression = expression as NewArrayExpression;
			if (newArrayExpression != null)
				return extractConstants(newArrayExpression);

			var unaryExpression = expression as UnaryExpression;
			if (unaryExpression != null)
				return extractConstants(unaryExpression);

			throw new InvalidOperationException("Could not fetch the arguments from the provided exception. This may be a bug so please report it");
		}

		private static IEnumerable<object> extractConstants(UnaryExpression unaryExpression)
		{
			return extractConstants(unaryExpression.Operand);
		}

		private static IEnumerable<object> extractConstants(NewArrayExpression newArrayExpression)
		{
			var arrayElements = new ArrayList();
			Type type = newArrayExpression.Type.GetElementType();
			foreach (var arrayElementExpression in newArrayExpression.Expressions)
			{
				var arrayElement = ((ConstantExpression)arrayElementExpression).Value;
				arrayElements.Add(Convert.ChangeType(arrayElement, arrayElementExpression.Type));
			}

			yield return arrayElements.ToArray(type);

		}

		private static IEnumerable<object> extractConstants(ConstantExpression constantExpression)
		{
			if (constantExpression.Value is Expression)
			{
				foreach (var constant in extractConstants((Expression)constantExpression.Value))
				{
					yield return constant;
				}
			}
			else
			{
				if (constantExpression.Type == typeof(string) || constantExpression.Type.IsPrimitive)
					yield return constantExpression.Value;
			}
		}

		private static IEnumerable<object> extractConstants(MemberExpression memberExpression)
		{
			var constExpression = (ConstantExpression)memberExpression.Expression;
			var type = constExpression.Type;
			var member = type.GetMember(memberExpression.Member.Name, MemberTypes.Field | MemberTypes.Property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Single();

			if (member.MemberType == MemberTypes.Field)
				yield return ((FieldInfo)member).GetValue(constExpression.Value);
			else
				yield return ((PropertyInfo)member).GetGetMethod(true).Invoke(constExpression.Value, null);
		}
	}
}
