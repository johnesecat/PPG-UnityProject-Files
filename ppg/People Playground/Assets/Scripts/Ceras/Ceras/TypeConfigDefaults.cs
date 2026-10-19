using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Ceras.Exceptions;
using Ceras.Formatters;
using Ceras.Helpers;
using Ceras.Resolvers;

namespace Ceras
{
	internal static class TypeConfigDefaults
	{
		private const BindingFlags BindingFlagsStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		private const BindingFlags BindingFlagsCtor = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance;

		internal static void ApplyTypeAttributes(TypeConfig typeConfig)
		{
			Type type = typeConfig.Type;
			ApplyConstructorDefaults(typeConfig);
			MemberConfigAttribute customAttribute = type.GetCustomAttribute<MemberConfigAttribute>();
			if (customAttribute != null)
			{
				typeConfig.ReadonlyFieldOverride = customAttribute.ReadonlyFieldHandling;
				typeConfig.TargetMembers = customAttribute.TargetMembers;
			}
		}

		private static void ApplyConstructorDefaults(TypeConfig typeConfig)
		{
			if (typeConfig.TypeConstruction != null)
			{
				return;
			}
			if (typeConfig.TypeConstruction == null && (CerasSerializer.IsFormatterConstructed(typeConfig.Type) || typeConfig.Type.IsStatic()))
			{
				typeConfig.TypeConstruction = TypeConstruction.Null();
				return;
			}
			Type type = typeConfig.Type;
			MethodBase[] array = (from m in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Cast<MethodBase>().Concat(type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance))
				where m.GetCustomAttribute<CerasConstructorAttribute>() != null
				select m).ToArray();
			if (array.Length > 1)
			{
				throw new InvalidConfigException("There are multiple constructors on your type '" + typeConfig.Type.FriendlyName() + "' that have the '[CerasConstructor]' attribute, so its unclear which one to use. Only one constructor in this type can have the attribute.");
			}
			MethodBase methodBase = ((array.Length != 1) ? type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance).FirstOrDefault((ConstructorInfo c) => c.GetParameters().Length == 0) : array[0]);
			if (methodBase != null)
			{
				ConstructorInfo constructorInfo = methodBase as ConstructorInfo;
				if ((object)constructorInfo != null)
				{
					typeConfig.TypeConstruction = TypeConstruction.ByConstructor(constructorInfo);
				}
				else
				{
					MethodInfo methodInfo = methodBase as MethodInfo;
					if ((object)methodInfo != null)
					{
						typeConfig.TypeConstruction = TypeConstruction.ByStaticMethod(methodInfo);
					}
				}
			}
			if (type.IsValueType && typeConfig.TypeConstruction == null)
			{
				typeConfig.TypeConstruction = ConstructNull.Instance;
			}
		}

		internal static void ApplyMemberAttributes(MemberConfig memberConfig)
		{
			Type type = memberConfig.TypeConfig.Type;
			MemberInfo member = memberConfig.Member;
			DataMemberAttribute customAttribute = member.GetCustomAttribute<DataMemberAttribute>();
			if (customAttribute != null)
			{
				memberConfig.WriteBackOrder = customAttribute.Order;
			}
			if (memberConfig.IsCompilerGenerated && memberConfig.TypeConfig.Config.Advanced.SkipCompilerGeneratedFields)
			{
				memberConfig.ExcludeWithReason("Compiler generated field");
				return;
			}
			FieldInfo fieldInfo = member as FieldInfo;
			if ((object)fieldInfo != null)
			{
				ReadonlyConfigAttribute customAttribute2 = fieldInfo.GetCustomAttribute<ReadonlyConfigAttribute>(inherit: true);
				if (customAttribute2 != null)
				{
					memberConfig.ReadonlyFieldHandling = customAttribute2.ReadonlyFieldHandling;
				}
			}
			else
			{
				if (!(member is PropertyInfo))
				{
					throw new InvalidOperationException(member.Name + " must be a field or a property");
				}
				if (ShouldSkipProperty(memberConfig))
				{
					return;
				}
			}
			bool flag = member.GetCustomAttribute<ExcludeAttribute>(inherit: true) != null;
			bool flag2 = member.GetCustomAttribute<IncludeAttribute>(inherit: true) != null;
			bool flag3 = member.GetCustomAttribute<NonSerializedAttribute>(inherit: true) != null;
			if (flag2 && (flag || flag3))
			{
				throw new Exception("Member '" + member.Name + "' on type '" + type.FriendlyName() + "' has both [Include] and [Exclude] (or [NonSerialized]) !");
			}
			if (flag)
			{
				memberConfig.ExcludeWithReason("[Exclude] attribute");
			}
			if (flag3)
			{
				memberConfig.HasNonSerialized = true;
			}
			if (flag2)
			{
				memberConfig.SetIncludeWithReason(SerializationOverride.ForceInclude, "[Include] attribute on member");
			}
			PreviousNameAttribute customAttribute3 = member.GetCustomAttribute<PreviousNameAttribute>();
			if (customAttribute3 != null)
			{
				memberConfig.PersistentName = customAttribute3.Name;
			}
		}

		private static bool ShouldSkipProperty(MemberConfig m)
		{
			if (((PropertyInfo)m.Member).GetAccessors(nonPublic: true).Length <= 1)
			{
				m.ExcludeWithReason("Computed Property (has no 'set' function, not even a private one)");
				return true;
			}
			return false;
		}

		internal static void ApplySpecializedDefaults(TypeConfig typeConfig)
		{
			Type type = typeConfig.Type;
			if (!type.IsPrimitive && type.IsValueType)
			{
				if (typeConfig.TypeConstruction == null)
				{
					typeConfig.TypeConstruction = ConstructNull.Instance;
				}
				typeConfig.ReadonlyFieldOverride = ReadonlyFieldHandling.ForcedOverwrite;
				return;
			}
			if (type.Assembly == typeof(Expression).Assembly)
			{
				if (!type.IsAbstract && type.IsSubclassOf(typeof(Expression)))
				{
					ForceSerialization(typeConfig);
					return;
				}
				if (type.FullName.StartsWith("System.Runtime.CompilerServices.TrueReadOnlyCollection"))
				{
					ForceSerialization(typeConfig);
					return;
				}
			}
			if (type.Assembly == typeof(ReadOnlyCollection<>).Assembly && type.FullName.StartsWith("System.Collections.ObjectModel.ReadOnlyCollection"))
			{
				ForceSerialization(typeConfig);
			}
		}

		private static void ForceSerialization(TypeConfig typeConfig)
		{
			typeConfig.TypeConstruction = TypeConstruction.ByUninitialized();
			typeConfig.ReadonlyFieldOverride = ReadonlyFieldHandling.ForcedOverwrite;
			typeConfig.TargetMembers = TargetMember.PrivateFields;
			typeConfig.CustomResolver = ForceDynamicResolver;
		}

		private static Ceras.Formatters.IFormatter ForceDynamicResolver(CerasSerializer ceras, Type type)
		{
			return ((ICerasAdvanced)ceras).GetFormatterResolver<DynamicObjectFormatterResolver>().GetFormatter(type);
		}
	}
}
