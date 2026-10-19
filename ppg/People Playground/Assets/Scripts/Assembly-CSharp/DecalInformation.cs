using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[Obsolete]
public struct DecalInformation
{
	[Obsolete]
	public enum DecalIndex
	{
		HumanBlood,
		AlienBlood,
		BlastMark
	}

	public Color Color;

	public string Path;

	public float IgnoreRadius;

	public static readonly DecalInformation[] Decals = new DecalInformation[3]
	{
		new DecalInformation("Sprites/Decals/Blood", new Color(0.5660378f, 0f, 0f)),
		new DecalInformation("Sprites/Decals/Blood", new Color(0f, 0.4f, 0.17f)),
		new DecalInformation("Sprites/Decals/BlastMark", new Color(1f, 1f, 1f))
	};

	public static Dictionary<string, Sprite[]> SpriteCache = new Dictionary<string, Sprite[]>();

	public static DecalInformation HumanBlood => Decals[0];

	public static DecalInformation AlienBlood => Decals[1];

	public static DecalInformation BlastMark => Decals[2];

	public DecalInformation(string path, Color color, float ignoreRadius = 0.1f)
	{
		Color = color;
		Path = path;
		IgnoreRadius = ignoreRadius;
	}

	public static DecalInformation Get(DecalIndex index)
	{
		return Decals[(int)index];
	}

	public static Sprite[] GetSprites(string path)
	{
		if (SpriteCache.TryGetValue(path, out var value))
		{
			return value;
		}
		Sprite[] array = Resources.LoadAll<Sprite>(path);
		SpriteCache.Add(path, array);
		return array;
	}

	public override bool Equals(object obj)
	{
		if (obj is DecalInformation)
		{
			DecalInformation decalInformation = (DecalInformation)obj;
			if (Color.Equals(decalInformation.Color) && Path == decalInformation.Path)
			{
				return IgnoreRadius == decalInformation.IgnoreRadius;
			}
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((-1088103025 * -1521134295 + EqualityComparer<Color>.Default.GetHashCode(Color)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Path)) * -1521134295 + IgnoreRadius.GetHashCode();
	}

	public static bool operator ==(DecalInformation left, DecalInformation right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(DecalInformation left, DecalInformation right)
	{
		return !(left == right);
	}
}
