using System;
using System.Linq.Expressions;
using System.Reflection;

namespace HarmonyLib
{
	public static class SymbolExtensions
	{
		public static MethodInfo GetMethodInfo(Expression<Action> expression)
		{
			return GetMethodInfo((LambdaExpression)expression);
		}

		public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
		{
			return GetMethodInfo((LambdaExpression)expression);
		}

		public static MethodInfo GetMethodInfo<T, TResult>(Expression<Func<T, TResult>> expression)
		{
			return GetMethodInfo((LambdaExpression)expression);
		}

		public static MethodInfo GetMethodInfo(LambdaExpression expression)
		{
			MethodCallExpression methodCallExpression = expression.Body as MethodCallExpression;
			if (methodCallExpression == null)
			{
				UnaryExpression unaryExpression = expression.Body as UnaryExpression;
				if (unaryExpression != null)
				{
					MethodCallExpression methodCallExpression2 = unaryExpression.Operand as MethodCallExpression;
					if (methodCallExpression2 != null)
					{
						ConstantExpression constantExpression = methodCallExpression2.Object as ConstantExpression;
						if (constantExpression != null)
						{
							MethodInfo methodInfo = constantExpression.Value as MethodInfo;
							if ((object)methodInfo != null)
							{
								return methodInfo;
							}
						}
					}
				}
				throw new ArgumentException("Invalid Expression. Expression should consist of a Method call only.");
			}
			return methodCallExpression.Method ?? throw new Exception($"Cannot find method for expression {expression}");
		}
	}
}
