using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Ceras.Formatters;
using Ceras.Helpers;

namespace Ceras
{
	internal class SpecificConstructor : MethodBaseConstruction
	{
		internal ConstructorInfo Constructor;

		internal override bool HasDataArguments => Constructor.GetParameters().Length != 0;

		public SpecificConstructor(ConstructorInfo constructor)
		{
			Constructor = constructor;
		}

		internal override Func<object> GetRefFormatterConstructor(bool allowDynamicCodeGen)
		{
			if (allowDynamicCodeGen)
			{
				return Expression.Lambda<Func<object>>(Expression.New(Constructor), Array.Empty<ParameterExpression>()).Compile();
			}
			return () => Constructor.Invoke(null);
		}

		internal override void EmitConstruction(Schema schema, List<Expression> body, ParameterExpression refValueArg, HashSet<ParameterExpression> usedVariables, MemberParameterPair[] memberParameters)
		{
			ParameterInfo[] parameters = Constructor.GetParameters();
			Expression[] arguments = GenerateArgumentExpressions(parameters, schema, usedVariables, memberParameters);
			BinaryExpression item = Expression.Assign(refValueArg, Expression.New(Constructor, arguments));
			body.Add(item);
		}

		internal override void VerifyReturnType()
		{
			VerifyMethodReturn(Constructor);
		}

		internal override void VerifyParameterMapping()
		{
			VerifyParameterMapping(Constructor);
		}
	}
}
