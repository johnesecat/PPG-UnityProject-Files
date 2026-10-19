using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace HarmonyLib
{
	internal static class HarmonySharedState
	{
		private const string name = "HarmonySharedState";

		internal const int internalVersion = 102;

		private static readonly Dictionary<MethodBase, byte[]> state;

		private static readonly Dictionary<MethodInfo, MethodBase> originals;

		private static readonly Dictionary<long, MethodInfo> methodStarts;

		private static bool methodStartsInvalidated;

		internal static readonly int actualVersion;

		private static readonly FieldInfo methodAddress;

		static HarmonySharedState()
		{
			methodAddress = typeof(StackFrame).GetField("methodAddress", BindingFlags.Instance | BindingFlags.NonPublic);
			Type orCreateSharedStateType = GetOrCreateSharedStateType();
			FieldInfo field = orCreateSharedStateType.GetField("version");
			if ((int)field.GetValue(null) == 0)
			{
				field.SetValue(null, 102);
			}
			actualVersion = (int)field.GetValue(null);
			FieldInfo field2 = orCreateSharedStateType.GetField("state");
			if (field2.GetValue(null) == null)
			{
				field2.SetValue(null, new Dictionary<MethodBase, byte[]>());
			}
			FieldInfo field3 = orCreateSharedStateType.GetField("originals");
			if (field3 != null && field3.GetValue(null) == null)
			{
				field3.SetValue(null, new Dictionary<MethodInfo, MethodBase>());
			}
			state = (Dictionary<MethodBase, byte[]>)field2.GetValue(null);
			originals = new Dictionary<MethodInfo, MethodBase>();
			if (field3 != null)
			{
				originals = (Dictionary<MethodInfo, MethodBase>)field3.GetValue(null);
			}
			methodStarts = new Dictionary<long, MethodInfo>();
			RefreshMethodStarts();
			DetourHelper.Runtime.OnMethodCompiled += delegate(MethodBase method, IntPtr codeStart, ulong codeLen)
			{
				if (!(method == null))
				{
					PatchInfo patchInfo = GetPatchInfo(method);
					if (patchInfo != null)
					{
						PatchFunctions.UpdateRecompiledMethod(method, codeStart, patchInfo);
						methodStartsInvalidated = true;
					}
				}
			};
		}

		private static void RefreshMethodStarts()
		{
			lock (originals)
			{
				methodStarts.Clear();
				foreach (MethodInfo key in originals.Keys)
				{
					methodStarts.Add(key.GetNativeStart().ToInt64(), key);
				}
			}
			methodStartsInvalidated = false;
		}

		private static Type GetOrCreateSharedStateType()
		{
			Type type = Type.GetType("HarmonySharedState", throwOnError: false);
			if (type != null)
			{
				return type;
			}
			using (ModuleDefinition moduleDefinition = ModuleDefinition.CreateModule("HarmonySharedState", new ModuleParameters
			{
				Kind = ModuleKind.Dll,
				ReflectionImporterProvider = MMReflectionImporter.Provider
			}))
			{
				Mono.Cecil.TypeAttributes attributes = Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.Abstract | Mono.Cecil.TypeAttributes.Sealed;
				TypeDefinition typeDefinition = new TypeDefinition("", "HarmonySharedState", attributes)
				{
					BaseType = moduleDefinition.TypeSystem.Object
				};
				moduleDefinition.Types.Add(typeDefinition);
				typeDefinition.Fields.Add(new FieldDefinition("state", Mono.Cecil.FieldAttributes.Public | Mono.Cecil.FieldAttributes.Static, moduleDefinition.ImportReference(typeof(Dictionary<MethodBase, byte[]>))));
				typeDefinition.Fields.Add(new FieldDefinition("originals", Mono.Cecil.FieldAttributes.Public | Mono.Cecil.FieldAttributes.Static, moduleDefinition.ImportReference(typeof(Dictionary<MethodInfo, MethodBase>))));
				typeDefinition.Fields.Add(new FieldDefinition("version", Mono.Cecil.FieldAttributes.Public | Mono.Cecil.FieldAttributes.Static, moduleDefinition.ImportReference(typeof(int))));
				return ReflectionHelper.Load(moduleDefinition).GetType("HarmonySharedState");
			}
		}

		internal static PatchInfo GetPatchInfo(MethodBase method)
		{
			byte[] valueSafe;
			lock (state)
			{
				valueSafe = state.GetValueSafe(method);
			}
			if (valueSafe == null)
			{
				return null;
			}
			return PatchInfoSerialization.Deserialize(valueSafe);
		}

		internal static IEnumerable<MethodBase> GetPatchedMethods()
		{
			lock (state)
			{
				return state.Keys.ToArray();
			}
		}

		internal static void UpdatePatchInfo(MethodBase original, MethodInfo replacement, PatchInfo patchInfo)
		{
			byte[] value = patchInfo.Serialize();
			lock (state)
			{
				state[original] = value;
			}
			lock (originals)
			{
				originals[replacement] = original;
			}
			lock (methodStarts)
			{
				methodStarts[replacement.GetNativeStart().ToInt64()] = replacement;
			}
		}

		internal static MethodBase GetOriginal(MethodInfo replacement)
		{
			lock (originals)
			{
				return originals.GetValueSafe(replacement);
			}
		}

		internal static MethodBase FindReplacement(StackFrame frame)
		{
			MethodBase method = frame.GetMethod();
			long num = 0L;
			if ((object)method == null || method.IsGenericMethod)
			{
				if (methodAddress == null)
				{
					return null;
				}
				num = (long)methodAddress.GetValue(frame);
			}
			else
			{
				num = DetourHelper.Runtime.GetIdentifiable(method).GetNativeStart().ToInt64();
			}
			if (num == 0L)
			{
				return method;
			}
			lock (methodStarts)
			{
				if (methodStartsInvalidated)
				{
					RefreshMethodStarts();
				}
				MethodInfo value;
				return methodStarts.TryGetValue(num, out value) ? value : method;
			}
		}
	}
}
