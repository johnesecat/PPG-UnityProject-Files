using System;
using System.Buffers;
using System.Collections.Generic;
using UnityEngine;

public static class ShatteredColliderShapeCache
{
	public struct Shard : IEquatable<Shard>
	{
		public byte MinRange;

		public byte Size;

		public int TextureAssetId;

		public override bool Equals(object obj)
		{
			if (obj is Shard)
			{
				Shard other = (Shard)obj;
				return Equals(other);
			}
			return false;
		}

		public bool Equals(Shard other)
		{
			if (MinRange == other.MinRange && Size == other.Size)
			{
				return TextureAssetId == other.TextureAssetId;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((-1239005421 * -1521134295 + MinRange.GetHashCode()) * -1521134295 + Size.GetHashCode()) * -1521134295 + TextureAssetId.GetHashCode();
		}

		public static bool operator ==(Shard left, Shard right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Shard left, Shard right)
		{
			return !(left == right);
		}
	}

	public static Dictionary<Shard, Vector2[]> Polygons = new Dictionary<Shard, Vector2[]>();

	private static readonly List<Vector2> resultBuffer = new List<Vector2>();

	public static Vector2[] GetPolygon(Texture2D texture, byte min, byte size)
	{
		Shard shard = default(Shard);
		shard.Size = size;
		shard.MinRange = min;
		shard.TextureAssetId = texture.GetInstanceID();
		Shard key = shard;
		if (Polygons.TryGetValue(key, out var value))
		{
			return value;
		}
		Vector2[] array = ArrayPool<Vector2>.Shared.Rent(texture.width * texture.height);
		Color[] pixels = texture.GetPixels();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < texture.height; i++)
		{
			for (int j = 0; j < texture.width; j++)
			{
				byte b = (byte)(pixels[num2++].r * 255f);
				if (b > min && b < min + size)
				{
					array[num++] = new Vector2((float)j / (float)texture.width - 0.5f, (float)i / (float)texture.height - 0.5f);
				}
			}
		}
		Vector2[] array2 = new Vector2[num];
		Array.Copy(array, array2, num);
		Utils.ComputeConvexHull(array2, resultBuffer);
		ArrayPool<Vector2>.Shared.Return(array);
		Vector2[] array3 = resultBuffer.ToArray();
		Polygons.Add(key, array3);
		return array3;
	}

	public static void Release()
	{
		Polygons.Clear();
	}
}
