using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Mono.Cecil.Cil;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour.Platforms
{
	public abstract class DetourRuntimeILPlatform : IDetourRuntimePlatform
	{
		private struct _SelftestStruct
		{
			private readonly short Value;

			private readonly byte E1;

			private readonly byte E2;

			private readonly byte E3;

			[MethodImpl(MethodImplOptions.NoInlining)]
			public short _SelftestGetInStruct()
			{
				Console.Error.WriteLine("If you're reading this, the MonoMod.RuntimeDetour selftest failed.");
				return -1;
			}
		}

		protected class PrivateMethodPin
		{
			public MethodPinInfo Pin;
		}

		public struct MethodPinInfo
		{
			public int Count;

			public MethodBase Method;

			public RuntimeMethodHandle Handle;

			public override string ToString()
			{
				return $"(MethodPinInfo: {Count}, {Method}, 0x{(long)Handle.Value:X})";
			}
		}

		protected enum GlueThiscallStructRetPtrOrder
		{
			Original,
			ThisRetArgs,
			RetThisArgs
		}

		protected GlueThiscallStructRetPtrOrder GlueThiscallStructRetPtr;

		protected GlueThiscallStructRetPtrOrder GlueThiscallInStructRetPtr;

		protected ConcurrentDictionary<MethodBase, PrivateMethodPin> PinnedMethods = new ConcurrentDictionary<MethodBase, PrivateMethodPin>();

		protected ConcurrentDictionary<RuntimeMethodHandle, PrivateMethodPin> PinnedHandles = new ConcurrentDictionary<RuntimeMethodHandle, PrivateMethodPin>();

		private IntPtr ReferenceNonDynamicPoolPtr;

		private IntPtr ReferenceDynamicPoolPtr;

		protected static readonly uint _MemAllocScratchDummySafeSize = 16u;

		protected static readonly MethodInfo _MemAllocScratchDummy = typeof(DetourRuntimeILPlatform).GetMethod("MemAllocScratchDummy", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

		public abstract bool OnMethodCompiledWillBeCalled { get; }

		public abstract event OnMethodCompiledEvent OnMethodCompiled;

		protected abstract RuntimeMethodHandle GetMethodHandle(MethodBase method);

		public unsafe DetourRuntimeILPlatform()
		{
			MethodInfo method = typeof(DetourRuntimeILPlatform).GetMethod("_SelftestGetRefPtr", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method2 = typeof(DetourRuntimeILPlatform).GetMethod("_SelftestGetRefPtrHook", BindingFlags.Static | BindingFlags.NonPublic);
			_HookSelftest(method, method2);
			IntPtr arg = ((Func<IntPtr>)Delegate.CreateDelegate(typeof(Func<IntPtr>), this, method))();
			MethodInfo method3 = typeof(DetourRuntimeILPlatform).GetMethod("_SelftestGetStruct", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method4 = typeof(DetourRuntimeILPlatform).GetMethod("_SelftestGetStructHook", BindingFlags.Static | BindingFlags.NonPublic);
			_HookSelftest(method3, method4);
			fixed (GlueThiscallStructRetPtrOrder* ptr = &GlueThiscallStructRetPtr)
			{
				((Func<IntPtr, IntPtr, IntPtr, _SelftestStruct>)Delegate.CreateDelegate(typeof(Func<IntPtr, IntPtr, IntPtr, _SelftestStruct>), this, method3))((IntPtr)ptr, (IntPtr)ptr, arg);
			}
			MethodInfo method5 = typeof(_SelftestStruct).GetMethod("_SelftestGetInStruct", BindingFlags.Instance | BindingFlags.Public);
			MethodInfo method6 = typeof(DetourRuntimeILPlatform).GetMethod("_SelftestGetInStructHook", BindingFlags.Static | BindingFlags.NonPublic);
			_HookSelftest(method5, method6);
			fixed (GlueThiscallStructRetPtrOrder* ptr2 = &GlueThiscallInStructRetPtr)
			{
				*ptr2 = (GlueThiscallStructRetPtrOrder)((Func<short>)Delegate.CreateDelegate(firstArgument: default(_SelftestStruct), type: typeof(Func<short>), method: method5))();
				if (*ptr2 == (GlueThiscallStructRetPtrOrder)(-1))
				{
					throw new Exception("_SelftestGetInStruct failed!");
				}
			}
			Pin(method);
			ReferenceNonDynamicPoolPtr = GetNativeStart(method);
			if (DynamicMethodDefinition.IsDynamicILAvailable)
			{
				MethodBase method7;
				using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition(_MemAllocScratchDummy))
				{
					dynamicMethodDefinition.Name = "MemAllocScratch<Reference>";
					method7 = DMDGenerator<DMDEmitDynamicMethodGenerator>.Generate(dynamicMethodDefinition);
				}
				Pin(method7);
				ReferenceDynamicPoolPtr = GetNativeStart(method7);
			}
		}

		private void _HookSelftest(MethodInfo from, MethodInfo to)
		{
			Pin(from);
			Pin(to);
			NativeDetourData detour = DetourHelper.Native.Create(GetNativeStart(from), GetNativeStart(to));
			DetourHelper.Native.MakeWritable(detour);
			DetourHelper.Native.Apply(detour);
			DetourHelper.Native.MakeExecutable(detour);
			DetourHelper.Native.FlushICache(detour);
			DetourHelper.Native.Free(detour);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private IntPtr _SelftestGetRefPtr()
		{
			Console.Error.WriteLine("If you're reading this, the MonoMod.RuntimeDetour selftest failed.");
			throw new Exception("This method should've been detoured!");
		}

		private static IntPtr _SelftestGetRefPtrHook(IntPtr self)
		{
			return self;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private _SelftestStruct _SelftestGetStruct(IntPtr x, IntPtr y, IntPtr thisPtr)
		{
			Console.Error.WriteLine("If you're reading this, the MonoMod.RuntimeDetour selftest failed.");
			throw new Exception("_SelftestGetStruct failed!");
		}

		private unsafe static void _SelftestGetStructHook(IntPtr a, IntPtr b, IntPtr c, IntPtr d, IntPtr e)
		{
			if (b == c)
			{
				*(int*)(void*)b = 0;
			}
			else if (b == e)
			{
				*(int*)(void*)c = 2;
			}
			else
			{
				*(int*)(void*)c = 1;
			}
		}

		private unsafe static short _SelftestGetInStructHook(IntPtr a)
		{
			*(short*)(void*)a = 2;
			return 0;
		}

		protected virtual IntPtr GetFunctionPointer(MethodBase method, RuntimeMethodHandle handle)
		{
			return handle.GetFunctionPointer();
		}

		protected virtual void PrepareMethod(MethodBase method, RuntimeMethodHandle handle)
		{
			RuntimeHelpers.PrepareMethod(handle);
		}

		protected virtual void PrepareMethod(MethodBase method, RuntimeMethodHandle handle, RuntimeTypeHandle[] instantiation)
		{
			RuntimeHelpers.PrepareMethod(handle, instantiation);
		}

		protected virtual void DisableInlining(MethodBase method, RuntimeMethodHandle handle)
		{
		}

		public virtual MethodBase GetIdentifiable(MethodBase method)
		{
			if (!PinnedHandles.TryGetValue(GetMethodHandle(method), out var value))
			{
				return method;
			}
			return value.Pin.Method;
		}

		public virtual MethodPinInfo GetPin(MethodBase method)
		{
			if (!PinnedMethods.TryGetValue(method, out var value))
			{
				return default(MethodPinInfo);
			}
			return value.Pin;
		}

		public virtual MethodPinInfo GetPin(RuntimeMethodHandle handle)
		{
			if (!PinnedHandles.TryGetValue(handle, out var value))
			{
				return default(MethodPinInfo);
			}
			return value.Pin;
		}

		public virtual MethodPinInfo[] GetPins()
		{
			return (from p in PinnedHandles.Values.ToArray()
				select p.Pin).ToArray();
		}

		public virtual IntPtr GetNativeStart(MethodBase method)
		{
			method = GetIdentifiable(method);
			if (PinnedMethods.TryGetValue(method, out var value))
			{
				return GetFunctionPointer(method, value.Pin.Handle);
			}
			return GetFunctionPointer(method, GetMethodHandle(method));
		}

		public virtual void Pin(MethodBase method)
		{
			method = GetIdentifiable(method);
			Interlocked.Increment(ref PinnedMethods.GetOrAdd(method, delegate(MethodBase m)
			{
				PrivateMethodPin privateMethodPin = new PrivateMethodPin
				{
					Pin = 
					{
						Method = m
					}
				};
				RuntimeMethodHandle runtimeMethodHandle = (privateMethodPin.Pin.Handle = GetMethodHandle(m));
				PinnedHandles[runtimeMethodHandle] = privateMethodPin;
				DisableInlining(method, runtimeMethodHandle);
				if (method.DeclaringType?.IsGenericType ?? false)
				{
					PrepareMethod(method, runtimeMethodHandle, (from type in method.DeclaringType.GetGenericArguments()
						select type.TypeHandle).ToArray());
				}
				else
				{
					PrepareMethod(method, runtimeMethodHandle);
				}
				return privateMethodPin;
			}).Pin.Count);
		}

		public virtual void Unpin(MethodBase method)
		{
			method = GetIdentifiable(method);
			if (PinnedMethods.TryGetValue(method, out var value) && Interlocked.Decrement(ref value.Pin.Count) <= 0)
			{
				PinnedMethods.TryRemove(method, out var value2);
				PinnedHandles.TryRemove(value.Pin.Handle, out value2);
			}
		}

		public MethodInfo CreateCopy(MethodBase method)
		{
			method = GetIdentifiable(method);
			if (method == null || (method.GetMethodImplementationFlags() & MethodImplAttributes.CodeTypeMask) != 0)
			{
				throw new InvalidOperationException("Uncopyable method: " + (method?.ToString() ?? "NULL"));
			}
			using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition(method))
			{
				return dynamicMethodDefinition.Generate();
			}
		}

		public bool TryCreateCopy(MethodBase method, out MethodInfo dm)
		{
			method = GetIdentifiable(method);
			if (method == null || (method.GetMethodImplementationFlags() & MethodImplAttributes.CodeTypeMask) != 0)
			{
				dm = null;
				return false;
			}
			try
			{
				dm = CreateCopy(method);
				return true;
			}
			catch
			{
				dm = null;
				return false;
			}
		}

		public MethodBase GetDetourTarget(MethodBase from, MethodBase to)
		{
			to = GetIdentifiable(to);
			MethodInfo methodInfo = null;
			MethodInfo methodInfo2 = from as MethodInfo;
			if ((object)methodInfo2 != null && !from.IsStatic)
			{
				MethodInfo methodInfo3 = to as MethodInfo;
				GlueThiscallStructRetPtrOrder glueThiscallStructRetPtrOrder;
				if ((object)methodInfo3 != null && to.IsStatic && methodInfo2.ReturnType == methodInfo3.ReturnType && methodInfo2.ReturnType.IsValueType && (glueThiscallStructRetPtrOrder = ((from.DeclaringType?.IsValueType ?? false) ? GlueThiscallInStructRetPtr : GlueThiscallStructRetPtr)) != 0)
				{
					int managedSize = methodInfo2.ReturnType.GetManagedSize();
					if (managedSize == 3 || managedSize == 5 || managedSize == 6 || managedSize == 7 || managedSize > IntPtr.Size)
					{
						Type thisParamType = from.GetThisParamType();
						Type item = methodInfo2.ReturnType.MakeByRefType();
						int value = 0;
						int num = 1;
						if (glueThiscallStructRetPtrOrder == GlueThiscallStructRetPtrOrder.RetThisArgs)
						{
							value = 1;
							num = 0;
						}
						List<Type> list = new List<Type> { thisParamType };
						list.Insert(num, item);
						list.AddRange(from p in @from.GetParameters()
							select p.ParameterType);
						using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition("Glue:ThiscallStructRetPtr<" + from.GetID(null, null, withType: true, proxyMethod: false, simple: true) + "," + to.GetID(null, null, withType: true, proxyMethod: false, simple: true) + ">", typeof(void), list.ToArray()))
						{
							ILProcessor iLProcessor = dynamicMethodDefinition.GetILProcessor();
							iLProcessor.Emit(OpCodes.Ldarg, num);
							iLProcessor.Emit(OpCodes.Ldarg, value);
							for (int i = 2; i < list.Count; i++)
							{
								iLProcessor.Emit(OpCodes.Ldarg, i);
							}
							iLProcessor.Emit(OpCodes.Call, iLProcessor.Body.Method.Module.ImportReference(to));
							iLProcessor.Emit(OpCodes.Stobj, iLProcessor.Body.Method.Module.ImportReference(methodInfo2.ReturnType));
							iLProcessor.Emit(OpCodes.Ret);
							methodInfo = dynamicMethodDefinition.Generate();
						}
					}
				}
			}
			return methodInfo ?? to;
		}

		public uint TryMemAllocScratchCloseTo(IntPtr target, out IntPtr ptr, int size)
		{
			if (size == 0 || size > _MemAllocScratchDummySafeSize)
			{
				ptr = IntPtr.Zero;
				return 0u;
			}
			bool num = Math.Abs((long)target - (long)ReferenceNonDynamicPoolPtr) < 1073741824;
			bool flag = DynamicMethodDefinition.IsDynamicILAvailable && Math.Abs((long)target - (long)ReferenceDynamicPoolPtr) < 1073741824;
			if (!num && !flag)
			{
				ptr = IntPtr.Zero;
				return 0u;
			}
			MethodBase method;
			using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition(_MemAllocScratchDummy))
			{
				dynamicMethodDefinition.Name = $"MemAllocScratch<{(long)target:X16}>";
				method = ((!flag) ? DMDGenerator<DMDCecilGenerator>.Generate(dynamicMethodDefinition) : DMDGenerator<DMDEmitDynamicMethodGenerator>.Generate(dynamicMethodDefinition));
			}
			Pin(method);
			ptr = GetNativeStart(method);
			DetourHelper.Native.MakeReadWriteExecutable(ptr, _MemAllocScratchDummySafeSize);
			return _MemAllocScratchDummySafeSize;
		}

		public static int MemAllocScratchDummy(int a, int b)
		{
			if (a >= 1024 && b >= 1024)
			{
				return a + b;
			}
			return MemAllocScratchDummy(a + b, b + 1);
		}
	}
}
