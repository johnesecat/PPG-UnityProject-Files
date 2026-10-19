using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MonoMod.RuntimeDetour.Platforms
{
	internal class DetourNativeWindowsPlatform : IDetourNativePlatform
	{
		[Flags]
		private enum PAGE : uint
		{
			UNSET = 0x0u,
			NOACCESS = 0x1u,
			READONLY = 0x2u,
			READWRITE = 0x4u,
			WRITECOPY = 0x8u,
			EXECUTE = 0x10u,
			EXECUTE_READ = 0x20u,
			EXECUTE_READWRITE = 0x40u,
			EXECUTE_WRITECOPY = 0x80u,
			GUARD = 0x100u,
			NOCACHE = 0x200u,
			WRITECOMBINE = 0x400u
		}

		private enum MEM : uint
		{
			UNSET = 0u,
			MEM_COMMIT = 0x1000u,
			MEM_RESERVE = 0x2000u,
			MEM_FREE = 0x10000u,
			MEM_PRIVATE = 0x20000u,
			MEM_MAPPED = 0x40000u,
			MEM_IMAGE = 0x1000000u
		}

		private struct MEMORY_BASIC_INFORMATION
		{
			public IntPtr BaseAddress;

			public IntPtr AllocationBase;

			public PAGE AllocationProtect;

			public IntPtr RegionSize;

			public MEM State;

			public PAGE Protect;

			public MEM Type;
		}

		private readonly IDetourNativePlatform Inner;

		public DetourNativeWindowsPlatform(IDetourNativePlatform inner)
		{
			Inner = inner;
		}

		public void MakeWritable(IntPtr src, uint size)
		{
			if (!VirtualProtect(src, (IntPtr)size, PAGE.EXECUTE_READWRITE, out var _))
			{
				throw LogAllSections(Marshal.GetLastWin32Error(), "MakeWriteable", src, size);
			}
		}

		public void MakeExecutable(IntPtr src, uint size)
		{
			if (!VirtualProtect(src, (IntPtr)size, PAGE.EXECUTE_READWRITE, out var _))
			{
				throw LogAllSections(Marshal.GetLastWin32Error(), "MakeExecutable", src, size);
			}
		}

		public void MakeReadWriteExecutable(IntPtr src, uint size)
		{
			if (!VirtualProtect(src, (IntPtr)size, PAGE.EXECUTE_READWRITE, out var _))
			{
				throw LogAllSections(Marshal.GetLastWin32Error(), "MakeExecutable", src, size);
			}
		}

		public void FlushICache(IntPtr src, uint size)
		{
			if (!FlushInstructionCache(GetCurrentProcess(), src, (UIntPtr)size))
			{
				throw LogAllSections(Marshal.GetLastWin32Error(), "FlushICache", src, size);
			}
		}

		private unsafe Exception LogAllSections(int error, string from, IntPtr src, uint size)
		{
			Exception ex = new Win32Exception(error);
			if (MMDbgLog.Writer == null)
			{
				return ex;
			}
			MMDbgLog.Log($"{from} failed for 0x{(long)src:X16} + {size} - logging all memory sections");
			MMDbgLog.Log("reason: " + ex.Message);
			try
			{
				IntPtr currentProcess = GetCurrentProcess();
				IntPtr intPtr = (IntPtr)65536;
				int num = 0;
				MEMORY_BASIC_INFORMATION lpBuffer;
				while (VirtualQueryEx(currentProcess, intPtr, out lpBuffer, sizeof(MEMORY_BASIC_INFORMATION)) != 0)
				{
					ulong num2 = (ulong)(long)src;
					ulong num3 = num2 + size;
					long num4 = (long)lpBuffer.BaseAddress;
					ulong num5 = (ulong)(num4 + (long)lpBuffer.RegionSize);
					bool flag = (ulong)num4 <= num3 && num2 <= num5;
					MMDbgLog.Log(string.Format("{0} #{1}", flag ? "*" : "-", num++));
					MMDbgLog.Log($"addr: 0x{(long)lpBuffer.BaseAddress:X16}");
					MMDbgLog.Log($"size: 0x{(long)lpBuffer.RegionSize:X16}");
					MMDbgLog.Log($"aaddr: 0x{(long)lpBuffer.AllocationBase:X16}");
					MMDbgLog.Log($"state: {lpBuffer.State}");
					MMDbgLog.Log($"type: {lpBuffer.Type}");
					MMDbgLog.Log($"protect: {lpBuffer.Protect}");
					MMDbgLog.Log($"aprotect: {lpBuffer.AllocationProtect}");
					try
					{
						IntPtr intPtr2 = intPtr;
						intPtr = (IntPtr)((long)lpBuffer.BaseAddress + (long)lpBuffer.RegionSize);
						if ((ulong)(long)intPtr > (ulong)(long)intPtr2)
						{
							continue;
						}
						return ex;
					}
					catch (OverflowException)
					{
						MMDbgLog.Log("overflow");
						return ex;
					}
				}
				return ex;
			}
			catch
			{
				throw ex;
			}
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

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool VirtualProtect(IntPtr lpAddress, IntPtr dwSize, PAGE flNewProtect, out PAGE lpflOldProtect);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetCurrentProcess();

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool FlushInstructionCache(IntPtr hProcess, IntPtr lpBaseAddress, UIntPtr dwSize);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, int dwLength);
	}
}
