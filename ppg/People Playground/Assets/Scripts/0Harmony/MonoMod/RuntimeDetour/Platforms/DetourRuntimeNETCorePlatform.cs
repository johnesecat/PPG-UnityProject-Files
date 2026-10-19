using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour.Platforms
{
	public class DetourRuntimeNETCorePlatform : DetourRuntimeNETPlatform
	{
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate IntPtr d_getJit();

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void d_getVersionIdentifier(IntPtr thisPtr, out Guid versionIdentifier);

		private static d_getJit getJit;

		private static bool isNet5Jit;

		private const int vtableIndex_ICorJitCompiler_getVersionIdentifier = 4;

		private const int vtableIndex_ICorJitCompiler_getVersionIdentifier_net5 = 2;

		protected virtual int VTableIndex_ICorJitCompiler_compileMethod => 0;

		public override bool OnMethodCompiledWillBeCalled => false;

		public override event OnMethodCompiledEvent OnMethodCompiled;

		public DetourRuntimeNETCorePlatform()
		{
			GlueThiscallInStructRetPtr = GlueThiscallStructRetPtr;
		}

		protected static IntPtr GetJitObject()
		{
			if (getJit == null)
			{
				ProcessModule processModule = Process.GetCurrentProcess().Modules.Cast<ProcessModule>().FirstOrDefault((ProcessModule m) => Path.GetFileNameWithoutExtension(m.FileName).EndsWith("clrjit", StringComparison.Ordinal));
				if (processModule == null)
				{
					throw new PlatformNotSupportedException();
				}
				if (!DynDll.TryOpenLibrary(processModule.FileName, out var libraryPtr))
				{
					throw new PlatformNotSupportedException();
				}
				if (PlatformHelper.Is(Platform.Windows))
				{
					isNet5Jit = processModule.FileVersionInfo.ProductMajorPart >= 5;
				}
				else
				{
					isNet5Jit = typeof(object).Assembly.GetName().Version.Major >= 5;
				}
				try
				{
					getJit = libraryPtr.GetFunction("getJit").AsDelegate<d_getJit>();
				}
				catch
				{
					DynDll.CloseLibrary(libraryPtr);
					throw;
				}
			}
			return getJit();
		}

		protected static Guid GetJitGuid(IntPtr jit)
		{
			int index = (isNet5Jit ? 2 : 4);
			ReadObjectVTable(jit, index).AsDelegate<d_getVersionIdentifier>()(jit, out var versionIdentifier);
			return versionIdentifier;
		}

		protected unsafe static IntPtr* GetVTableEntry(IntPtr @object, int index)
		{
			return (IntPtr*)((ulong)(long)(*(IntPtr*)(void*)@object) + (ulong)(long)(IntPtr)(void*)((long)index * (long)sizeof(IntPtr)));
		}

		protected unsafe static IntPtr ReadObjectVTable(IntPtr @object, int index)
		{
			return *GetVTableEntry(@object, index);
		}

		protected override void DisableInlining(MethodBase method, RuntimeMethodHandle handle)
		{
		}

		protected virtual void InstallJitHooks(IntPtr jitObject)
		{
			throw new PlatformNotSupportedException();
		}

		protected virtual void JitHookCore(RuntimeTypeHandle declaringType, RuntimeMethodHandle methodHandle, IntPtr methodBodyStart, ulong methodBodySize, RuntimeTypeHandle[] genericClassArguments, RuntimeTypeHandle[] genericMethodArguments)
		{
			try
			{
				Type type = Type.GetTypeFromHandle(declaringType);
				if (genericClassArguments != null && type.IsGenericTypeDefinition)
				{
					type = type.MakeGenericType(genericClassArguments.Select(Type.GetTypeFromHandle).ToArray());
				}
				MethodBase methodBase = MethodBase.GetMethodFromHandle(methodHandle, type.TypeHandle);
				if (methodBase == null)
				{
					methodBase = GetPin(methodHandle).Method;
				}
				try
				{
					OnMethodCompiled?.Invoke(methodBase, methodBodyStart, methodBodySize);
				}
				catch (Exception arg)
				{
					MMDbgLog.Log($"Error executing OnMethodCompiled event: {arg}");
				}
			}
			catch (Exception arg2)
			{
				MMDbgLog.Log($"Error in JitHookCore: {arg2}");
			}
		}

		public static DetourRuntimeNETCorePlatform Create()
		{
			try
			{
				IntPtr jitObject = GetJitObject();
				Guid jitGuid = GetJitGuid(jitObject);
				DetourRuntimeNETCorePlatform detourRuntimeNETCorePlatform = null;
				if (jitGuid == DetourRuntimeNET60Platform.JitVersionGuid)
				{
					detourRuntimeNETCorePlatform = new DetourRuntimeNET60Platform();
				}
				else if (jitGuid == DetourRuntimeNET50Platform.JitVersionGuid)
				{
					detourRuntimeNETCorePlatform = new DetourRuntimeNET50Platform();
				}
				else if (jitGuid == DetourRuntimeNETCore30Platform.JitVersionGuid)
				{
					detourRuntimeNETCorePlatform = new DetourRuntimeNETCore30Platform();
				}
				if (detourRuntimeNETCorePlatform == null)
				{
					return new DetourRuntimeNETCorePlatform();
				}
				detourRuntimeNETCorePlatform?.InstallJitHooks(jitObject);
				return detourRuntimeNETCorePlatform;
			}
			catch (Exception arg)
			{
				MMDbgLog.Log("Could not get JIT information for the runtime, falling out to the version without JIT hooks");
				MMDbgLog.Log($"Error: {arg}");
			}
			return new DetourRuntimeNETCorePlatform();
		}
	}
}
