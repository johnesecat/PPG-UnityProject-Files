using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using MonoMod.Utils;

namespace HarmonyLib
{
	internal class StructReturnBuffer
	{
		private static readonly Dictionary<Type, int> sizes = new Dictionary<Type, int>();

		private static readonly HashSet<int> specialSizes = new HashSet<int> { 1, 2, 4, 8 };

		internal static bool hasTestResult_Mono;

		private static readonly object hasTestResult_Mono_lock = new object();

		internal static bool hasTestResult_Net;

		private static readonly object hasTestResult_Net_lock = new object();

		private static int SizeOf(Type type)
		{
			lock (sizes)
			{
				if (sizes.TryGetValue(type, out var value))
				{
					return value;
				}
				value = type.GetManagedSize();
				sizes.Add(type, value);
				return value;
			}
		}

		internal static bool NeedsFix(MethodBase method)
		{
			Type returnedType = AccessTools.GetReturnedType(method);
			if (!AccessTools.IsStruct(returnedType))
			{
				return false;
			}
			if (!AccessTools.IsMonoRuntime && method.IsStatic)
			{
				return false;
			}
			int num = SizeOf(returnedType);
			if (!Tools.isWindows && num <= 16)
			{
				return false;
			}
			if (specialSizes.Contains(num))
			{
				return false;
			}
			return HasStructReturnBuffer();
		}

		private static bool HasStructReturnBuffer()
		{
			if (AccessTools.IsMonoRuntime)
			{
				lock (hasTestResult_Mono_lock)
				{
					if (!hasTestResult_Mono)
					{
						Sandbox.hasStructReturnBuffer_Mono = false;
						MethodInfo original = AccessTools.DeclaredMethod(typeof(Sandbox), "GetStruct_Mono");
						MethodInfo replacement = AccessTools.DeclaredMethod(typeof(Sandbox), "GetStructReplacement_Mono");
						Memory.DetourMethod(original, replacement);
						new Sandbox().GetStruct_Mono(Sandbox.magicValue, Sandbox.magicValue);
						hasTestResult_Mono = true;
					}
				}
				return Sandbox.hasStructReturnBuffer_Mono;
			}
			lock (hasTestResult_Net_lock)
			{
				if (!hasTestResult_Net)
				{
					Sandbox.hasStructReturnBuffer_Net = false;
					MethodInfo original2 = AccessTools.DeclaredMethod(typeof(Sandbox), Tools.isWindows ? "GetStruct_Net" : "GetStruct_NetLinux");
					MethodInfo replacement2 = AccessTools.DeclaredMethod(typeof(Sandbox), "GetStructReplacement_Net");
					Memory.DetourMethod(original2, replacement2);
					if (Tools.isWindows)
					{
						new Sandbox().GetStruct_Net(Sandbox.magicValue, Sandbox.magicValue);
					}
					else
					{
						new Sandbox().GetStruct_NetLinux(Sandbox.magicValue, Sandbox.magicValue);
					}
					hasTestResult_Net = true;
				}
			}
			return Sandbox.hasStructReturnBuffer_Net;
		}

		internal static void ResetCaches()
		{
			lock (sizes)
			{
				sizes.Clear();
			}
			lock (hasTestResult_Mono_lock)
			{
				hasTestResult_Mono = false;
			}
			lock (hasTestResult_Net_lock)
			{
				hasTestResult_Net = false;
			}
		}

		internal static void ArgumentShifter(List<CodeInstruction> instructions, bool shiftArgZero)
		{
			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Ldarg_3)
				{
					instruction.opcode = OpCodes.Ldarg;
					instruction.operand = 4;
				}
				else if (instruction.opcode == OpCodes.Ldarg_2)
				{
					instruction.opcode = OpCodes.Ldarg_3;
				}
				else if (instruction.opcode == OpCodes.Ldarg_1)
				{
					instruction.opcode = OpCodes.Ldarg_2;
				}
				else if (shiftArgZero && instruction.opcode == OpCodes.Ldarg_0)
				{
					instruction.opcode = OpCodes.Ldarg_1;
				}
				else if (instruction.opcode == OpCodes.Ldarg || instruction.opcode == OpCodes.Ldarg_S || instruction.opcode == OpCodes.Ldarga || instruction.opcode == OpCodes.Ldarga_S || instruction.opcode == OpCodes.Starg || instruction.opcode == OpCodes.Starg_S)
				{
					short num = Convert.ToInt16(instruction.operand);
					if (num > 0 || shiftArgZero)
					{
						instruction.operand = num + 1;
					}
				}
			}
		}
	}
}
