using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Ceras.Formatters;
using Ceras.Helpers;

namespace Ceras
{
	internal abstract class MethodBaseConstruction : TypeConstruction
	{
		protected Expression[] GenerateArgumentExpressions(ParameterInfo[] targetMethodParameters, Schema schema, HashSet<ParameterExpression> usedVariables, MemberParameterPair[] memberParameters)
		{
			Expression[] array = new Expression[targetMethodParameters.Length];
			for (int i = 0; i < targetMethodParameters.Length; i++)
			{
				ParameterInfo parameterInfo = targetMethodParameters[i];
				MemberInfo sourceMember = TypeConfig.ParameterMap[parameterInfo];
				SchemaMember schemaMember = schema.Members.First((SchemaMember m) => m.MemberInfo == sourceMember);
				if (schemaMember.IsSkip)
				{
					throw new InvalidOperationException("Can not generate the constructor-call or call to the factory method for type '" + schema.Type.FullName + "'. The parameter '" + parameterInfo.Name + "' is not part of the serialization / serialized data.");
				}
				MemberParameterPair memberParameterPair = memberParameters.First((MemberParameterPair m) => m.Member == schemaMember.MemberInfo);
				array[i] = memberParameterPair.LocalVar;
				usedVariables.Add(memberParameterPair.LocalVar);
			}
			return array;
		}
	}
}
