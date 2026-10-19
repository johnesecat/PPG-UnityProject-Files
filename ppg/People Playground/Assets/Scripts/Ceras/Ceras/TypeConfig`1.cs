using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Ceras.Formatters;

namespace Ceras
{
	public class TypeConfig<T> : TypeConfig
	{
		public new IFormatter<T> CustomFormatter
		{
			get
			{
				return (IFormatter<T>)base.CustomFormatter;
			}
			set
			{
				base.CustomFormatter = value;
			}
		}

		internal TypeConfig(SerializerConfig config, bool isStatic)
			: base(config, typeof(T), isStatic)
		{
		}

		public TypeConfig<T> ConstructByFormatter()
		{
			base.TypeConstruction = TypeConstruction.Null();
			return this;
		}

		public TypeConfig<T> ConstructBy(MethodInfo methodInfo)
		{
			base.TypeConstruction = new ConstructByMethod(methodInfo);
			return this;
		}

		public TypeConfig<T> ConstructBy(object instance, MethodInfo methodInfo)
		{
			base.TypeConstruction = new ConstructByMethod(instance, methodInfo);
			return this;
		}

		public TypeConfig<T> ConstructBy(ConstructorInfo constructorInfo)
		{
			if (constructorInfo.IsAbstract || constructorInfo.DeclaringType != base.Type)
			{
				throw new InvalidOperationException("This constructor does not belong to the type " + base.Type.FullName);
			}
			base.TypeConstruction = new SpecificConstructor(constructorInfo);
			return this;
		}

		public TypeConfig<T> ConstructByDelegate(Func<T> factory)
		{
			object target = factory.Target;
			MethodInfo method = factory.Method;
			base.TypeConstruction = new ConstructByMethod(target, method);
			return this;
		}

		public TypeConfig<T> ConstructBy(Expression<Func<T>> methodSelectExpression)
		{
			return ConstructBy(null, methodSelectExpression);
		}

		public TypeConfig<T> ConstructBy(object instance, Expression<Func<T>> methodSelectExpression)
		{
			Expression body = methodSelectExpression.Body;
			NewExpression newExpression = body as NewExpression;
			if (newExpression != null)
			{
				if (instance != null)
				{
					throw new InvalidOperationException("You can't specify a constructor and an instance at the same time");
				}
				ConstructorInfo constructor = newExpression.Constructor;
				base.TypeConstruction = new SpecificConstructor(constructor);
			}
			else
			{
				MethodCallExpression methodCallExpression = body as MethodCallExpression;
				if (methodCallExpression == null)
				{
					throw new InvalidOperationException("The given expression must be a 'method-call' or 'new-expression'");
				}
				if (instance == null)
				{
					base.TypeConstruction = new ConstructByMethod(methodCallExpression.Method);
				}
				else
				{
					base.TypeConstruction = new ConstructByMethod(instance, methodCallExpression.Method);
				}
			}
			return this;
		}

		public TypeConfig<T> ConstructBy(TypeConstruction manualConstructConfig)
		{
			base.TypeConstruction = manualConstructConfig;
			return this;
		}

		public TypeConfig<T> ConstructByUninitialized()
		{
			base.TypeConstruction = new UninitializedObject();
			return this;
		}

		public TypeConfig<T> MapParameters(Dictionary<ParameterInfo, MemberInfo> mapping)
		{
			if (ParameterMap != null)
			{
				throw new InvalidOperationException("ParameterMapping is already set");
			}
			if (_typeConstruction == null)
			{
				throw new InvalidOperationException("You must set a type construction method before mapping parameters");
			}
			ParameterMap = mapping.ToDictionary((KeyValuePair<ParameterInfo, MemberInfo> kvp) => kvp.Key, (KeyValuePair<ParameterInfo, MemberInfo> kvp) => kvp.Value);
			return this;
		}

		public TypeConfig<T> SetFormatter(IFormatter<T> formatterInstance)
		{
			CustomFormatter = formatterInstance;
			return this;
		}

		public TypeConfig<T> SetReadonlyHandling(ReadonlyFieldHandling mode)
		{
			_readonlyOverride = mode;
			return this;
		}

		public TypeConfig<T> SetTargetMembers(TargetMember targets)
		{
			_targetMembers = targets;
			return this;
		}

		public MemberConfig<T> ConfigMember<TMember>(Expression<Func<T, TMember>> selectMemberExpression)
		{
			MemberInfo memberInfo = ((MemberExpression)selectMemberExpression.Body).Member;
			return (MemberConfig<T>)base.Members.FirstOrDefault((MemberConfig m) => m.Member == memberInfo);
		}

		public MemberConfig<T> ConfigField(string fieldName)
		{
			return (MemberConfig<T>)base.Members.FirstOrDefault((MemberConfig m) => m.Member is FieldInfo && m.Member.Name == fieldName);
		}

		public MemberConfig<T> ConfigProperty(string propName)
		{
			return (MemberConfig<T>)base.Members.FirstOrDefault((MemberConfig m) => m.Member is PropertyInfo && m.Member.Name == propName);
		}
	}
}
