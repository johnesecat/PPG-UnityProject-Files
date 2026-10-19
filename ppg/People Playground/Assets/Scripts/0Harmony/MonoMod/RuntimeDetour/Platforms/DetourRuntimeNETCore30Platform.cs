using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour.Platforms
{
	public class DetourRuntimeNETCore30Platform : DetourRuntimeNETCorePlatform
	{
		protected enum CorJitResult
		{
			CORJIT_OK
		}

		protected struct CORINFO_SIG_INST
		{
			public uint classInstCount;

			public unsafe IntPtr* classInst;

			public uint methInstCount;

			public unsafe IntPtr* methInst;
		}

		protected struct CORINFO_SIG_INFO
		{
			public int callConv;

			public IntPtr retTypeClass;

			public IntPtr retTypeSigClass;

			public byte retType;

			public byte flags;

			public ushort numArgs;

			public CORINFO_SIG_INST sigInst;

			public IntPtr args;

			public IntPtr pSig;

			public uint sbSig;

			public IntPtr scope;

			public uint token;
		}

		protected struct CORINFO_METHOD_INFO
		{
			public IntPtr ftn;

			public IntPtr scope;

			public unsafe byte* ILCode;

			public uint ILCodeSize;

			public uint maxStack;

			public uint EHcount;

			public int options;

			public int regionKind;

			public CORINFO_SIG_INFO args;

			public CORINFO_SIG_INFO locals;
		}

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private unsafe delegate CorJitResult d_compileMethod(IntPtr thisPtr, IntPtr corJitInfo, in CORINFO_METHOD_INFO methodInfo, uint flags, out byte* nativeEntry, out uint nativeSizeOfCode);

		protected delegate object d_MethodHandle_GetLoaderAllocator(IntPtr methodHandle);

		protected delegate object d_CreateRuntimeMethodInfoStub(IntPtr methodHandle, object loaderAllocator);

		protected delegate RuntimeMethodHandle d_CreateRuntimeMethodHandle(object runtimeMethodInfo);

		protected delegate Type d_GetDeclaringTypeOfMethodHandle(IntPtr methodHandle);

		protected delegate Type d_GetTypeFromNativeHandle(IntPtr handle);

		public static readonly Guid JitVersionGuid = new Guid("d609bed1-7831-49fc-bd49-b6f054dd4d46");

		private d_compileMethod our_compileMethod;

		private IntPtr real_compileMethodPtr;

		private d_compileMethod real_compileMethod;

		[ThreadStatic]
		private static int hookEntrancy = 0;

		protected d_MethodHandle_GetLoaderAllocator MethodHandle_GetLoaderAllocator;

		protected d_CreateRuntimeMethodInfoStub CreateRuntimeMethodInfoStub;

		protected d_CreateRuntimeMethodHandle CreateRuntimeMethodHandle;

		protected d_GetDeclaringTypeOfMethodHandle GetDeclaringTypeOfMethodHandle;

		protected d_GetTypeFromNativeHandle GetTypeFromNativeHandle;

		private MethodInfo _getTypeFromHandleUnsafeMethod;

		private static FieldInfo _runtimeAssemblyPtrField = Type.GetType("System.Reflection.RuntimeAssembly").GetField("m_assembly", BindingFlags.Instance | BindingFlags.NonPublic);

		public override bool OnMethodCompiledWillBeCalled => true;

		protected unsafe override void DisableInlining(MethodBase method, RuntimeMethodHandle handle)
		{
			ushort* ptr = (ushort*)((byte*)(void*)handle.Value + 6);
			*ptr = (ushort)(*ptr | 0x2000u);
		}

		private IntPtr GetCompileMethod(IntPtr jit)
		{
			return DetourRuntimeNETCorePlatform.ReadObjectVTable(jit, VTableIndex_ICorJitCompiler_compileMethod);
		}

		protected unsafe virtual CorJitResult InvokeRealCompileMethod(IntPtr thisPtr, IntPtr corJitInfo, in CORINFO_METHOD_INFO methodInfo, uint flags, out byte* nativeEntry, out uint nativeSizeOfCode)
		{
			nativeEntry = null;
			nativeSizeOfCode = 0u;
			if (real_compileMethod == null)
			{
				return CorJitResult.CORJIT_OK;
			}
			return real_compileMethod(thisPtr, corJitInfo, in methodInfo, flags, out nativeEntry, out nativeSizeOfCode);
		}

		protected unsafe virtual IntPtr GetCompileMethodHook(IntPtr real)
		{
			real_compileMethod = real.AsDelegate<d_compileMethod>();
			our_compileMethod = CompileMethodHook;
			IntPtr functionPointerForDelegate = Marshal.GetFunctionPointerForDelegate(our_compileMethod);
			NativeDetourData data = CreateNativeTrampolineTo(functionPointerForDelegate);
			d_compileMethod d_compileMethod = data.Method.AsDelegate<d_compileMethod>();
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			CORINFO_METHOD_INFO methodInfo = default(CORINFO_METHOD_INFO);
			d_compileMethod(zero, zero2, in methodInfo, 0u, out var _, out var _);
			FreeNativeTrampoline(data);
			return functionPointerForDelegate;
		}

		protected unsafe override void InstallJitHooks(IntPtr jit)
		{
			SetupJitHookHelpers();
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			CORINFO_METHOD_INFO methodInfo = default(CORINFO_METHOD_INFO);
			InvokeRealCompileMethod(zero, zero2, in methodInfo, 0u, out var _, out var _);
			IntPtr compileMethodHook = GetCompileMethodHook(GetCompileMethod(jit));
			_ = hookEntrancy;
			IntPtr* vTableEntry = DetourRuntimeNETCorePlatform.GetVTableEntry(jit, VTableIndex_ICorJitCompiler_compileMethod);
			DetourHelper.Native.MakeWritable((IntPtr)vTableEntry, (uint)IntPtr.Size);
			real_compileMethodPtr = *vTableEntry;
			*vTableEntry = compileMethodHook;
		}

		protected static NativeDetourData CreateNativeTrampolineTo(IntPtr target)
		{
			IntPtr from = DetourHelper.Native.MemAlloc(64u);
			NativeDetourData nativeDetourData = DetourHelper.Native.Create(from, target);
			DetourHelper.Native.MakeWritable(nativeDetourData);
			DetourHelper.Native.Apply(nativeDetourData);
			DetourHelper.Native.MakeExecutable(nativeDetourData);
			DetourHelper.Native.FlushICache(nativeDetourData);
			return nativeDetourData;
		}

		protected static void FreeNativeTrampoline(NativeDetourData data)
		{
			DetourHelper.Native.MakeWritable(data);
			DetourHelper.Native.MemFree(data.Method);
			DetourHelper.Native.Free(data);
		}

		protected unsafe CorJitResult CompileMethodHook(IntPtr jit, IntPtr corJitInfo, in CORINFO_METHOD_INFO methodInfo, uint flags, out byte* nativeEntry, out uint nativeSizeOfCode)
		{
			nativeEntry = null;
			nativeSizeOfCode = 0u;
			if (jit == IntPtr.Zero)
			{
				return CorJitResult.CORJIT_OK;
			}
			hookEntrancy++;
			try
			{
				CorJitResult result = InvokeRealCompileMethod(jit, corJitInfo, in methodInfo, flags, out nativeEntry, out nativeSizeOfCode);
				if (hookEntrancy == 1)
				{
					try
					{
						RuntimeTypeHandle[] array = null;
						RuntimeTypeHandle[] array2 = null;
						if (methodInfo.args.sigInst.classInst != null)
						{
							array = new RuntimeTypeHandle[methodInfo.args.sigInst.classInstCount];
							for (int i = 0; i < array.Length; i++)
							{
								array[i] = GetTypeFromNativeHandle(methodInfo.args.sigInst.classInst[i]).TypeHandle;
							}
						}
						if (methodInfo.args.sigInst.methInst != null)
						{
							array2 = new RuntimeTypeHandle[methodInfo.args.sigInst.methInstCount];
							for (int j = 0; j < array2.Length; j++)
							{
								array2[j] = GetTypeFromNativeHandle(methodInfo.args.sigInst.methInst[j]).TypeHandle;
							}
						}
						RuntimeTypeHandle typeHandle = GetDeclaringTypeOfMethodHandle(methodInfo.ftn).TypeHandle;
						RuntimeMethodHandle methodHandle = CreateHandleForHandlePointer(methodInfo.ftn);
						JitHookCore(typeHandle, methodHandle, (IntPtr)nativeEntry, nativeSizeOfCode, array, array2);
					}
					catch
					{
					}
				}
				return result;
			}
			finally
			{
				hookEntrancy--;
			}
		}

		protected RuntimeMethodHandle CreateHandleForHandlePointer(IntPtr handle)
		{
			return CreateRuntimeMethodHandle(CreateRuntimeMethodInfoStub(handle, MethodHandle_GetLoaderAllocator(handle)));
		}

		protected virtual void SetupJitHookHelpers()
		{
			MethodInfo methodInfo = typeof(object).Assembly.GetType("Internal.Runtime.CompilerServices.Unsafe").GetMethods().First((MethodInfo m) => m.Name == "As" && m.ReturnType.IsByRef);
			MethodInfo method = typeof(RuntimeMethodHandle).GetMethod("GetLoaderAllocator", BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo method2;
			using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition("MethodHandle_GetLoaderAllocator", typeof(object), new Type[1] { typeof(IntPtr) }))
			{
				ILProcessor iLProcessor = dynamicMethodDefinition.GetILProcessor();
				ModuleDefinition module = iLProcessor.Body.Method.Module;
				Type parameterType = method.GetParameters().First().ParameterType;
				iLProcessor.Emit(OpCodes.Ldarga_S, iLProcessor.Body.Method.Parameters[0]);
				iLProcessor.Emit(OpCodes.Call, module.ImportReference(methodInfo.MakeGenericMethod(typeof(IntPtr), parameterType)));
				iLProcessor.Emit(OpCodes.Ldobj, module.ImportReference(parameterType));
				iLProcessor.Emit(OpCodes.Call, module.ImportReference(method));
				iLProcessor.Emit(OpCodes.Ret);
				method2 = dynamicMethodDefinition.Generate();
			}
			MethodHandle_GetLoaderAllocator = method2.CreateDelegate<d_MethodHandle_GetLoaderAllocator>();
			MethodInfo orCreateGetTypeFromHandleUnsafe = GetOrCreateGetTypeFromHandleUnsafe();
			GetTypeFromNativeHandle = orCreateGetTypeFromHandleUnsafe.CreateDelegate<d_GetTypeFromNativeHandle>();
			Type type = typeof(RuntimeMethodHandle).Assembly.GetType("System.RuntimeMethodHandleInternal");
			MethodInfo method3 = typeof(RuntimeMethodHandle).GetMethod("GetDeclaringType", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[1] { type }, null);
			MethodInfo method4;
			using (DynamicMethodDefinition dynamicMethodDefinition2 = new DynamicMethodDefinition("GetDeclaringTypeOfMethodHandle", typeof(Type), new Type[1] { typeof(IntPtr) }))
			{
				ILProcessor iLProcessor2 = dynamicMethodDefinition2.GetILProcessor();
				ModuleDefinition module2 = iLProcessor2.Body.Method.Module;
				iLProcessor2.Emit(OpCodes.Ldarga_S, iLProcessor2.Body.Method.Parameters[0]);
				iLProcessor2.Emit(OpCodes.Call, module2.ImportReference(methodInfo.MakeGenericMethod(typeof(IntPtr), type)));
				iLProcessor2.Emit(OpCodes.Ldobj, module2.ImportReference(type));
				iLProcessor2.Emit(OpCodes.Call, module2.ImportReference(method3));
				iLProcessor2.Emit(OpCodes.Ret);
				method4 = dynamicMethodDefinition2.Generate();
			}
			GetDeclaringTypeOfMethodHandle = method4.CreateDelegate<d_GetDeclaringTypeOfMethodHandle>();
			Type[] array = new Type[2]
			{
				typeof(IntPtr),
				typeof(object)
			};
			Type type2 = typeof(RuntimeMethodHandle).Assembly.GetType("System.RuntimeMethodInfoStub");
			ConstructorInfo constructor = type2.GetConstructor(array);
			MethodInfo method5;
			using (DynamicMethodDefinition dynamicMethodDefinition3 = new DynamicMethodDefinition("new RuntimeMethodInfoStub", type2, array))
			{
				ILProcessor iLProcessor3 = dynamicMethodDefinition3.GetILProcessor();
				ModuleDefinition module3 = iLProcessor3.Body.Method.Module;
				iLProcessor3.Emit(OpCodes.Ldarg_0);
				iLProcessor3.Emit(OpCodes.Ldarg_1);
				iLProcessor3.Emit(OpCodes.Newobj, module3.ImportReference(constructor));
				iLProcessor3.Emit(OpCodes.Ret);
				method5 = dynamicMethodDefinition3.Generate();
			}
			CreateRuntimeMethodInfoStub = method5.CreateDelegate<d_CreateRuntimeMethodInfoStub>();
			ConstructorInfo method6 = typeof(RuntimeMethodHandle).GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).First();
			MethodInfo method7;
			using (DynamicMethodDefinition dynamicMethodDefinition4 = new DynamicMethodDefinition("new RuntimeMethodHandle", typeof(RuntimeMethodHandle), new Type[1] { typeof(object) }))
			{
				ILProcessor iLProcessor4 = dynamicMethodDefinition4.GetILProcessor();
				ModuleDefinition module4 = iLProcessor4.Body.Method.Module;
				iLProcessor4.Emit(OpCodes.Ldarg_0);
				iLProcessor4.Emit(OpCodes.Newobj, module4.ImportReference(method6));
				iLProcessor4.Emit(OpCodes.Ret);
				method7 = dynamicMethodDefinition4.Generate();
			}
			CreateRuntimeMethodHandle = method7.CreateDelegate<d_CreateRuntimeMethodHandle>();
		}

		private MethodInfo GetOrCreateGetTypeFromHandleUnsafe()
		{
			if (_getTypeFromHandleUnsafeMethod != null)
			{
				return _getTypeFromHandleUnsafeMethod;
			}
			Assembly assembly;
			using (ModuleDefinition moduleDefinition = ModuleDefinition.CreateModule("MonoMod.RuntimeDetour.Runtime.NETCore3+Helpers", new ModuleParameters
			{
				Kind = ModuleKind.Dll
			}))
			{
				TypeDefinition typeDefinition = new TypeDefinition("System", "Type", Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.Abstract)
				{
					BaseType = moduleDefinition.TypeSystem.Object
				};
				moduleDefinition.Types.Add(typeDefinition);
				MethodDefinition methodDefinition = new MethodDefinition("GetTypeFromHandleUnsafe", Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.Static, moduleDefinition.ImportReference(typeof(Type)))
				{
					IsInternalCall = true
				};
				methodDefinition.Parameters.Add(new ParameterDefinition(moduleDefinition.ImportReference(typeof(IntPtr))));
				typeDefinition.Methods.Add(methodDefinition);
				assembly = ReflectionHelper.Load(moduleDefinition);
			}
			MakeAssemblySystemAssembly(assembly);
			return _getTypeFromHandleUnsafeMethod = assembly.GetType("System.Type").GetMethod("GetTypeFromHandleUnsafe");
		}

		protected unsafe virtual void MakeAssemblySystemAssembly(Assembly assembly)
		{
			IntPtr intPtr = (IntPtr)_runtimeAssemblyPtrField.GetValue(assembly);
			int num = IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + 4 + IntPtr.Size + IntPtr.Size + 4 + 4 + IntPtr.Size + IntPtr.Size + 4 + 4 + IntPtr.Size;
			if (IntPtr.Size == 8)
			{
				num += 4;
			}
			IntPtr intPtr2 = *(IntPtr*)((byte*)(void*)intPtr + num);
			int num2 = IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size;
			IntPtr intPtr3 = *(IntPtr*)((byte*)(void*)intPtr2 + num2);
			int num3 = IntPtr.Size + IntPtr.Size + IntPtr.Size + 4 + 4 + IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + 4;
			int* ptr = (int*)((byte*)(void*)intPtr3 + num3);
			*ptr |= 1;
		}

		protected void HookPermanent(MethodBase from, MethodBase to)
		{
			Pin(from);
			Pin(to);
			HookPermanent(GetNativeStart(from), GetNativeStart(to));
		}

		protected void HookPermanent(IntPtr from, IntPtr to)
		{
			NativeDetourData detour = DetourHelper.Native.Create(from, to);
			DetourHelper.Native.MakeWritable(detour);
			DetourHelper.Native.Apply(detour);
			DetourHelper.Native.MakeExecutable(detour);
			DetourHelper.Native.FlushICache(detour);
			DetourHelper.Native.Free(detour);
		}
	}
}
