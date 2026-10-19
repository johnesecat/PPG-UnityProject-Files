using System;

[Serializable]
public class TexturePack
{
	public string Name;

	public string Author;

	public string Description;

	public string Thumbnail;

	public string GameVersion;

	public string Version;

	public float ResolutionMultiplier = 1f;

	public TextureOverride[] Overrides;

	[NonSerialized]
	public string MetaLocation;
}
