using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Ceras.Formatters;
using Ceras.Helpers;

namespace Ceras
{
	internal class ConstructByMethod : MethodBaseConstruction
	{
		internal readonly MethodInfo Method;

		internal readonly object TargetObject;

		internal override bool HasDataArguments => Method.GetParameters().Length != 0;

		internal ConstructByMethod(MethodInfo staticMethod)
		{
			if (!staticMethod.IsStatic)
			{
				throw new InvalidOperationException("You have provided an instance method without a target object");
			}
			Method = staticMethod;
		}

		internal ConstructByMethod(object targetObject, MethodInfo instanceMethod)
		{
			if (instanceMethod.IsStatic)
			{
				throw new InvalidOperationException("You have provided target-instance but the given method is a static method");
			}
			if (targetObject == null)
			{
				throw new ArgumentNullException("targetObject", "The given method requires an instance (a targetObject), but you have given 'null'");
			}
			Method = instanceMethod;
			TargetObject = targetObject;
		}

		internal override Func<object> GetRefFormatterConstructor(bool allowDynamicCodeGen)
		{
			if (Method.IsStatic)
			{
				return (Func<object>)Delegate.CreateDelegate(typeof(Func<object>), Method);
			}
			return (Func<object>)Delegate.CreateDelegate(typeof(Func<object>), TargetObject, Method);
		}

		internal override void EmitConstruction(Schema schema, List<Expression> body, ParameterExpression refValueArg, HashSet<ParameterExpression> usedVariables, MemberParameterPair[] memberParameters)
		{
			ParameterInfo[] parameters = Method.GetParameters();
			Expression[] arguments = GenerateArgumentExpressions(parameters, schema, usedVariables, memberParameters);
			Expression item = ((!Method.IsStatic) ? Expression.Assign(refValueArg, Expression.Call(Expression.Constant(TargetObject), Method, arguments)) : Expression.Assign(refValueArg, Expression.Call(Method, arguments)));
			body.Add(item);
		}

		internal override void VerifyReturnType()
		{
			VerifyMethodReturn(Method);
		}

		internal override void VerifyParameterMapping()
		{
			VerifyParameterMapping(Method);
		}
	}
}
