using System;
using System.Runtime.InteropServices;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour.Platforms
{
	internal class DetourRuntimeNET60Platform : DetourRuntimeNETCore30Platform
	{
		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private unsafe delegate CorJitResult d_compileMethod_thiscall(IntPtr thisPtr, IntPtr corJitInfo, in CORINFO_METHOD_INFO methodInfo, uint flags, out byte* nativeEntry, out uint nativeSizeOfCode);

		public new static readonly Guid JitVersionGuid = new Guid("5ed35c58-857b-48dd-a818-7c0136dc9f73");

		private d_compileMethod_thiscall our_compileMethod;

		private d_compileMethod_thiscall real_compileMethod;

		protected unsafe override CorJitResult InvokeRealCompileMethod(IntPtr thisPtr, IntPtr corJitInfo, in CORINFO_METHOD_INFO methodInfo, uint flags, out byte* nativeEntry, out uint nativeSizeOfCode)
		{
			if (real_compileMethod == null)
			{
				return base.InvokeRealCompileMethod(thisPtr, corJitInfo, in methodInfo, flags, out nativeEntry, out nativeSizeOfCode);
			}
			return real_compileMethod(thisPtr, corJitInfo, in methodInfo, flags, out nativeEntry, out nativeSizeOfCode);
		}

		protected unsafe override IntPtr GetCompileMethodHook(IntPtr real)
		{
			if (PlatformHelper.Is(Platform.Windows) && IntPtr.Size == 4)
			{
				real_compileMethod = real.AsDelegate<d_compileMethod_thiscall>();
				our_compileMethod = base.CompileMethodHook;
				IntPtr functionPointerForDelegate = Marshal.GetFunctionPointerForDelegate(our_compileMethod);
				NativeDetourData data = DetourRuntimeNETCore30Platform.CreateNativeTrampolineTo(functionPointerForDelegate);
				d_compileMethod_thiscall d_compileMethod_thiscall = data.Method.AsDelegate<d_compileMethod_thiscall>();
				IntPtr zero = IntPtr.Zero;
				IntPtr zero2 = IntPtr.Zero;
				CORINFO_METHOD_INFO methodInfo = default(CORINFO_METHOD_INFO);
				d_compileMethod_thiscall(zero, zero2, in methodInfo, 0u, out var _, out var _);
				DetourRuntimeNETCore30Platform.FreeNativeTrampoline(data);
				return functionPointerForDelegate;
			}
			return base.GetCompileMethodHook(real);
		}
	}
}
