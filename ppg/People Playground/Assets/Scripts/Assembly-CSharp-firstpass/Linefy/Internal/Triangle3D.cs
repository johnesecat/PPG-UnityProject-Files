using UnityEngine;

namespace Linefy.Internal
{
	public struct Triangle3D
	{
		private Vector3 a;

		private Vector3 b;

		private Vector3 c;

		private Vector3 e1;

		private Vector3 e2;

		public Triangle3D(Vector3 a, Vector3 b, Vector3 c)
		{
			this.a = a;
			this.b = b;
			this.c = c;
			e1 = b - a;
			e2 = c - a;
		}

		public bool RaycastDoublesided(Ray r, ref Vector3 bary, ref Vector3 hit)
		{
			Vector3 rhs = Vector3.Cross(r.direction, e2);
			float num = Vector3.Dot(e1, rhs);
			if (num.EqualsApproximately(0f))
			{
				return false;
			}
			float num2 = 1f / num;
			Vector3 lhs = r.origin - a;
			float num3 = Vector3.Dot(lhs, rhs) * num2;
			if (num3 < 0f || num3 > 1f)
			{
				return false;
			}
			Vector3 rhs2 = Vector3.Cross(lhs, e1);
			float num4 = Vector3.Dot(r.direction, rhs2) * num2;
			float num5 = num3 + num4;
			if (num4 < 0f || num5 > 1f)
			{
				return false;
			}
			float distance = Vector3.Dot(e2, rhs2) * num2;
			bary.x = 1f - num5;
			bary.y = num3;
			bary.z = num4;
			hit = r.GetPoint(distance);
			return true;
		}

		public bool Raycast(Ray r, ref Vector3 bary, ref Vector3 hit)
		{
			Vector3 rhs = Vector3.Cross(r.direction, e2);
			float num = Vector3.Dot(e1, rhs);
			if (num.LessOrEqualsThan(0f))
			{
				return false;
			}
			float num2 = 1f / num;
			Vector3 lhs = r.origin - a;
			float num3 = Vector3.Dot(lhs, rhs) * num2;
			if (num3 < 0f || num3 > 1f)
			{
				return false;
			}
			Vector3 rhs2 = Vector3.Cross(lhs, e1);
			float num4 = Vector3.Dot(r.direction, rhs2) * num2;
			float num5 = num3 + num4;
			if (num4 < 0f || num5 > 1f)
			{
				return false;
			}
			float distance = Vector3.Dot(e2, rhs2) * num2;
			bary.x = 1f - num5;
			bary.y = num3;
			bary.z = num4;
			hit = r.GetPoint(distance);
			return true;
		}

		public Vector3 GetPoint(Vector3 bary)
		{
			return a * bary.x + b * bary.y + bary.z * c;
		}

		public float Area()
		{
			return Vector3.Cross(a - b, b - c).magnitude / 2f;
		}

		public float Area2()
		{
			float num = Vector3.Distance(a, b);
			float num2 = Vector3.Distance(b, c);
			float num3 = Vector3.Distance(c, a);
			float num4 = (num + num2 + num3) / 2f;
			return Mathf.Sqrt(num4 * (num4 - num) * (num4 - num2) * (num4 - num3));
		}

		public void DrawDebug(Color color)
		{
			Debug.DrawLine(a, b, color);
			Debug.DrawLine(b, c, color);
			Debug.DrawLine(c, a, color);
		}

		public float DistanceToPoint(Vector3 point)
		{
			return new Plane(a, b, c).GetDistanceToPoint(point);
		}
	}
}
