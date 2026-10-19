using System;
using UnityEngine;

public readonly struct DamagePoint : IEquatable<DamagePoint>
{
	public readonly float X;

	public readonly float Y;

	public readonly float Intensity;

	public readonly DamageType Type;

	public static readonly DamagePoint None = new DamagePoint(0f, 0f, 0f, DamageType.None);

	public DamagePoint(float x, float y, float intensity, DamageType type)
	{
		X = x;
		Y = y;
		Intensity = intensity;
		Type = type;
	}

	public override bool Equals(object obj)
	{
		if (obj is DamagePoint)
		{
			DamagePoint other = (DamagePoint)obj;
			return Equals(other);
		}
		return false;
	}

	public bool Equals(DamagePoint other)
	{
		if (X == other.X && Y == other.Y && Intensity == other.Intensity)
		{
			return Type == other.Type;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((252580816 * -1521134295 + X.GetHashCode()) * -1521134295 + Y.GetHashCode()) * -1521134295 + Intensity.GetHashCode()) * -1521134295 + Type.GetHashCode();
	}

	public static bool operator ==(DamagePoint left, DamagePoint right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(DamagePoint left, DamagePoint right)
	{
		return !(left == right);
	}

	public static implicit operator Vector4(DamagePoint a)
	{
		return new Vector4(a.X, a.Y, a.Intensity, (float)a.Type);
	}

	public static implicit operator DamagePoint(Vector4 a)
	{
		return new DamagePoint(a.x, a.y, a.z, (DamageType)Mathf.FloorToInt(a.w));
	}
}
