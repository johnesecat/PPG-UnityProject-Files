using System;
using System.Runtime.InteropServices;

namespace NAudio.Utils
{
	internal class NativeMethods
	{
		[DllImport("kernel32.dll")]
		public static extern global::System.IntPtr LoadLibrary(string dllToLoad);

		[DllImport("kernel32.dll")]
		public static extern global::System.IntPtr GetProcAddress(global::System.IntPtr hModule, string procedureName);

		[DllImport("kernel32.dll")]
		public static extern bool FreeLibrary(global::System.IntPtr hModule);
	}
}
