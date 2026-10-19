using UnityEngine;

namespace Linefy.Internal
{
	public struct Triangle2D
	{
		public Vector2 a;

		public Vector2 b;

		public Vector2 c;

		private Vector2 v0;

		private Vector2 v1;

		private float dot00;

		private float dot01;

		private float dot11;

		private float invDenom;

		public Triangle2D(Vector2 _a, Vector2 _b, Vector2 _c)
		{
			a = _a;
			b = _b;
			c = _c;
			v0 = c - a;
			v1 = b - a;
			dot00 = Vector2.Dot(v0, v0);
			dot01 = Vector2.Dot(v0, v1);
			dot11 = Vector2.Dot(v1, v1);
			invDenom = 1f / (dot00 * dot11 - dot01 * dot01);
		}

		public bool PointTest(Vector2 p, ref Vector3 bary)
		{
			Vector2 rhs = p - a;
			float num = Vector2.Dot(v0, rhs);
			float num2 = Vector2.Dot(v1, rhs);
			bary.z = (dot11 * num - dot01 * num2) * invDenom;
			bary.y = (dot00 * num2 - dot01 * num) * invDenom;
			bary.x = 1f - (bary.z + bary.y);
			if (bary.z >= 0f && bary.y >= 0f)
			{
				return bary.z + bary.y < 1f;
			}
			return false;
		}

		public bool PointTest(Vector2 p)
		{
			Vector2 rhs = p - a;
			float num = Vector2.Dot(v0, rhs);
			float num2 = Vector2.Dot(v1, rhs);
			Vector3 vector = default(Vector3);
			vector.z = (dot11 * num - dot01 * num2) * invDenom;
			vector.y = (dot00 * num2 - dot01 * num) * invDenom;
			vector.x = 1f - (vector.z + vector.y);
			if (vector.z >= 0f && vector.y >= 0f)
			{
				return vector.z + vector.y < 1f;
			}
			return false;
		}

		public static bool PointTest(Vector2 pa, Vector2 pb, Vector2 pc, Vector2 pp, ref Vector3 _bary)
		{
			if (pa.x < pp.x && pb.x < pp.x && pc.x < pp.x)
			{
				return false;
			}
			if (pa.x > pp.x && pb.x > pp.x && pc.x > pp.x)
			{
				return false;
			}
			if (pa.y > 0f && pb.y > 0f && pc.y > 0f)
			{
				return false;
			}
			if (pa.y < pp.y && pb.y < pp.y && pc.y < pp.y)
			{
				return false;
			}
			if ((pc.x - pa.x) * (pb.y - pa.y) - (pc.y - pa.y) * (pb.x - pa.x) < 0f)
			{
				return false;
			}
			Vector2 vector = pc - pa;
			Vector2 vector2 = pb - pa;
			Vector2 rhs = pp - pa;
			float num = Vector2.Dot(vector, vector);
			float num2 = Vector2.Dot(vector, vector2);
			float num3 = Vector2.Dot(vector2, vector2);
			float num4 = 1f / (num * num3 - num2 * num2);
			float num5 = Vector2.Dot(vector, rhs);
			float num6 = Vector2.Dot(vector2, rhs);
			_bary.z = (num3 * num5 - num2 * num6) * num4;
			_bary.y = (num * num6 - num2 * num5) * num4;
			_bary.x = 1f - (_bary.z + _bary.y);
			if (_bary.z >= 0f && _bary.y >= 0f)
			{
				return _bary.z + _bary.y < 1f;
			}
			return false;
		}

		public static bool PointTestDoublesided(Vector2 pa, Vector2 pb, Vector2 pc, Vector2 pp, ref Vector3 _bary)
		{
			if (pa.x < pp.x && pb.x < pp.x && pc.x < pp.x)
			{
				return false;
			}
			if (pa.x > pp.x && pb.x > pp.x && pc.x > pp.x)
			{
				return false;
			}
			if (pa.y < pp.y && pb.y < pp.y && pc.y < pp.y)
			{
				return false;
			}
			Vector2 vector = pc - pa;
			Vector2 vector2 = pb - pa;
			Vector2 rhs = pp - pa;
			float num = Vector2.Dot(vector, vector);
			float num2 = Vector2.Dot(vector, vector2);
			float num3 = Vector2.Dot(vector2, vector2);
			float num4 = 1f / (num * num3 - num2 * num2);
			float num5 = Vector2.Dot(vector, rhs);
			float num6 = Vector2.Dot(vector2, rhs);
			_bary.z = (num3 * num5 - num2 * num6) * num4;
			_bary.y = (num * num6 - num2 * num5) * num4;
			_bary.x = 1f - (_bary.z + _bary.y);
			if (_bary.z >= 0f && _bary.y >= 0f)
			{
				return _bary.z + _bary.y < 1f;
			}
			return false;
		}

		public static bool PointTestDoublesided(Vector2 pa, Vector2 pb, Vector2 pc, Vector2 pp)
		{
			if (pa.x < pp.x && pb.x < pp.x && pc.x < pp.x)
			{
				return false;
			}
			if (pa.x > pp.x && pb.x > pp.x && pc.x > pp.x)
			{
				return false;
			}
			if (pa.y < pp.y && pb.y < pp.y && pc.y < pp.y)
			{
				return false;
			}
			Vector2 vector = pc - pa;
			Vector2 vector2 = pb - pa;
			Vector2 rhs = pp - pa;
			float num = Vector2.Dot(vector, vector);
			float num2 = Vector2.Dot(vector, vector2);
			float num3 = Vector2.Dot(vector2, vector2);
			float num4 = 1f / (num * num3 - num2 * num2);
			float num5 = Vector2.Dot(vector, rhs);
			float num6 = Vector2.Dot(vector2, rhs);
			float num7 = (num3 * num5 - num2 * num6) * num4;
			float num8 = (num * num6 - num2 * num5) * num4;
			if (num7 >= 0f && num8 >= 0f)
			{
				return num7 + num8 < 1f;
			}
			return false;
		}

		public static Vector3 InscribedCircle(Vector2 a, Vector2 b, Vector2 c)
		{
			Vector2 redDir = Vector2.LerpUnclamped((c - a).normalized, (b - a).normalized, 0.5f);
			Vector2 greenDir = Vector2.LerpUnclamped((c - b).normalized, (a - b).normalized, 0.5f);
			Vector2 intersection = default(Vector2);
			if (Edge2D.LineLineItersection(a, redDir, b, greenDir, ref intersection))
			{
				float lv = 0f;
				float distance = Edge2D.GetDistance(a, b, intersection, ref lv);
				return new Vector3(intersection.x, intersection.y, distance);
			}
			return new Vector3(a.x, a.y, 0f);
		}

		public static bool IsClockwise(Vector2 pa, Vector2 pb, Vector2 pc)
		{
			return (pc.x - pa.x) * (pb.y - pa.y) - (pc.y - pa.y) * (pb.x - pa.x) >= 0f;
		}

		public static int Clockwise(Vector2 pa, Vector2 pb, Vector2 pc)
		{
			float num = (pc.x - pa.x) * (pb.y - pa.y) - (pc.y - pa.y) * (pb.x - pa.x);
			if (MathUtility.ApproximatelyZero(num))
			{
				return 0;
			}
			if (num < 0f)
			{
				return -1;
			}
			return 1;
		}
	}
}
