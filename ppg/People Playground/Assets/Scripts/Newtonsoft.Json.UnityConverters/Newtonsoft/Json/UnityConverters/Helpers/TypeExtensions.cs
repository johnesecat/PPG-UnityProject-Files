using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Newtonsoft.Json.UnityConverters.Helpers
{
	internal static class TypeExtensions
	{
		public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
		{
			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				return ex.Types.Where((Type t) => t != null);
			}
		}

		public static FieldInfo GetFieldInfoOrThrow(this Type type, string name)
		{
			return type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new MissingMemberException(type.FullName, name);
		}

		public static void SetValueDirectRef<T1, T2>(this FieldInfo field, ref T1 state, T2 value)
		{
			TypedReference obj = __makeref(state);
			field.SetValueDirect(obj, value);
		}
	}
}
