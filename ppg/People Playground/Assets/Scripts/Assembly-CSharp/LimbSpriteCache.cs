using System;
using System.Collections.Generic;
using UnityEngine;

public class LimbSpriteCache : MonoBehaviour
{
	public readonly struct LimbSprites : IEquatable<LimbSprites>
	{
		public readonly Sprite Skin;

		public readonly Sprite Flesh;

		public readonly Sprite Bone;

		public LimbSprites(Sprite skin, Sprite flesh, Sprite bone)
		{
			Skin = skin;
			Flesh = flesh;
			Bone = bone;
		}

		public override bool Equals(object obj)
		{
			if (obj is LimbSprites)
			{
				LimbSprites other = (LimbSprites)obj;
				return Equals(other);
			}
			return false;
		}

		public bool Equals(LimbSprites other)
		{
			if (EqualityComparer<Sprite>.Default.Equals(Skin, other.Skin) && EqualityComparer<Sprite>.Default.Equals(Flesh, other.Flesh))
			{
				return EqualityComparer<Sprite>.Default.Equals(Bone, other.Bone);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((-721118468 * -1521134295 + EqualityComparer<Sprite>.Default.GetHashCode(Skin)) * -1521134295 + EqualityComparer<Sprite>.Default.GetHashCode(Flesh)) * -1521134295 + EqualityComparer<Sprite>.Default.GetHashCode(Bone);
		}

		public static bool operator ==(LimbSprites left, LimbSprites right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(LimbSprites left, LimbSprites right)
		{
			return !(left == right);
		}
	}

	public readonly struct Key : IEquatable<Key>
	{
		public readonly Sprite Original;

		public readonly Texture2D Skin;

		public readonly Texture2D Flesh;

		public readonly Texture2D Bone;

		public readonly float Scale;

		public Key(Sprite original, Texture2D skin, Texture2D flesh, Texture2D bone, float scale)
		{
			Original = original;
			Skin = skin;
			Flesh = flesh;
			Bone = bone;
			Scale = scale;
		}

		public override bool Equals(object obj)
		{
			if (obj is Key)
			{
				Key other = (Key)obj;
				return Equals(other);
			}
			return false;
		}

		public bool Equals(Key other)
		{
			if (EqualityComparer<Sprite>.Default.Equals(Original, other.Original) && EqualityComparer<Texture2D>.Default.Equals(Skin, other.Skin) && EqualityComparer<Texture2D>.Default.Equals(Flesh, other.Flesh) && EqualityComparer<Texture2D>.Default.Equals(Bone, other.Bone))
			{
				return Mathf.Approximately(Scale, other.Scale);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((((969546047 * -1521134295 + EqualityComparer<Sprite>.Default.GetHashCode(Original)) * -1521134295 + EqualityComparer<Texture2D>.Default.GetHashCode(Skin)) * -1521134295 + EqualityComparer<Texture2D>.Default.GetHashCode(Flesh)) * -1521134295 + EqualityComparer<Texture2D>.Default.GetHashCode(Bone)) * -1521134295 + Mathf.RoundToInt(Scale * 10000f).GetHashCode();
		}

		public static bool operator ==(Key left, Key right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Key left, Key right)
		{
			return !(left == right);
		}
	}

	public Dictionary<Key, LimbSprites> Sprites = new Dictionary<Key, LimbSprites>();

	public static LimbSpriteCache Instance { get; set; }

	private void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		foreach (LimbSprites value in Sprites.Values)
		{
			UnityEngine.Object.Destroy(value.Skin);
			UnityEngine.Object.Destroy(value.Flesh);
			UnityEngine.Object.Destroy(value.Bone);
		}
	}

	public LimbSprites LoadFor(Sprite original, Texture2D skin, Texture2D flesh, Texture2D bone, float scale)
	{
		Key key = new Key(original, skin, flesh, bone, scale);
		if (Sprites.TryGetValue(key, out var value))
		{
			return value;
		}
		value = new LimbSprites(generateFor(skin, original), flesh ? generateFor(flesh, original) : null, bone ? generateFor(bone, original) : null);
		Sprites.Add(key, value);
		Debug.LogFormat("Created custom limb sprites: {0} {1} {2}", skin.name, flesh ? flesh.name : "NULL", bone ? bone.name : "NULL");
		return value;
		Sprite generateFor(Texture2D t, Sprite original)
		{
			return Sprite.Create(t, new Rect(original.rect.position * scale, original.rect.size * scale), new Vector2(0.5f, 0.5f), original.pixelsPerUnit * scale, 0u, SpriteMeshType.FullRect, original.border * scale, generateFallbackPhysicsShape: false);
		}
	}
}
