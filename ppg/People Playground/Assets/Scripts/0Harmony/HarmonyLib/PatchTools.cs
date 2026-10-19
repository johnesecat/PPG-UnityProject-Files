using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace HarmonyLib
{
	internal static class PatchTools
	{
		[ThreadStatic]
		private static Dictionary<object, object> objectReferences;

		internal static void RememberObject(object key, object value)
		{
			if (objectReferences == null)
			{
				objectReferences = new Dictionary<object, object>();
			}
			objectReferences[key] = value;
		}

		internal static MethodInfo GetPatchMethod(Type patchType, string attributeName)
		{
			MethodInfo methodInfo = patchType.GetMethods(AccessTools.all).FirstOrDefault((MethodInfo m) => m.GetCustomAttributes(inherit: true).Any((object a) => a.GetType().FullName == attributeName));
			if ((object)methodInfo == null)
			{
				string name = attributeName.Replace("HarmonyLib.Harmony", "");
				methodInfo = patchType.GetMethod(name, AccessTools.all);
			}
			return methodInfo;
		}

		internal static AssemblyBuilder DefineDynamicAssembly(string name)
		{
			AssemblyName name2 = new AssemblyName(name);
			return AppDomain.CurrentDomain.DefineDynamicAssembly(name2, AssemblyBuilderAccess.Run);
		}

		internal static List<AttributePatch> GetPatchMethods(Type type)
		{
			return (from method in AccessTools.GetDeclaredMethods(type)
				select AttributePatch.Create(method) into attributePatch
				where attributePatch != null
				select attributePatch).ToList();
		}

		internal static MethodBase GetOriginalMethod(this HarmonyMethod attr)
		{
			try
			{
				MethodType? methodType = attr.methodType;
				if (methodType.HasValue)
				{
					switch (methodType.GetValueOrDefault())
					{
					case MethodType.Normal:
						if (attr.methodName == null)
						{
							return null;
						}
						return AccessTools.DeclaredMethod(attr.declaringType, attr.methodName, attr.argumentTypes);
					case MethodType.Getter:
						if (attr.methodName == null)
						{
							return null;
						}
						return AccessTools.DeclaredProperty(attr.declaringType, attr.methodName).GetGetMethod(nonPublic: true);
					case MethodType.Setter:
						if (attr.methodName == null)
						{
							return null;
						}
						return AccessTools.DeclaredProperty(attr.declaringType, attr.methodName).GetSetMethod(nonPublic: true);
					case MethodType.Constructor:
						return AccessTools.DeclaredConstructor(attr.declaringType, attr.argumentTypes);
					case MethodType.StaticConstructor:
						return (from c in AccessTools.GetDeclaredConstructors(attr.declaringType)
							where c.IsStatic
							select c).FirstOrDefault();
					case MethodType.Enumerator:
						if (attr.methodName == null)
						{
							return null;
						}
						return AccessTools.EnumeratorMoveNext(AccessTools.DeclaredMethod(attr.declaringType, attr.methodName, attr.argumentTypes));
					}
				}
			}
			catch (AmbiguousMatchException ex)
			{
				throw new HarmonyException("Ambiguous match for HarmonyMethod[" + attr.Description() + "]", ex.InnerException ?? ex);
			}
			return null;
		}
	}
}
