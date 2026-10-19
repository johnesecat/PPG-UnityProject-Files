using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Ceras.Exceptions;
using Ceras.Formatters;
using Ceras.Helpers;

namespace Ceras
{
	public abstract class TypeConstruction
	{
		internal TypeConfig TypeConfig;

		internal abstract bool HasDataArguments { get; }

		internal abstract Func<object> GetRefFormatterConstructor(bool allowDynamicCodeGen);

		internal virtual void EmitConstruction(Schema schema, List<Expression> body, ParameterExpression refValueArg, HashSet<ParameterExpression> usedVariables, MemberParameterPair[] memberParameters)
		{
			throw new NotImplementedException("This construction type can not be used in deferred mode.");
		}

		internal virtual void VerifyReturnType()
		{
		}

		internal virtual void VerifyParameterMapping()
		{
		}

		protected void VerifyMethodReturn(MethodBase methodBase)
		{
			if (methodBase.IsAbstract)
			{
				throw new InvalidOperationException("The given method '" + methodBase.Name + "' is abstract so it can not be used to construct anything.");
			}
			MethodInfo methodInfo = methodBase as MethodInfo;
			Type type;
			if ((object)methodInfo != null)
			{
				type = methodInfo.ReturnType;
			}
			else
			{
				ConstructorInfo constructorInfo = methodBase as ConstructorInfo;
				if ((object)constructorInfo == null)
				{
					throw new NotImplementedException("this helper method cannot handle a member info of type " + methodBase.GetType().FullName);
				}
				type = constructorInfo.DeclaringType;
			}
			Type type2 = TypeConfig.Type;
			if (!type2.IsAssignableFrom(type))
			{
				throw new InvalidOperationException("The given method or constructor returns a '" + type.FullName + "' which is not compatible to the needed type '" + type2.FullName + "'");
			}
		}

		protected void VerifyParameterMapping(MethodBase methodBase)
		{
			ParameterInfo[] parameters = methodBase.GetParameters();
			if (parameters.Length == 0)
			{
				return;
			}
			Dictionary<ParameterInfo, MemberInfo> map = TypeConfig.ParameterMap ?? (TypeConfig.ParameterMap = new Dictionary<ParameterInfo, MemberInfo>());
			MemberConfig[] source = TypeConfig.Members.Where((MemberConfig mc) => mc.ComputeFinalInclusionFast()).ToArray();
			ParameterInfo[] array = parameters;
			foreach (ParameterInfo parameterInfo in array)
			{
				if (!map.TryGetValue(parameterInfo, out var sourceMember))
				{
					MemberConfig[] configs2 = source.Where((MemberConfig mc) => parameterInfo.Name.Equals(mc.Member.Name, StringComparison.OrdinalIgnoreCase)).ToArray();
					if (SetMatchOrThrow(parameterInfo, configs2))
					{
						continue;
					}
					MemberConfig[] configs3 = source.Where((MemberConfig mc) => parameterInfo.Name.Equals(MiscHelpers.CleanMemberName(mc.Member.Name), StringComparison.OrdinalIgnoreCase)).ToArray();
					if (!SetMatchOrThrow(parameterInfo, configs3))
					{
						string text = string.Join(", ", parameters.Select((ParameterInfo t) => t.ParameterType.FriendlyName() + " " + t.Name));
						throw new CerasException("There is no mapping specified from the members of '" + TypeConfig.Type.FriendlyName() + "' to the constructor '(" + text + ")'. Ceras has tried to automatically detect a mapping by matching the names of the fields/properties to the method parameters, but no source field or property could be found to populate the parameter '" + parameterInfo.ParameterType.FriendlyName() + " " + parameterInfo.Name + "'");
					}
				}
				else if (!TypeConfig.Members.First((MemberConfig mc) => mc.Member == sourceMember).ComputeFinalInclusionFast())
				{
					throw new CerasException("The type construction mode for the type '" + TypeConfig.Type.FriendlyName() + "' is invalid because the parameter '" + parameterInfo.ParameterType.FriendlyName() + " " + parameterInfo.Name + "' is supposed to be initialized from the member '" + sourceMember.FieldOrPropType().FriendlyName() + " " + sourceMember.Name + "', but that member is not part of the serialization, so it will not be available at deserialization-time.");
				}
			}
			bool SetMatchOrThrow(ParameterInfo p, MemberConfig[] configs)
			{
				if (configs.Length > 1)
				{
					throw new AmbiguousMatchException("There are multiple members that match the parameter '" + p.ParameterType.FriendlyName() + " " + p.Name + "': " + string.Join(", ", configs.Select((MemberConfig c) => c.Member.Name)));
				}
				if (configs.Length == 0)
				{
					return false;
				}
				map.Add(p, configs[0].Member);
				return true;
			}
		}

		public static TypeConstruction Null()
		{
			return ConstructNull.Instance;
		}

		public static TypeConstruction ByStaticMethod(MethodInfo methodInfo)
		{
			return new ConstructByMethod(methodInfo);
		}

		public static TypeConstruction ByStaticMethod(Expression<Func<object>> expression)
		{
			return new ConstructByMethod(((MethodCallExpression)expression.Body).Method);
		}

		public static TypeConstruction ByConstructor(ConstructorInfo constructorInfo)
		{
			return new SpecificConstructor(constructorInfo);
		}

		public static TypeConstruction ByUninitialized()
		{
			return new UninitializedObject();
		}
	}
}
