using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HarmonyLib
{
	internal class AttributePatch
	{
		private static readonly HarmonyPatchType[] allPatchTypes = new HarmonyPatchType[5]
		{
			HarmonyPatchType.Prefix,
			HarmonyPatchType.Postfix,
			HarmonyPatchType.Transpiler,
			HarmonyPatchType.Finalizer,
			HarmonyPatchType.ReversePatch
		};

		internal HarmonyMethod info;

		internal HarmonyPatchType? type;

		private static readonly string harmonyAttributeName = typeof(HarmonyAttribute).FullName;

		internal static AttributePatch Create(MethodInfo patch)
		{
			if ((object)patch == null)
			{
				throw new NullReferenceException("Patch method cannot be null");
			}
			object[] customAttributes = patch.GetCustomAttributes(inherit: true);
			HarmonyPatchType? patchType = GetPatchType(patch.Name, customAttributes);
			if (!patchType.HasValue)
			{
				return null;
			}
			if (patchType != HarmonyPatchType.ReversePatch && !patch.IsStatic)
			{
				throw new ArgumentException("Patch method " + patch.FullDescription() + " must be static");
			}
			HarmonyMethod harmonyMethod = HarmonyMethod.Merge((from attr in customAttributes
				where attr.GetType().BaseType.FullName == harmonyAttributeName
				select AccessTools.Field(attr.GetType(), "info").GetValue(attr) into harmonyInfo
				select AccessTools.MakeDeepCopy<HarmonyMethod>(harmonyInfo)).ToList());
			harmonyMethod.method = patch;
			return new AttributePatch
			{
				info = harmonyMethod,
				type = patchType
			};
		}

		private static HarmonyPatchType? GetPatchType(string methodName, object[] allAttributes)
		{
			HashSet<string> hashSet = new HashSet<string>(from attr in allAttributes
				select attr.GetType().FullName into name
				where name.StartsWith("Harmony")
				select name);
			HarmonyPatchType? result = null;
			HarmonyPatchType[] array = allPatchTypes;
			for (int i = 0; i < array.Length; i++)
			{
				HarmonyPatchType value = array[i];
				string text = value.ToString();
				if (text == methodName || hashSet.Contains("HarmonyLib.Harmony" + text))
				{
					result = value;
					break;
				}
			}
			return result;
		}
	}
}
