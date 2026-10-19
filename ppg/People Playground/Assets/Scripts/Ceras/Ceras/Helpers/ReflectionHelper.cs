using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ceras.Helpers
{
	internal static class ReflectionHelper
	{
		private static readonly Dictionary<Type, int> _typeToBlittableSize = new Dictionary<Type, int>();

		private static readonly Dictionary<Type, int> _typeToUnsafeSize = new Dictionary<Type, int>();

		public static Type FindClosedType(Type type, Type openGeneric)
		{
			if (openGeneric.IsInterface)
			{
				Type[] array = type.FindInterfaces((Type f, object o) => f.IsGenericType && f.GetGenericTypeDefinition() == openGeneric, null);
				if (type.IsGenericType && type.GetGenericTypeDefinition() == openGeneric)
				{
					return type;
				}
				if (array.Length != 0)
				{
					return array[0];
				}
			}
			else
			{
				Type type2 = type;
				while (type2 != null)
				{
					if (type2.IsGenericType && type2.GetGenericTypeDefinition() == openGeneric)
					{
						return type2;
					}
					type2 = type2.BaseType;
				}
			}
			return null;
		}

		public static Type FindClosedArg(this Type objectType, Type openGeneric, int argIndex = 0)
		{
			Type type = FindClosedType(objectType, openGeneric);
			if (type == null)
			{
				return null;
			}
			return type.GetGenericArguments()[argIndex];
		}

		public static bool IsAssignableToGenericType(Type givenType, Type genericType)
		{
			if (genericType.IsAssignableFrom(givenType))
			{
				return true;
			}
			Type[] interfaces = givenType.GetInterfaces();
			foreach (Type type in interfaces)
			{
				if (type == genericType)
				{
					return true;
				}
				if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
				{
					return true;
				}
			}
			if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
			{
				return true;
			}
			Type baseType = givenType.BaseType;
			if (baseType == null)
			{
				return false;
			}
			return IsAssignableToGenericType(baseType, genericType);
		}

		public static IEnumerable<MemberInfo> GetAllDataMembers(this Type type, bool fields = true, bool properties = true)
		{
			if (type.IsPrimitive)
			{
				yield break;
			}
			foreach (MemberInfo i in type.EnumerateMembers())
			{
				FieldInfo fieldInfo = i as FieldInfo;
				if ((object)fieldInfo != null && fields && !fieldInfo.IsStatic)
				{
					yield return i;
				}
				PropertyInfo propertyInfo = i as PropertyInfo;
				if ((object)propertyInfo != null && properties && !propertyInfo.GetAccessors(nonPublic: true)[0].IsStatic)
				{
					yield return i;
				}
			}
		}

		public static IEnumerable<MemberInfo> GetAllStaticDataMembers(this Type type, bool fields = true, bool properties = true)
		{
			if (type.IsPrimitive)
			{
				yield break;
			}
			foreach (MemberInfo i in type.EnumerateMembers())
			{
				FieldInfo fieldInfo = i as FieldInfo;
				if ((object)fieldInfo != null && fields && fieldInfo.IsStatic)
				{
					yield return i;
				}
				PropertyInfo propertyInfo = i as PropertyInfo;
				if ((object)propertyInfo != null && properties && propertyInfo.GetAccessors(nonPublic: true)[0].IsStatic)
				{
					yield return i;
				}
			}
		}

		private static IEnumerable<MemberInfo> EnumerateMembers(this Type type)
		{
			BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			while (type != null)
			{
				FieldInfo[] fields = type.GetFields(flags);
				foreach (FieldInfo fieldInfo in fields)
				{
					if (fieldInfo.DeclaringType == type)
					{
						yield return fieldInfo;
					}
				}
				PropertyInfo[] properties = type.GetProperties(flags);
				foreach (PropertyInfo propertyInfo in properties)
				{
					if (propertyInfo.DeclaringType == type)
					{
						MethodInfo getMethod = propertyInfo.GetGetMethod(nonPublic: true);
						if (!(getMethod != null) || getMethod.GetParameters().Length == 0)
						{
							yield return propertyInfo;
						}
					}
				}
				type = type.BaseType;
			}
		}

		internal static bool ComputeExpectedSize(Type type, out int size)
		{
			size = -1;
			_ = type.FullName;
			if (!type.IsValueType)
			{
				return false;
			}
			if (type.ContainsGenericParameters)
			{
				return false;
			}
			if (type.IsPointer || type == typeof(IntPtr) || type == typeof(UIntPtr))
			{
				return false;
			}
			if (type.IsPrimitive)
			{
				size = Marshal.SizeOf(type);
				return true;
			}
			if (type.DeclaringType != null && type.DeclaringType.IsValueType)
			{
				FixedBufferAttribute customAttribute = type.DeclaringType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Single((FieldInfo f) => f.FieldType == type).GetCustomAttribute<FixedBufferAttribute>();
				if (customAttribute != null)
				{
					if (!ComputeExpectedSize(customAttribute.ElementType, out var size2))
					{
						throw new Exception();
					}
					size = customAttribute.Length * size2;
					return true;
				}
			}
			if (type.IsAutoLayout)
			{
				return false;
			}
			StructLayoutAttribute structLayoutAttribute = type.StructLayoutAttribute;
			if (structLayoutAttribute == null)
			{
				throw new Exception("Type '" + type.FriendlyName(fullName: true) + "' is a value-type but does not have a StructLayoutAttribute!");
			}
			size = 0;
			foreach (FieldInfo item in type.GetAllDataMembers(fields: true, properties: false).Cast<FieldInfo>())
			{
				FixedBufferAttribute customAttribute2 = item.GetCustomAttribute<FixedBufferAttribute>();
				if (customAttribute2 != null)
				{
					if (!ComputeExpectedSize(customAttribute2.ElementType, out var size3))
					{
						throw new InvalidOperationException();
					}
					int num = customAttribute2.Length * size3;
					size += num;
				}
				else
				{
					if (!ComputeExpectedSize(item.FieldType, out var size4))
					{
						size = -1;
						return false;
					}
					size += size4;
				}
			}
			if (structLayoutAttribute.Size != 0 && structLayoutAttribute.Size != size)
			{
				throw new Exception("Computed size of '" + type.FriendlyName(fullName: true) + "' did not match the StructLayout value");
			}
			int num2 = Marshal.SizeOf(type);
			if (size != num2)
			{
				throw new Exception("Computed size of '" + type.FriendlyName(fullName: true) + "' does not match marshal size");
			}
			return true;
		}

		public static bool IsBlittableType(Type type)
		{
			if (!type.IsValueType)
			{
				return false;
			}
			if (type.IsEnum)
			{
				return true;
			}
			if (type.IsPointer || type == typeof(IntPtr) || type == typeof(UIntPtr))
			{
				return false;
			}
			if (type.IsPrimitive)
			{
				return true;
			}
			if (type.ContainsGenericParameters)
			{
				return false;
			}
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				return false;
			}
			if (type.IsAutoLayout)
			{
				return false;
			}
			foreach (FieldInfo item in type.GetAllDataMembers(fields: true, properties: false).Cast<FieldInfo>())
			{
				if (item.GetCustomAttribute<FixedBufferAttribute>() == null)
				{
					if (item.GetCustomAttribute<MarshalAsAttribute>() != null)
					{
						throw new NotSupportedException("The [MarshalAs] attribute is not supported");
					}
					if (!IsBlittableType(item.FieldType))
					{
						return false;
					}
				}
			}
			return true;
		}

		public static int GetSize(Type type)
		{
			if (!IsBlittableType(type))
			{
				return -1;
			}
			lock (_typeToBlittableSize)
			{
				if (_typeToBlittableSize.TryGetValue(type, out var value))
				{
					return value;
				}
				value = (type.IsGenericType ? Marshal.SizeOf(Activator.CreateInstance(type)) : Marshal.SizeOf(type));
				_typeToBlittableSize.Add(type, value);
				return value;
			}
		}

		public static int UnsafeGetSize(Type type)
		{
			lock (_typeToUnsafeSize)
			{
				if (_typeToUnsafeSize.TryGetValue(type, out var value))
				{
					return value;
				}
				value = (int)typeof(Unsafe).GetMethod("SizeOf").MakeGenericMethod(type).Invoke(null, null);
				_typeToUnsafeSize.Add(type, value);
				return value;
			}
		}

		public static Type FieldOrPropType(this MemberInfo memberInfo)
		{
			FieldInfo fieldInfo = memberInfo as FieldInfo;
			if ((object)fieldInfo != null)
			{
				return fieldInfo.FieldType;
			}
			PropertyInfo propertyInfo = memberInfo as PropertyInfo;
			if ((object)propertyInfo != null)
			{
				return propertyInfo.PropertyType;
			}
			throw new InvalidOperationException();
		}

		public static MethodInfo ResolveMethod(Type declaringType, string name, Type[] parameters)
		{
			return SelectMethod((from m in declaringType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				where m.Name == name && m.GetParameters().Length == parameters.Length
				select m).ToArray(), parameters);
		}

		private static MethodInfo SelectMethod(MethodInfo[] methods, Type[] specificArguments)
		{
			List<MethodInfo> list = new List<MethodInfo>();
			foreach (MethodInfo methodInfo in methods)
			{
				if (methodInfo.IsGenericMethod)
				{
					MethodInfo methodInfo2 = TryCloseOpenGeneric(methodInfo, specificArguments);
					if (methodInfo2 != null)
					{
						list.Add(methodInfo2);
					}
					continue;
				}
				ParameterInfo[] parameters = methodInfo.GetParameters();
				bool flag = true;
				for (int j = 0; j < specificArguments.Length; j++)
				{
					Type parameterType = parameters[j].ParameterType;
					Type type = specificArguments[j];
					if (parameterType != type)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					list.Add(methodInfo);
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
			if (list.Count > 1)
			{
				throw new AmbiguousMatchException("The given parameters can match more than one method overload.");
			}
			return list[0];
		}

		public static MethodInfo TryCloseOpenGeneric(MethodInfo openGenericMethod, Type[] specificArguments)
		{
			if (!openGenericMethod.IsGenericMethodDefinition)
			{
				throw new ArgumentException("'openGenericMethod' must be a generic method definition");
			}
			foreach (Type type in specificArguments)
			{
				if (type.ContainsGenericParameters)
				{
					throw new InvalidOperationException($"Can't close open generic method '{openGenericMethod}' At least one of the given argument types is not fully closed: '{type.FullName}'");
				}
			}
			ParameterInfo[] parameters = openGenericMethod.GetParameters();
			Dictionary<Type, Type> genArgToConcreteType = new Dictionary<Type, Type>();
			Type[] genericArguments = openGenericMethod.GetGenericArguments();
			for (int j = 0; j < parameters.Length; j++)
			{
				ParameterInfo obj = parameters[j];
				if (!IsParameterMatch(arg: specificArguments[j], parameterType: obj.ParameterType, genArgToConcreteType: genArgToConcreteType, methodGenericArgs: genericArguments, mustMatchExactly: true))
				{
					return null;
				}
			}
			Type[] typeArguments = (from g in genericArguments
				orderby g.GenericParameterPosition
				select genArgToConcreteType[g]).ToArray();
			try
			{
				MethodInfo methodInfo = openGenericMethod.MakeGenericMethod(typeArguments);
				ParameterInfo[] parameters2 = methodInfo.GetParameters();
				for (int k = 0; k < parameters2.Length; k++)
				{
					if (parameters2[k].ParameterType != specificArguments[k])
					{
						return null;
					}
				}
				return methodInfo;
			}
			catch
			{
				return null;
			}
		}

		private static bool IsParameterMatch(Type parameterType, Type arg, Dictionary<Type, Type> genArgToConcreteType, Type[] methodGenericArgs, bool mustMatchExactly)
		{
			if (mustMatchExactly)
			{
				if (parameterType == arg)
				{
					return true;
				}
			}
			else if (parameterType.IsAssignableFrom(arg))
			{
				return true;
			}
			Type type = methodGenericArgs.FirstOrDefault((Type g) => g == parameterType);
			if (type != null)
			{
				if (genArgToConcreteType.TryGetValue(type, out var value))
				{
					if (value != arg)
					{
						return false;
					}
				}
				else
				{
					genArgToConcreteType.Add(type, arg);
				}
				return true;
			}
			if (parameterType.IsGenericType && arg.IsGenericType)
			{
				Type genericTypeDefinition = parameterType.GetGenericTypeDefinition();
				Type genericTypeDefinition2 = arg.GetGenericTypeDefinition();
				if (genericTypeDefinition != genericTypeDefinition2)
				{
					return false;
				}
				Type[] genericArguments = arg.GetGenericArguments();
				Type[] genericArguments2 = parameterType.GetGenericArguments();
				for (int i = 0; i < genericArguments2.Length; i++)
				{
					if (!IsParameterMatch(genericArguments2[i], genericArguments[i], genArgToConcreteType, methodGenericArgs, mustMatchExactly))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		internal static MethodInfo GetMethod(Expression<Action> e)
		{
			MethodCallExpression methodCallExpression = e.Body as MethodCallExpression;
			if (methodCallExpression != null)
			{
				return methodCallExpression.Method;
			}
			throw new ArgumentException();
		}

		internal static MethodInfo GetMethod<T>(Expression<Func<T>> e)
		{
			MethodCallExpression methodCallExpression = e.Body as MethodCallExpression;
			if (methodCallExpression != null)
			{
				return methodCallExpression.Method;
			}
			throw new ArgumentException();
		}

		internal static bool IsStatic(this Type type)
		{
			if (type.IsAbstract)
			{
				return type.IsSealed;
			}
			return false;
		}

		internal static bool IsAbstract(this Type type)
		{
			if (type.IsAbstract)
			{
				return !type.IsSealed;
			}
			return false;
		}

		public static string FriendlyName(this Type type, bool fullName = false)
		{
			if (type == typeof(int))
			{
				return "int";
			}
			if (type == typeof(short))
			{
				return "short";
			}
			if (type == typeof(byte))
			{
				return "byte";
			}
			if (type == typeof(bool))
			{
				return "bool";
			}
			if (type == typeof(long))
			{
				return "long";
			}
			if (type == typeof(float))
			{
				return "float";
			}
			if (type == typeof(double))
			{
				return "double";
			}
			if (type == typeof(decimal))
			{
				return "decimal";
			}
			if (type == typeof(string))
			{
				return "string";
			}
			if (type.IsGenericType)
			{
				return (fullName ? type.FullName : type.Name).Split('`')[0] + "<" + string.Join(", ", (from t in type.GetGenericArguments()
					select t.FriendlyName(fullName)).ToArray()) + ">";
			}
			if (!fullName)
			{
				return type.Name;
			}
			return type.FullName;
		}
	}
}
