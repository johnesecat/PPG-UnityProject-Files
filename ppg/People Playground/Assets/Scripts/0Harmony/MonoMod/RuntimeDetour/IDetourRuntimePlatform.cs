using System;
using System.Reflection;

namespace MonoMod.RuntimeDetour
{
	internal interface IDetourRuntimePlatform
	{
		bool OnMethodCompiledWillBeCalled { get; }

		event OnMethodCompiledEvent OnMethodCompiled;

		MethodBase GetIdentifiable(MethodBase method);

		IntPtr GetNativeStart(MethodBase method);

		MethodInfo CreateCopy(MethodBase method);

		bool TryCreateCopy(MethodBase method, out MethodInfo dm);

		void Pin(MethodBase method);

		void Unpin(MethodBase method);

		MethodBase GetDetourTarget(MethodBase from, MethodBase to);

		uint TryMemAllocScratchCloseTo(IntPtr target, out IntPtr ptr, int size);
	}
}
