using System;
using System.Reflection;
using System.Reflection.Emit;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace HarmonyLib
{
	public static class Memory
	{
		public unsafe static void MarkForNoInlining(MethodBase method)
		{
			if (AccessTools.IsMonoRuntime)
			{
				byte* intPtr = (byte*)(void*)method.MethodHandle.Value + 2;
				*(ushort*)intPtr = (ushort)(*(ushort*)intPtr | 8);
			}
		}

		public static string DetourMethod(MethodBase original, MethodBase replacement)
		{
			Exception exception;
			long methodStart = GetMethodStart(original, out exception);
			if (methodStart == 0L)
			{
				return exception.Message;
			}
			PadShortMethods(original);
			long methodStart2 = GetMethodStart(replacement, out exception);
			if (methodStart2 == 0L)
			{
				return exception.Message;
			}
			return WriteJump(methodStart, methodStart2);
		}

		internal static void DetourCompiledMethod(IntPtr originalCodeStart, MethodBase replacement)
		{
			Exception exception;
			long methodStart = GetMethodStart(replacement, out exception);
			if (methodStart != 0L && exception == null)
			{
				WriteJump((long)originalCodeStart, methodStart);
			}
		}

		internal static void DetourMethodAndPersist(MethodBase original, MethodBase replacement)
		{
			string text = DetourMethod(original, replacement);
			if (text != null)
			{
				throw new FormatException("Method " + original.FullDescription() + " cannot be patched. Reason: " + text);
			}
			PatchTools.RememberObject(original, replacement);
		}

		internal static void PadShortMethods(MethodBase method)
		{
			if (!Tools.isWindows)
			{
				int valueOrDefault = (method.GetMethodBody()?.GetILAsByteArray()?.Length).GetValueOrDefault();
				if (valueOrDefault != 0 && valueOrDefault < 16)
				{
					DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition($"PadMethod-{Guid.NewGuid()}", typeof(void), new Type[0]);
					dynamicMethodDefinition.GetILGenerator().Emit(OpCodes.Ret);
					dynamicMethodDefinition.Generate().Invoke(null, null);
				}
			}
		}

		public static string WriteJump(long memory, long destination)
		{
			NativeDetourData detour = DetourHelper.Native.Create((IntPtr)memory, (IntPtr)destination);
			DetourHelper.Native.MakeWritable(detour);
			DetourHelper.Native.Apply(detour);
			DetourHelper.Native.MakeExecutable(detour);
			DetourHelper.Native.FlushICache(detour);
			DetourHelper.Native.Free(detour);
			return null;
		}

		public static long GetMethodStart(MethodBase method, out Exception exception)
		{
			try
			{
				exception = null;
				return method.Pin().GetNativeStart().ToInt64();
			}
			catch (Exception ex)
			{
				Exception ex2 = (exception = ex);
				return 0L;
			}
		}
	}
}
