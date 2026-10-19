using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Ceras.Formatters;
using Ceras.Helpers;

namespace Ceras
{
	internal class UninitializedObject : TypeConstruction
	{
		private static MethodInfo _getUninitialized;

		private ConstructorInfo _directConstructor;

		private bool _writeMembersAgain;

		internal override bool HasDataArguments
		{
			get
			{
				if (_directConstructor != null)
				{
					return _directConstructor.GetParameters().Length != 0;
				}
				return false;
			}
		}

		public UninitializedObject()
		{
			if (_getUninitialized == null)
			{
				_getUninitialized = ((MethodCallExpression)((Expression<Func<object>>)(() => FormatterServices.GetUninitializedObject(null))).Body).Method;
			}
		}

		internal override Func<object> GetRefFormatterConstructor(bool allowDynamicCodeGen)
		{
			Type type = TypeConfig.Type;
			return Expression.Lambda<Func<object>>(Expression.Call(_getUninitialized, Expression.Constant(type)), Array.Empty<ParameterExpression>()).Compile();
		}

		internal override void VerifyReturnType()
		{
			if (_directConstructor != null && _directConstructor.DeclaringType != TypeConfig.Type)
			{
				throw new InvalidOperationException("The given constructor is not part of the type '" + TypeConfig.Type.FullName + "'");
			}
		}

		internal override void EmitConstruction(Schema schema, List<Expression> body, ParameterExpression refValueArg, HashSet<ParameterExpression> usedVariables, MemberParameterPair[] memberParameters)
		{
			throw new NotImplementedException("running a ctor or factory is not yet supported in this mode");
		}
	}
}
