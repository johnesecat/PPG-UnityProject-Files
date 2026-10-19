using System;
using System.Runtime.CompilerServices;

namespace HarmonyLib
{
	internal class Sandbox
	{
		internal struct SomeStruct_Net
		{
			private readonly byte b1;

			private readonly byte b2;

			private readonly byte b3;
		}

		internal struct SomeStruct_NetLinux
		{
			public unsafe fixed byte headerBytes[17];
		}

		internal struct SomeStruct_Mono
		{
			private readonly byte b1;

			private readonly byte b2;

			private readonly byte b3;

			private readonly byte b4;
		}

		internal static bool hasStructReturnBuffer_Net;

		internal static bool hasStructReturnBuffer_Mono;

		internal static readonly IntPtr magicValue = (IntPtr)305419896;

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal SomeStruct_Net GetStruct_Net(IntPtr x, IntPtr y)
		{
			throw new Exception("This method should've been detoured!");
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal SomeStruct_NetLinux GetStruct_NetLinux(IntPtr x, IntPtr y)
		{
			throw new Exception("This method should've been detoured!");
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal SomeStruct_Mono GetStruct_Mono(IntPtr x, IntPtr y)
		{
			throw new Exception("This method should've been detoured!");
		}

		internal static void GetStructReplacement_Net(Sandbox self, IntPtr ptr, IntPtr a, IntPtr b)
		{
			hasStructReturnBuffer_Net = a == magicValue && b == magicValue;
		}

		internal static void GetStructReplacement_Mono(Sandbox self, IntPtr ptr, IntPtr a, IntPtr b)
		{
			hasStructReturnBuffer_Mono = a == magicValue && b == magicValue;
		}
	}
}
