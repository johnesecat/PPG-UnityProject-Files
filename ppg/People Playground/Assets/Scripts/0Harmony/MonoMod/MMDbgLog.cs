using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace MonoMod
{
	internal static class MMDbgLog
	{
		public static readonly string Tag;

		public static TextWriter Writer;

		public static bool Debugging;

		static MMDbgLog()
		{
			Tag = typeof(MMDbgLog).Assembly.GetName().Name;
			if (Environment.GetEnvironmentVariable("MONOMOD_DBGLOG") == "1" || (Environment.GetEnvironmentVariable("MONOMOD_DBGLOG")?.ToLower(CultureInfo.InvariantCulture)?.Contains(Tag.ToLower(CultureInfo.InvariantCulture), StringComparison.Ordinal)).GetValueOrDefault())
			{
				Start();
			}
		}

		public static void WaitForDebugger()
		{
			if (!Debugging)
			{
				Debugging = true;
				Debugger.Launch();
				Thread.Sleep(6000);
				Debugger.Break();
			}
		}

		public static void Start()
		{
			if (Writer != null)
			{
				return;
			}
			string text = Environment.GetEnvironmentVariable("MONOMOD_DBGLOG_PATH");
			if (text == "-")
			{
				Writer = Console.Out;
				return;
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "mmdbglog.txt";
			}
			text = Path.GetFullPath(Path.GetFileNameWithoutExtension(text) + "-" + Tag + Path.GetExtension(text));
			try
			{
				if (File.Exists(text))
				{
					File.Delete(text);
				}
			}
			catch
			{
			}
			try
			{
				string directoryName = Path.GetDirectoryName(text);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				Writer = new StreamWriter(new FileStream(text, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete), Encoding.UTF8);
			}
			catch
			{
			}
		}

		public static void Log(string str)
		{
			TextWriter writer = Writer;
			if (writer != null)
			{
				writer.WriteLine(str);
				writer.Flush();
			}
		}

		public static T Log<T>(string str, T value)
		{
			TextWriter writer = Writer;
			if (writer == null)
			{
				return value;
			}
			writer.WriteLine(string.Format(CultureInfo.InvariantCulture, str, new object[1] { value }));
			writer.Flush();
			return value;
		}
	}
}
