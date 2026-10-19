using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MonoMod.RuntimeDetour.Platforms
{
	internal class DetourNativeLibcPlatform : IDetourNativePlatform
	{
		[Flags]
		private enum MmapProts
		{
			PROT_READ = 0x1,
			PROT_WRITE = 0x2,
			PROT_EXEC = 0x4,
			PROT_NONE = 0x0,
			PROT_GROWSDOWN = 0x1000000,
			PROT_GROWSUP = 0x2000000
		}

		private readonly IDetourNativePlatform Inner;

		private readonly long _Pagesize;

		public DetourNativeLibcPlatform(IDetourNativePlatform inner)
		{
			Inner = inner;
			PropertyInfo property = typeof(Environment).GetProperty("SystemPageSize");
			if (property == null)
			{
				throw new NotSupportedException("Unsupported runtime");
			}
			_Pagesize = (int)property.GetValue(null, new object[0]);
		}

		private void SetMemPerms(IntPtr start, ulong len, MmapProts prot)
		{
			long pagesize = _Pagesize;
			long num = (long)start & ~(pagesize - 1);
			long num2 = ((long)start + (long)len + pagesize - 1) & ~(pagesize - 1);
			if (mprotect((IntPtr)num, (IntPtr)(num2 - num), prot) != 0)
			{
				throw new Win32Exception();
			}
		}

		public void MakeWritable(IntPtr src, uint size)
		{
			SetMemPerms(src, size, MmapProts.PROT_READ | MmapProts.PROT_WRITE | MmapProts.PROT_EXEC);
		}

		public void MakeExecutable(IntPtr src, uint size)
		{
			SetMemPerms(src, size, MmapProts.PROT_READ | MmapProts.PROT_WRITE | MmapProts.PROT_EXEC);
		}

		public void MakeReadWriteExecutable(IntPtr src, uint size)
		{
			SetMemPerms(src, size, MmapProts.PROT_READ | MmapProts.PROT_WRITE | MmapProts.PROT_EXEC);
		}

		public void FlushICache(IntPtr src, uint size)
		{
			Inner.FlushICache(src, size);
		}

		public NativeDetourData Create(IntPtr from, IntPtr to, byte? type)
		{
			return Inner.Create(from, to, type);
		}

		public void Free(NativeDetourData detour)
		{
			Inner.Free(detour);
		}

		public void Apply(NativeDetourData detour)
		{
			Inner.Apply(detour);
		}

		public void Copy(IntPtr src, IntPtr dst, byte type)
		{
			Inner.Copy(src, dst, type);
		}

		public IntPtr MemAlloc(uint size)
		{
			return Inner.MemAlloc(size);
		}

		public void MemFree(IntPtr ptr)
		{
			Inner.MemFree(ptr);
		}

		[DllImport("libc", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
		private static extern int mprotect(IntPtr start, IntPtr len, MmapProts prot);
	}
}
