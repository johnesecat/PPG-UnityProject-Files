using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public struct LineSegment : IEquatable<LineSegment>
{
	public Vector2 A;

	public Vector2 B;

	public LineSegment(Vector2 a, Vector2 b)
	{
		A = a;
		B = b;
	}

	[IsReadOnly]
	public Vector2 GetRandomPoint()
	{
		return Vector2.Lerp(A, B, UnityEngine.Random.value);
	}

	[IsReadOnly]
	public Vector2 GetPointAlong(float progress)
	{
		return Vector2.Lerp(A, B, progress);
	}

	[IsReadOnly]
	public Vector2 GetMidpoint()
	{
		return (A + B) * 0.5f;
	}

	[IsReadOnly]
	public float GetDistance(Vector2 point)
	{
		return Utils.DistanceFromPointToLineSegment(point, A, B);
	}

	public override bool Equals(object obj)
	{
		if (obj is LineSegment)
		{
			LineSegment other = (LineSegment)obj;
			return Equals(other);
		}
		return false;
	}

	public bool Equals(LineSegment other)
	{
		if (A.Equals(other.A))
		{
			return B.Equals(other.B);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (-1817952719 * -1521134295 + A.GetHashCode()) * -1521134295 + B.GetHashCode();
	}

	public static bool operator ==(LineSegment left, LineSegment right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(LineSegment left, LineSegment right)
	{
		return !(left == right);
	}
}
