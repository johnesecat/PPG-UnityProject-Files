using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class ModExtraMetadataLoader
{
	public const string Path = "externalactives";

	public static string Serialise(IEnumerable<string> activeMods)
	{
		return string.Join("\n", activeMods);
	}

	public static HashSet<string> Deserialise(string path)
	{
		if (!File.Exists(path))
		{
			StoreActive();
		}
		return new HashSet<string>(File.ReadAllLines(path));
	}

	public static void StoreActive()
	{
		string contents = Serialise(from m in ModLoader.LoadedMods
			where !string.IsNullOrWhiteSpace(m.UGCIdentity) && m.Active
			select m.UGCIdentity);
		File.WriteAllText("externalactives", contents);
	}
}
