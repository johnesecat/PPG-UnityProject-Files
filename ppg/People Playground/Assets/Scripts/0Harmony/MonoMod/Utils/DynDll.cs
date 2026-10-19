using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MonoMod.Utils
{
	internal static class DynDll
	{
		public static class DlopenFlags
		{
			public const int RTLD_LAZY = 1;

			public const int RTLD_NOW = 2;

			public const int RTLD_LOCAL = 0;

			public const int RTLD_GLOBAL = 256;
		}

		public static Dictionary<string, List<DynDllMapping>> Mappings;

		private static int dlVersion;

		public static T AsDelegate<T>(this IntPtr s) where T : class
		{
			return Marshal.GetDelegateForFunctionPointer(s, typeof(T)) as T;
		}

		public static void ResolveDynDllImports(this Type type, Dictionary<string, List<DynDllMapping>> mappings = null)
		{
			InternalResolveDynDllImports(type, null, mappings);
		}

		public static void ResolveDynDllImports(object instance, Dictionary<string, List<DynDllMapping>> mappings = null)
		{
			InternalResolveDynDllImports(instance.GetType(), instance, mappings);
		}

		private static void InternalResolveDynDllImports(Type type, object instance, Dictionary<string, List<DynDllMapping>> mappings)
		{
			BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
			bindingFlags = ((instance != null) ? (bindingFlags | BindingFlags.Instance) : (bindingFlags | BindingFlags.Static));
			FieldInfo[] fields = type.GetFields(bindingFlags);
			foreach (FieldInfo fieldInfo in fields)
			{
				bool flag = true;
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(DynDllImportAttribute), inherit: true);
				for (int j = 0; j < customAttributes.Length; j++)
				{
					DynDllImportAttribute dynDllImportAttribute = (DynDllImportAttribute)customAttributes[j];
					flag = false;
					IntPtr libraryPtr = IntPtr.Zero;
					if (mappings != null && mappings.TryGetValue(dynDllImportAttribute.LibraryName, out var value))
					{
						bool flag2 = false;
						foreach (DynDllMapping item in value)
						{
							if (TryOpenLibrary(item.LibraryName, out libraryPtr, skipMapping: true, item.Flags))
							{
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							continue;
						}
					}
					else if (!TryOpenLibrary(dynDllImportAttribute.LibraryName, out libraryPtr))
					{
						continue;
					}
					foreach (string item2 in dynDllImportAttribute.EntryPoints.Concat(new string[2]
					{
						fieldInfo.Name,
						fieldInfo.FieldType.Name
					}))
					{
						if (libraryPtr.TryGetFunction(item2, out var functionPtr))
						{
							fieldInfo.SetValue(instance, Marshal.GetDelegateForFunctionPointer(functionPtr, fieldInfo.FieldType));
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
				if (!flag)
				{
					throw new EntryPointNotFoundException("No matching entry point found for " + fieldInfo.Name + " in " + fieldInfo.DeclaringType.FullName);
				}
			}
		}

		[DllImport("kernel32", SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32", SetLastError = true)]
		private static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport("kernel32", SetLastError = true)]
		private static extern bool FreeLibrary(IntPtr hLibModule);

		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		[DllImport("dl", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "dlopen")]
		private static extern IntPtr dl_dlopen(string filename, int flags);

		[DllImport("dl", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "dlclose")]
		private static extern bool dl_dlclose(IntPtr handle);

		[DllImport("dl", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "dlsym")]
		private static extern IntPtr dl_dlsym(IntPtr handle, string symbol);

		[DllImport("dl", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "dlerror")]
		private static extern IntPtr dl_dlerror();

		[DllImport("libdl.so.2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "dlopen")]
		private static extern IntPtr dl2_dlopen(string filename, int flags);

		[DllImport("libdl.so.2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "dlclose")]
		private static extern bool dl2_dlclose(IntPtr handle);

		[DllImport("libdl.so.2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "dlsym")]
		private static extern IntPtr dl2_dlsym(IntPtr handle, string symbol);

		[DllImport("libdl.so.2", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "dlerror")]
		private static extern IntPtr dl2_dlerror();

		private static IntPtr dlopen(string filename, int flags)
		{
			while (true)
			{
				try
				{
					int num = dlVersion;
					if (num != 0 && num == 1)
					{
						return dl2_dlopen(filename, flags);
					}
					return dl_dlopen(filename, flags);
				}
				catch (DllNotFoundException) when (dlVersion > 0)
				{
					dlVersion--;
				}
			}
		}

		private static bool dlclose(IntPtr handle)
		{
			while (true)
			{
				try
				{
					int num = dlVersion;
					if (num != 0 && num == 1)
					{
						return dl2_dlclose(handle);
					}
					return dl_dlclose(handle);
				}
				catch (DllNotFoundException) when (dlVersion > 0)
				{
					dlVersion--;
				}
			}
		}

		private static IntPtr dlsym(IntPtr handle, string symbol)
		{
			while (true)
			{
				try
				{
					int num = dlVersion;
					if (num != 0 && num == 1)
					{
						return dl2_dlsym(handle, symbol);
					}
					return dl_dlsym(handle, symbol);
				}
				catch (DllNotFoundException) when (dlVersion > 0)
				{
					dlVersion--;
				}
			}
		}

		private static IntPtr dlerror()
		{
			while (true)
			{
				try
				{
					int num = dlVersion;
					if (num != 0 && num == 1)
					{
						return dl2_dlerror();
					}
					return dl_dlerror();
				}
				catch (DllNotFoundException) when (dlVersion > 0)
				{
					dlVersion--;
				}
			}
		}

		static DynDll()
		{
			Mappings = new Dictionary<string, List<DynDllMapping>>();
			dlVersion = 1;
			if (!PlatformHelper.Is(Platform.Windows))
			{
				dlerror();
			}
		}

		private static bool CheckError(out Exception exception)
		{
			if (PlatformHelper.Is(Platform.Windows))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error != 0)
				{
					exception = new Win32Exception(lastWin32Error);
					return false;
				}
			}
			else
			{
				IntPtr intPtr = dlerror();
				if (intPtr != IntPtr.Zero)
				{
					exception = new Win32Exception(Marshal.PtrToStringAnsi(intPtr));
					return false;
				}
			}
			exception = null;
			return true;
		}

		public static IntPtr OpenLibrary(string name, bool skipMapping = false, int? flags = null)
		{
			if (!InternalTryOpenLibrary(name, out var libraryPtr, skipMapping, flags))
			{
				throw new DllNotFoundException("Unable to load library '" + name + "'");
			}
			if (!CheckError(out var exception))
			{
				throw exception;
			}
			return libraryPtr;
		}

		public static bool TryOpenLibrary(string name, out IntPtr libraryPtr, bool skipMapping = false, int? flags = null)
		{
			Exception exception;
			if (!InternalTryOpenLibrary(name, out libraryPtr, skipMapping, flags))
			{
				return CheckError(out exception);
			}
			return true;
		}

		private static bool InternalTryOpenLibrary(string name, out IntPtr libraryPtr, bool skipMapping, int? flags)
		{
			if (name != null && !skipMapping && Mappings.TryGetValue(name, out var value))
			{
				foreach (DynDllMapping item in value)
				{
					if (InternalTryOpenLibrary(item.LibraryName, out libraryPtr, skipMapping: true, item.Flags))
					{
						return true;
					}
				}
				libraryPtr = IntPtr.Zero;
				return true;
			}
			if (PlatformHelper.Is(Platform.Windows))
			{
				libraryPtr = ((name == null) ? GetModuleHandle(name) : LoadLibrary(name));
			}
			else
			{
				int flags2 = flags ?? 258;
				libraryPtr = dlopen(name, flags2);
				if (libraryPtr == IntPtr.Zero && File.Exists(name))
				{
					libraryPtr = dlopen(Path.GetFullPath(name), flags2);
				}
			}
			return libraryPtr != IntPtr.Zero;
		}

		public static bool CloseLibrary(IntPtr lib)
		{
			if (PlatformHelper.Is(Platform.Windows))
			{
				CloseLibrary(lib);
			}
			else
			{
				dlclose(lib);
			}
			Exception exception;
			return CheckError(out exception);
		}

		public static IntPtr GetFunction(this IntPtr libraryPtr, string name)
		{
			if (!InternalTryGetFunction(libraryPtr, name, out var functionPtr))
			{
				throw new MissingMethodException("Unable to load function '" + name + "'");
			}
			if (!CheckError(out var exception))
			{
				throw exception;
			}
			return functionPtr;
		}

		public static bool TryGetFunction(this IntPtr libraryPtr, string name, out IntPtr functionPtr)
		{
			Exception exception;
			if (!InternalTryGetFunction(libraryPtr, name, out functionPtr))
			{
				return CheckError(out exception);
			}
			return true;
		}

		private static bool InternalTryGetFunction(IntPtr libraryPtr, string name, out IntPtr functionPtr)
		{
			if (libraryPtr == IntPtr.Zero)
			{
				throw new ArgumentNullException("libraryPtr");
			}
			functionPtr = (PlatformHelper.Is(Platform.Windows) ? GetProcAddress(libraryPtr, name) : dlsym(libraryPtr, name));
			return functionPtr != IntPtr.Zero;
		}
	}
}
