using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour.Platforms
{
	internal class DetourNativeMonoPlatform : IDetourNativePlatform
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int d_mono_pagesize();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
		private delegate int d_mono_mprotect(IntPtr addr, IntPtr length, int flags);

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

		[DynDllImport("mono", new string[] { })]
		private d_mono_pagesize mono_pagesize;

		[DynDllImport("mono", new string[] { })]
		private d_mono_mprotect mono_mprotect;

		public DetourNativeMonoPlatform(IDetourNativePlatform inner, string libmono)
		{
			Inner = inner;
			Dictionary<string, List<DynDllMapping>> dictionary = new Dictionary<string, List<DynDllMapping>>();
			if (!string.IsNullOrEmpty(libmono))
			{
				dictionary.Add("mono", new List<DynDllMapping> { libmono });
			}
			DynDll.ResolveDynDllImports(this, dictionary);
			_Pagesize = mono_pagesize();
		}

		private void SetMemPerms(IntPtr start, ulong len, MmapProts prot)
		{
			long pagesize = _Pagesize;
			long num = (long)start & ~(pagesize - 1);
			long num2 = ((long)start + (long)len + pagesize - 1) & ~(pagesize - 1);
			if (mono_mprotect((IntPtr)num, (IntPtr)(num2 - num), (int)prot) != 0 && Marshal.GetLastWin32Error() != 0)
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
	}
}
