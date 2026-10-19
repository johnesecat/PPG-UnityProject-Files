using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct ItemPersistence
{
	private const string p = "People Playground_Data\\persistence0";

	private static readonly string Key = Environment.MachineName;

	private static readonly List<string> cachedLines = new List<string>();

	internal static void Add(string name)
	{
		if (!Has(name))
		{
			cachedLines.Add(Utils.GetMD5AsString(name + Key));
		}
	}

	internal static bool Has(string name)
	{
		if (cachedLines.Count == 0)
		{
			return false;
		}
		string mD5AsString = Utils.GetMD5AsString(name + Key);
		for (int i = 0; i < cachedLines.Count; i++)
		{
			if (cachedLines[i].StartsWith(mD5AsString, StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	internal static void Serialise()
	{
		File.WriteAllLines("People Playground_Data\\persistence0", cachedLines, Encoding.UTF8);
	}

	internal static void Deserialise()
	{
		if (File.Exists("People Playground_Data\\persistence0"))
		{
			cachedLines.Clear();
			cachedLines.AddRange(File.ReadAllLines("People Playground_Data\\persistence0", Encoding.UTF8));
		}
	}
}
