using System;
using System.Runtime.InteropServices;

namespace MonoMod.RuntimeDetour.Platforms
{
	internal class DetourNativeARMPlatform : IDetourNativePlatform
	{
		public enum DetourType : byte
		{
			Thumb,
			ThumbBX,
			AArch32,
			AArch32BX,
			AArch64
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int d_flushicache(IntPtr code, ulong size);

		private static readonly uint[] DetourSizes = new uint[5] { 8u, 12u, 8u, 12u, 16u };

		public bool ShouldFlushICache = true;

		private readonly byte[] _FlushCache32 = new byte[44]
		{
			128, 64, 45, 233, 0, 48, 160, 225, 1, 192,
			128, 224, 20, 224, 159, 229, 3, 0, 160, 225,
			12, 16, 160, 225, 14, 112, 160, 225, 0, 32,
			160, 227, 0, 0, 0, 239, 128, 128, 189, 232,
			2, 0, 15, 0
		};

		private readonly byte[] _FlushCache64 = new byte[76]
		{
			1, 0, 1, 139, 0, 244, 126, 146, 63, 0,
			0, 235, 201, 0, 0, 84, 226, 3, 0, 170,
			34, 126, 11, 213, 66, 16, 0, 145, 63, 0,
			2, 235, 168, 255, 255, 84, 159, 59, 3, 213,
			63, 0, 0, 235, 169, 0, 0, 84, 32, 117,
			11, 213, 0, 16, 0, 145, 63, 0, 0, 235,
			168, 255, 255, 84, 159, 59, 3, 213, 223, 63,
			3, 213, 192, 3, 95, 214
		};

		private static DetourType GetDetourType(IntPtr from, IntPtr to)
		{
			if (IntPtr.Size >= 8)
			{
				return DetourType.AArch64;
			}
			bool num = ((long)from & 1) == 1;
			bool flag = ((long)to & 1) == 1;
			if (num)
			{
				if (flag)
				{
					return DetourType.Thumb;
				}
				return DetourType.ThumbBX;
			}
			if (flag)
			{
				return DetourType.AArch32BX;
			}
			return DetourType.AArch32;
		}

		public NativeDetourData Create(IntPtr from, IntPtr to, byte? type)
		{
			NativeDetourData nativeDetourData = default(NativeDetourData);
			nativeDetourData.Method = (IntPtr)((long)from & -2);
			nativeDetourData.Target = (IntPtr)((long)to & -2);
			NativeDetourData result = nativeDetourData;
			uint[] detourSizes = DetourSizes;
			int num = ((int?)type) ?? ((int)GetDetourType(from, to));
			byte b = (byte)num;
			result.Type = (byte)num;
			result.Size = detourSizes[b];
			return result;
		}

		public void Free(NativeDetourData detour)
		{
		}

		public void Apply(NativeDetourData detour)
		{
			int offs = 0;
			switch (detour.Type)
			{
			case 0:
				detour.Method.Write(ref offs, 223);
				detour.Method.Write(ref offs, 248);
				detour.Method.Write(ref offs, 0);
				detour.Method.Write(ref offs, 240);
				detour.Method.Write(ref offs, (uint)(int)detour.Target | 1u);
				break;
			case 1:
				detour.Method.Write(ref offs, 223);
				detour.Method.Write(ref offs, 248);
				detour.Method.Write(ref offs, 4);
				detour.Method.Write(ref offs, 160);
				detour.Method.Write(ref offs, 80);
				detour.Method.Write(ref offs, 71);
				detour.Method.Write(ref offs, 0);
				detour.Method.Write(ref offs, 191);
				detour.Method.Write(ref offs, (uint)(int)detour.Target | 0u);
				break;
			case 2:
				detour.Method.Write(ref offs, 4);
				detour.Method.Write(ref offs, 240);
				detour.Method.Write(ref offs, 31);
				detour.Method.Write(ref offs, 229);
				detour.Method.Write(ref offs, (uint)(int)detour.Target | 0u);
				break;
			case 3:
				detour.Method.Write(ref offs, 0);
				detour.Method.Write(ref offs, 128);
				detour.Method.Write(ref offs, 159);
				detour.Method.Write(ref offs, 229);
				detour.Method.Write(ref offs, 24);
				detour.Method.Write(ref offs, byte.MaxValue);
				detour.Method.Write(ref offs, 47);
				detour.Method.Write(ref offs, 225);
				detour.Method.Write(ref offs, (uint)(int)detour.Target | 1u);
				break;
			case 4:
				detour.Method.Write(ref offs, 79);
				detour.Method.Write(ref offs, 0);
				detour.Method.Write(ref offs, 0);
				detour.Method.Write(ref offs, 88);
				detour.Method.Write(ref offs, 224);
				detour.Method.Write(ref offs, 1);
				detour.Method.Write(ref offs, 31);
				detour.Method.Write(ref offs, 214);
				detour.Method.Write(ref offs, (ulong)(long)detour.Target);
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
				*(int*)((long)dst + 4) = *(int*)((long)src + 4);
				break;
			case 1:
				*(int*)(long)dst = *(int*)(long)src;
				*(short*)((long)dst + 4) = *(short*)((long)src + 4);
				*(short*)((long)dst + 6) = *(short*)((long)src + 6);
				*(int*)((long)dst + 8) = *(int*)((long)src + 8);
				break;
			case 2:
				*(int*)(long)dst = *(int*)(long)src;
				*(int*)((long)dst + 4) = *(int*)((long)src + 4);
				break;
			case 3:
				*(int*)(long)dst = *(int*)(long)src;
				*(int*)((long)dst + 4) = *(int*)((long)src + 4);
				*(int*)((long)dst + 8) = *(int*)((long)src + 8);
				break;
			case 4:
				*(int*)(long)dst = *(int*)(long)src;
				*(int*)((long)dst + 4) = *(int*)((long)src + 4);
				*(long*)((long)dst + 8) = *(long*)((long)src + 8);
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

		public unsafe void FlushICache(IntPtr src, uint size)
		{
			if (ShouldFlushICache)
			{
				byte[] array = ((IntPtr.Size >= 8) ? _FlushCache64 : _FlushCache32);
				fixed (byte* ptr = array)
				{
					DetourHelper.Native.MakeExecutable((IntPtr)ptr, (uint)array.Length);
					(Marshal.GetDelegateForFunctionPointer((IntPtr)ptr, typeof(d_flushicache)) as d_flushicache)(src, size);
				}
			}
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
