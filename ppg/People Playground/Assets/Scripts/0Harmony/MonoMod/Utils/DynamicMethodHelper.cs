using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MonoMod.Utils
{
	internal static class DynamicMethodHelper
	{
		private static List<object> References = new List<object>();

		private static readonly MethodInfo _GetMethodFromHandle = typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[1] { typeof(RuntimeMethodHandle) });

		private static readonly MethodInfo _GetReference = typeof(DynamicMethodHelper).GetMethod("GetReference");

		public static object GetReference(int id)
		{
			return References[id];
		}

		public static void SetReference(int id, object obj)
		{
			References[id] = obj;
		}

		private static int AddReference(object obj)
		{
			lock (References)
			{
				References.Add(obj);
				return References.Count - 1;
			}
		}

		public static void FreeReference(int id)
		{
			References[id] = null;
		}

		public static DynamicMethod Stub(this DynamicMethod dm)
		{
			ILGenerator iLGenerator = dm.GetILGenerator();
			for (int i = 0; i < 32; i++)
			{
				iLGenerator.Emit(System.Reflection.Emit.OpCodes.Nop);
			}
			if (dm.ReturnType != typeof(void))
			{
				iLGenerator.DeclareLocal(dm.ReturnType);
				iLGenerator.Emit(System.Reflection.Emit.OpCodes.Ldloca_S, (sbyte)0);
				iLGenerator.Emit(System.Reflection.Emit.OpCodes.Initobj, dm.ReturnType);
				iLGenerator.Emit(System.Reflection.Emit.OpCodes.Ldloc_0);
			}
			iLGenerator.Emit(System.Reflection.Emit.OpCodes.Ret);
			return dm;
		}

		public static DynamicMethodDefinition Stub(this DynamicMethodDefinition dmd)
		{
			ILProcessor iLProcessor = dmd.GetILProcessor();
			for (int i = 0; i < 32; i++)
			{
				iLProcessor.Emit(Mono.Cecil.Cil.OpCodes.Nop);
			}
			if (dmd.Definition.ReturnType != dmd.Definition.Module.TypeSystem.Void)
			{
				iLProcessor.Body.Variables.Add(new VariableDefinition(dmd.Definition.ReturnType));
				iLProcessor.Emit(Mono.Cecil.Cil.OpCodes.Ldloca_S, (sbyte)0);
				iLProcessor.Emit(Mono.Cecil.Cil.OpCodes.Initobj, dmd.Definition.ReturnType);
				iLProcessor.Emit(Mono.Cecil.Cil.OpCodes.Ldloc_0);
			}
			iLProcessor.Emit(Mono.Cecil.Cil.OpCodes.Ret);
			return dmd;
		}

		public static int EmitReference<T>(this ILGenerator il, T obj)
		{
			Type typeFromHandle = typeof(T);
			int num = AddReference(obj);
			il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, num);
			il.Emit(System.Reflection.Emit.OpCodes.Call, _GetReference);
			if (typeFromHandle.IsValueType)
			{
				il.Emit(System.Reflection.Emit.OpCodes.Unbox_Any, typeFromHandle);
			}
			return num;
		}

		public static int EmitReference<T>(this ILProcessor il, T obj)
		{
			ModuleDefinition module = il.Body.Method.Module;
			Type typeFromHandle = typeof(T);
			int num = AddReference(obj);
			il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, num);
			il.Emit(Mono.Cecil.Cil.OpCodes.Call, module.ImportReference(_GetReference));
			if (typeFromHandle.IsValueType)
			{
				il.Emit(Mono.Cecil.Cil.OpCodes.Unbox_Any, module.ImportReference(typeFromHandle));
			}
			return num;
		}

		public static int EmitGetReference<T>(this ILGenerator il, int id)
		{
			Type typeFromHandle = typeof(T);
			il.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, id);
			il.Emit(System.Reflection.Emit.OpCodes.Call, _GetReference);
			if (typeFromHandle.IsValueType)
			{
				il.Emit(System.Reflection.Emit.OpCodes.Unbox_Any, typeFromHandle);
			}
			return id;
		}

		public static int EmitGetReference<T>(this ILProcessor il, int id)
		{
			ModuleDefinition module = il.Body.Method.Module;
			Type typeFromHandle = typeof(T);
			il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, id);
			il.Emit(Mono.Cecil.Cil.OpCodes.Call, module.ImportReference(_GetReference));
			if (typeFromHandle.IsValueType)
			{
				il.Emit(Mono.Cecil.Cil.OpCodes.Unbox_Any, module.ImportReference(typeFromHandle));
			}
			return id;
		}
	}
}
