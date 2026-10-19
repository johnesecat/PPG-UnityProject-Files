using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public class ReflectionHelper
	{
		public static Type GetType(string fullTypeName)
		{
			return GetAllTypes().FirstOrDefault((Type t) => t.FullName == fullTypeName);
		}

		public static List<Type> GetAllInstantiableTypesDerivedFrom(Type type, List<Type> except = null)
		{
			List<Type> list = (from t in GetAllTypes()
				where t.BaseType != null && ((t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == type) || (t.IsSubclassOf(type) && t != type && !t.IsAbstract))
				select t).Distinct().ToList();
			if (except == null)
			{
				return list;
			}
			return list.Except(except).ToList();
		}

		public static List<Type> GetAllTypes()
		{
			return AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly s) => s.GetTypes()).ToList();
		}

		public static FieldInfo GetFieldInfoIncludingBaseClasses(Type type, string name, BindingFlags bindingFlags)
		{
			if (type.BaseType == typeof(object))
			{
				return type.GetField(name, bindingFlags);
			}
			FieldInfo field = type.GetField(name, bindingFlags);
			if (field == null)
			{
				return GetFieldInfoIncludingBaseClasses(type.BaseType, name, bindingFlags);
			}
			return field;
		}
	}
}
