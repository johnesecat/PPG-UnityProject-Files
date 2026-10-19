using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HarmonyLib
{
	internal static class PatchFunctions
	{
		internal static List<MethodInfo> GetSortedPatchMethods(MethodBase original, Patch[] patches, bool debug)
		{
			return new PatchSorter(patches, debug).Sort(original);
		}

		internal static MethodInfo UpdateWrapper(MethodBase original, PatchInfo patchInfo)
		{
			bool debug = patchInfo.Debugging || Harmony.DEBUG;
			List<MethodInfo> sortedPatchMethods = GetSortedPatchMethods(original, patchInfo.prefixes, debug);
			List<MethodInfo> sortedPatchMethods2 = GetSortedPatchMethods(original, patchInfo.postfixes, debug);
			List<MethodInfo> sortedPatchMethods3 = GetSortedPatchMethods(original, patchInfo.transpilers, debug);
			List<MethodInfo> sortedPatchMethods4 = GetSortedPatchMethods(original, patchInfo.finalizers, debug);
			Dictionary<int, CodeInstruction> finalInstructions;
			MethodInfo methodInfo = new MethodPatcher(original, null, sortedPatchMethods, sortedPatchMethods2, sortedPatchMethods3, sortedPatchMethods4, debug).CreateReplacement(out finalInstructions);
			if ((object)methodInfo == null)
			{
				throw new MissingMethodException("Cannot create replacement for " + original.FullDescription());
			}
			try
			{
				Memory.DetourMethodAndPersist(original, methodInfo);
				return methodInfo;
			}
			catch (Exception ex)
			{
				throw HarmonyException.Create(ex, finalInstructions);
			}
		}

		internal static void UpdateRecompiledMethod(MethodBase original, IntPtr codeStart, PatchInfo patchInfo)
		{
			try
			{
				List<MethodInfo> sortedPatchMethods = GetSortedPatchMethods(original, patchInfo.prefixes, debug: false);
				List<MethodInfo> sortedPatchMethods2 = GetSortedPatchMethods(original, patchInfo.postfixes, debug: false);
				List<MethodInfo> sortedPatchMethods3 = GetSortedPatchMethods(original, patchInfo.transpilers, debug: false);
				List<MethodInfo> sortedPatchMethods4 = GetSortedPatchMethods(original, patchInfo.finalizers, debug: false);
				Dictionary<int, CodeInstruction> finalInstructions;
				MethodInfo methodInfo = new MethodPatcher(original, null, sortedPatchMethods, sortedPatchMethods2, sortedPatchMethods3, sortedPatchMethods4, debug: false).CreateReplacement(out finalInstructions);
				if ((object)methodInfo == null)
				{
					throw new MissingMethodException("Cannot create replacement for " + original.FullDescription());
				}
				Memory.DetourCompiledMethod(codeStart, methodInfo);
			}
			catch
			{
			}
		}

		internal static MethodInfo ReversePatch(HarmonyMethod standin, MethodBase original, MethodInfo postTranspiler)
		{
			if (standin == null)
			{
				throw new ArgumentNullException("standin");
			}
			if ((object)standin.method == null)
			{
				throw new ArgumentNullException("standin", "standin.method is NULL");
			}
			bool debug = standin.debug.GetValueOrDefault() || Harmony.DEBUG;
			List<MethodInfo> list = new List<MethodInfo>();
			if (standin.reversePatchType == HarmonyReversePatchType.Snapshot)
			{
				Patches patchInfo = Harmony.GetPatchInfo(original);
				list.AddRange(GetSortedPatchMethods(original, patchInfo.Transpilers.ToArray(), debug));
			}
			if ((object)postTranspiler != null)
			{
				list.Add(postTranspiler);
			}
			List<MethodInfo> list2 = new List<MethodInfo>();
			Dictionary<int, CodeInstruction> finalInstructions;
			MethodInfo methodInfo = new MethodPatcher(standin.method, original, list2, list2, list, list2, debug).CreateReplacement(out finalInstructions);
			if ((object)methodInfo == null)
			{
				throw new MissingMethodException("Cannot create replacement for " + standin.method.FullDescription());
			}
			try
			{
				string text = Memory.DetourMethod(standin.method, methodInfo);
				if (text != null)
				{
					throw new FormatException("Method " + standin.method.FullDescription() + " cannot be patched. Reason: " + text);
				}
			}
			catch (Exception ex)
			{
				throw HarmonyException.Create(ex, finalInstructions);
			}
			PatchTools.RememberObject(standin.method, methodInfo);
			return methodInfo;
		}
	}
}
