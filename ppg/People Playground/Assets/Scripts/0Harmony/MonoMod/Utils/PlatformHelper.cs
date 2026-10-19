using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MonoMod.Utils
{
	internal static class PlatformHelper
	{
		private static Platform _current = Platform.Unknown;

		private static bool _currentLocked = false;

		private static string _librarySuffix;

		public static Platform Current
		{
			get
			{
				if (!_currentLocked)
				{
					if (_current == Platform.Unknown)
					{
						DeterminePlatform();
					}
					_currentLocked = true;
				}
				return _current;
			}
			set
			{
				if (_currentLocked)
				{
					throw new InvalidOperationException("Cannot set the value of PlatformHelper.Current once it has been accessed.");
				}
				_current = value;
			}
		}

		public static string LibrarySuffix
		{
			get
			{
				if (_librarySuffix == null)
				{
					_librarySuffix = (Is(Platform.MacOS) ? "dylib" : (Is(Platform.Unix) ? "so" : "dll"));
				}
				return _librarySuffix;
			}
		}

		private static void DeterminePlatform()
		{
			_current = Platform.Unknown;
			PropertyInfo property = typeof(Environment).GetProperty("Platform", BindingFlags.Static | BindingFlags.NonPublic);
			string text = ((!(property != null)) ? Environment.OSVersion.Platform.ToString() : property.GetValue(null, new object[0]).ToString());
			text = text.ToLower(CultureInfo.InvariantCulture);
			if (text.Contains("win"))
			{
				_current = Platform.Windows;
			}
			else if (text.Contains("mac") || text.Contains("osx"))
			{
				_current = Platform.MacOS;
			}
			else if (text.Contains("lin") || text.Contains("unix"))
			{
				_current = Platform.Linux;
			}
			if (_current != Platform.Unknown)
			{
				if (Is(Platform.Linux) && Directory.Exists("/data") && File.Exists("/system/build.prop"))
				{
					_current = Platform.Android;
				}
				else if (Is(Platform.Unix) && Directory.Exists("/Applications") && Directory.Exists("/System") && Directory.Exists("/User") && !Directory.Exists("/Users"))
				{
					_current = Platform.iOS;
				}
				else if (Is(Platform.Windows) && CheckWine())
				{
					_current |= Platform.Wine;
				}
			}
			MethodInfo methodInfo = typeof(Environment).GetProperty("Is64BitOperatingSystem")?.GetGetMethod();
			if (methodInfo != null)
			{
				_current |= (Platform)(((bool)methodInfo.Invoke(null, new object[0])) ? 2 : 0);
			}
			else
			{
				_current |= (Platform)((IntPtr.Size >= 8) ? 2 : 0);
			}
			if (_current != Platform.Unknown && (Is(Platform.Unix) || Is(Platform.Unknown)) && ReflectionHelper.IsMono)
			{
				try
				{
					string text2;
					using (Process process = Process.Start(new ProcessStartInfo("uname", "-m")
					{
						UseShellExecute = false,
						RedirectStandardOutput = true
					}))
					{
						text2 = process.StandardOutput.ReadLine().Trim();
					}
					if (text2.StartsWith("aarch", StringComparison.Ordinal) || text2.StartsWith("arm", StringComparison.Ordinal))
					{
						_current |= Platform.ARM;
					}
				}
				catch (Exception)
				{
				}
			}
			else
			{
				typeof(object).Module.GetPEKind(out var _, out var machine);
				if (machine == ImageFileMachine.ARM)
				{
					_current |= Platform.ARM;
				}
			}
		}

		public static bool Is(Platform platform)
		{
			return (Current & platform) == platform;
		}

		private static bool CheckWine()
		{
			string environmentVariable = Environment.GetEnvironmentVariable("MONOMOD_WINE");
			if (environmentVariable == "1")
			{
				return true;
			}
			if (environmentVariable == "0")
			{
				return false;
			}
			environmentVariable = Environment.GetEnvironmentVariable("XL_WINEONLINUX")?.ToLower(CultureInfo.InvariantCulture);
			if (environmentVariable == "true")
			{
				return true;
			}
			if (environmentVariable == "false")
			{
				return false;
			}
			IntPtr moduleHandle = GetModuleHandle("ntdll.dll");
			if (moduleHandle != IntPtr.Zero && GetProcAddress(moduleHandle, "wine_get_version") != IntPtr.Zero)
			{
				return true;
			}
			return false;
		}

		[DllImport("kernel32", SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
	}
}
