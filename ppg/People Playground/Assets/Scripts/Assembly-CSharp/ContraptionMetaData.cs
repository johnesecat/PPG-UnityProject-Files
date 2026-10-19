using System;
using System.Collections.Generic;
using System.IO;

[Serializable]
public class ContraptionMetaData
{
	public const string FileExtension = ".json";

	public string Name;

	public string DisplayName;

	public string Version;

	public ulong PublishedFileID;

	public string CreatorUGCIdentity;

	[NonSerialized]
	public List<string> Tags = new List<string>();

	public string PathToMetadata;

	[NonSerialized]
	public string PathToDataFile;

	[NonSerialized]
	public string PathToThumbnail;

	[NonSerialized]
	public string PathToOutlineFile;

	public List<RequiredMod> RequiredMods = new List<RequiredMod>();

	public bool IsCurrentVersion => Version == "1.27.5";

	public bool IsWorkshopItem => PublishedFileID != 0;

	public bool RequiresMods
	{
		get
		{
			if (RequiredMods != null)
			{
				return RequiredMods.Count > 0;
			}
			return false;
		}
	}

	public ContraptionMetaData()
	{
	}

	public ContraptionMetaData(string name)
	{
		Name = name;
		DisplayName = Name;
		Version = "1.27.5";
	}

	public static string GetPath(string name)
	{
		string text = "Contraptions/" + name + ".json";
		if (File.Exists(text))
		{
			return text;
		}
		return "Contraptions/" + name + "/" + name + ".json";
	}

	public bool IsLegacyStructure()
	{
		return File.Exists("Contraptions/" + Name + ".json");
	}

	public long GetSizeOnDisk()
	{
		long byteCount = 0L;
		count(PathToMetadata);
		count(PathToDataFile);
		count(PathToThumbnail);
		count(PathToOutlineFile);
		return byteCount;
		void count(string path)
		{
			if (File.Exists(path))
			{
				FileInfo fileInfo = new FileInfo(path);
				byteCount += fileInfo.Length;
			}
		}
	}
}
