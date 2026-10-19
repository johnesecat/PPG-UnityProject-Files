using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct IgnoreCollisionStackController
{
	private struct ColliderPair
	{
		public Collider2D A;

		public Collider2D B;

		public override bool Equals(object obj)
		{
			if (obj is ColliderPair)
			{
				ColliderPair colliderPair = (ColliderPair)obj;
				if (!EqualityComparer<Collider2D>.Default.Equals(A, colliderPair.A) || !EqualityComparer<Collider2D>.Default.Equals(B, colliderPair.B))
				{
					if (EqualityComparer<Collider2D>.Default.Equals(A, colliderPair.B))
					{
						return EqualityComparer<Collider2D>.Default.Equals(B, colliderPair.A);
					}
					return false;
				}
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = EqualityComparer<Collider2D>.Default.GetHashCode(A);
			int hashCode2 = EqualityComparer<Collider2D>.Default.GetHashCode(B);
			return (-1817952719 * -1521134295 + Mathf.Max(hashCode, hashCode2)) * -1521134295 + Mathf.Min(hashCode, hashCode2);
		}

		public static bool operator ==(ColliderPair left, ColliderPair right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ColliderPair left, ColliderPair right)
		{
			return !(left == right);
		}

		public static implicit operator ColliderPair((Collider2D, Collider2D) a)
		{
			ColliderPair result = default(ColliderPair);
			(result.A, result.B) = a;
			return result;
		}
	}

	private static readonly Dictionary<ColliderPair, RefInt> stack = new Dictionary<ColliderPair, RefInt>();

	public static void Clear()
	{
		stack.Clear();
	}

	public static void RequestIgnoreCollision(Collider2D a, Collider2D b)
	{
		if ((bool)a && (bool)b)
		{
			if (stack.TryGetValue((a, b), out var value))
			{
				value.Increment();
				return;
			}
			value = new RefInt(1);
			stack.Add((a, b), value);
			Physics2D.IgnoreCollision(a, b, ignore: true);
		}
	}

	public static void RequestDontIgnoreCollision(Collider2D a, Collider2D b)
	{
		if (!a || !b || !stack.TryGetValue((a, b), out var value))
		{
			return;
		}
		value.Decrement();
		if (value.Value <= 0)
		{
			stack.Remove((a, b));
			if ((bool)a && (bool)b)
			{
				Physics2D.IgnoreCollision(a, b, ignore: false);
			}
		}
	}

	public static void IgnoreCollisionSubstituteMethod(Collider2D a, Collider2D b, bool ignore = true)
	{
		if ((bool)a && (bool)b)
		{
			if (ignore)
			{
				RequestIgnoreCollision(a, b);
			}
			else
			{
				RequestDontIgnoreCollision(a, b);
			}
		}
	}
}
