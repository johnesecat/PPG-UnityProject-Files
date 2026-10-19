using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

[Serializable]
public class ModMetaData
{
	public string Name = "Mod";

	public string Author = "";

	public string Description = "";

	[NonSerialized]
	public string RichReadme;

	public string ModVersion = "1.0";

	public string GameVersion = "1.27.5";

	public string ThumbnailPath = "thumb.png";

	public string EntryPoint = "Mod.Mod";

	public string[] Tags = new string[0];

	public string[] Scripts = new string[0];

	public bool Active = true;

	[NonSerialized]
	public string MetaLocation;

	public string UGCIdentity;

	public string CreatorUGCIdentity;

	[NonSerialized]
	public string MetaPath;

	[NonSerialized]
	public bool HasErrors;

	[NonSerialized]
	public string Errors;

	[JsonIgnore]
	public bool Suspicious { get; internal set; }

	internal bool HasDoNotReuploadSign()
	{
		string fullPath = Path.GetFullPath(Path.Combine(MetaLocation, ThumbnailPath));
		if (!File.Exists(fullPath))
		{
			return false;
		}
		return DoNotReuploadSign.HasMark(fullPath);
	}

	internal string GetUniqueName()
	{
		string text = (string.IsNullOrWhiteSpace(CreatorUGCIdentity) ? "local" : CreatorUGCIdentity);
		return Author.Normalize() + "-" + Name.Normalize() + "-" + text.Normalize();
	}

	internal IEnumerable<string> GetTagsForSteam()
	{
		return (Tags ?? Array.Empty<string>()).Intersect(SteamTags.Tags).Append("Mods");
	}
}
