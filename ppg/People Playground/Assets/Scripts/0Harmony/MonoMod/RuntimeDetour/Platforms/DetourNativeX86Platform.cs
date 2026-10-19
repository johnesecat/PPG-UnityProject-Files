using System;
using System.Runtime.InteropServices;

namespace MonoMod.RuntimeDetour.Platforms
{
	internal class DetourNativeX86Platform : IDetourNativePlatform
	{
		public enum DetourType : byte
		{
			Rel32,
			Abs32,
			Abs64,
			Abs64Split
		}

		private static readonly uint[] DetourSizes = new uint[4] { 5u, 6u, 14u, 6u };

		private static bool Is32Bit(long to)
		{
			return (to & 0x7FFFFFFF) == to;
		}

		private unsafe static DetourType GetDetourType(IntPtr from, IntPtr to, ref IntPtr extra)
		{
			long num = (long)to - ((long)from + 5);
			if ((Is32Bit(num) || Is32Bit(-num)) && ((byte*)(void*)from)[5] != 95)
			{
				return DetourType.Rel32;
			}
			if (Is32Bit((long)to))
			{
				return DetourType.Abs32;
			}
			if ((DetourHelper.Runtime?.TryMemAllocScratchCloseTo(from, out extra, 8) ?? 0) >= 8)
			{
				num = (long)extra - ((long)from + 6);
				if (Is32Bit(num) || Is32Bit(-num))
				{
					return DetourType.Abs64Split;
				}
			}
			return DetourType.Abs64;
		}

		public NativeDetourData Create(IntPtr from, IntPtr to, byte? type)
		{
			NativeDetourData nativeDetourData = default(NativeDetourData);
			nativeDetourData.Method = from;
			nativeDetourData.Target = to;
			NativeDetourData result = nativeDetourData;
			uint[] detourSizes = DetourSizes;
			int num = ((int?)type) ?? ((int)GetDetourType(from, to, ref result.Extra));
			byte b = (byte)num;
			result.Type = (byte)num;
			result.Size = detourSizes[b];
			return result;
		}

		public void Free(NativeDetourData detour)
		{
			_ = detour.Type;
			_ = 3;
		}

		public void Apply(NativeDetourData detour)
		{
			int offs = 0;
			switch (detour.Type)
			{
			case 0:
				detour.Method.Write(ref offs, 233);
				detour.Method.Write(ref offs, (uint)((long)detour.Target - ((long)detour.Method + offs + 4)));
				break;
			case 1:
				detour.Method.Write(ref offs, 104);
				detour.Method.Write(ref offs, (uint)(int)detour.Target);
				detour.Method.Write(ref offs, 195);
				break;
			case 2:
			case 3:
				detour.Method.Write(ref offs, byte.MaxValue);
				detour.Method.Write(ref offs, 37);
				if (detour.Type == 3)
				{
					detour.Method.Write(ref offs, (uint)((long)detour.Extra - ((long)detour.Method + offs + 4)));
					offs = 0;
					detour.Extra.Write(ref offs, (ulong)(long)detour.Target);
				}
				else
				{
					detour.Method.Write(ref offs, 0u);
					detour.Method.Write(ref offs, (ulong)(long)detour.Target);
				}
				break;
			default:
				throw new NotSupportedException($"Unknown detour type {detour.Type}");
			}
		}

		public unsafe void Copy(IntPtr src, IntPtr dst, byte type)
		{
			switch (type)
			{
			case 0:
				*(int*)(long)dst = *(int*)(long)src;
				*(sbyte*)((long)dst + 4) = *(sbyte*)((long)src + 4);
				break;
			case 1:
			case 3:
				*(int*)(long)dst = *(int*)(long)src;
				*(short*)((long)dst + 4) = *(short*)((long)src + 4);
				break;
			case 2:
				*(long*)(long)dst = *(long*)(long)src;
				*(int*)((long)dst + 8) = *(int*)((long)src + 8);
				*(short*)((long)dst + 12) = *(short*)((long)src + 12);
				break;
			default:
				throw new NotSupportedException($"Unknown detour type {type}");
			}
		}

		public void MakeWritable(IntPtr src, uint size)
		{
		}

		public void MakeExecutable(IntPtr src, uint size)
		{
		}

		public void MakeReadWriteExecutable(IntPtr src, uint size)
		{
		}

		public void FlushICache(IntPtr src, uint size)
		{
		}

		public IntPtr MemAlloc(uint size)
		{
			return Marshal.AllocHGlobal((int)size);
		}

		public void MemFree(IntPtr ptr)
		{
			Marshal.FreeHGlobal(ptr);
		}
	}
}
