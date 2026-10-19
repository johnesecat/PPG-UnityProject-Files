using UnityEngine;

namespace Linefy.Internal
{
	public struct Edge2D
	{
		public Vector2 a;

		public Vector2 b;

		public Vector2 ab;

		public float length;

		private float lengthSquare;

		public Edge2D(Vector2 a, Vector2 b)
		{
			this.a = a;
			this.b = b;
			ab = this.b - this.a;
			length = ab.magnitude;
			lengthSquare = length * length;
		}

		public float GetDistance(Vector2 point)
		{
			float num = Vector2.Dot(point - a, ab) / lengthSquare;
			Vector3 zero = Vector3.zero;
			zero = ((num < 0f) ? ((Vector3)a) : ((!(num > 1f)) ? ((Vector3)(a + ab * num)) : ((Vector3)b)));
			return Vector2.Distance(zero, point);
		}

		public float GetDistance(Vector2 point, ref float lv)
		{
			float num = Vector2.Dot(point - a, ab) / lengthSquare;
			Vector3 zero = Vector3.zero;
			if (num < 0f)
			{
				lv = 0f;
				zero = a;
			}
			else if (num > 1f)
			{
				lv = 1f;
				zero = b;
			}
			else
			{
				lv = num;
				zero = a + ab * num;
			}
			return Vector2.Distance(zero, point);
		}

		public float GetLV(Vector2 point)
		{
			return Vector2.Dot(point - a, ab) / lengthSquare;
		}

		public static float GetDistance(Vector2 a, Vector2 b, Vector2 point, ref float lv)
		{
			if (a.EqualsApproximately(b))
			{
				lv = 0f;
				return Vector2.Distance(a, point);
			}
			Vector2 vector = b - a;
			float magnitude = vector.magnitude;
			float num = magnitude * magnitude;
			float num2 = Vector2.Dot(point - a, vector) / num;
			Vector3 zero = Vector3.zero;
			if (num2 < 0f)
			{
				zero = a;
				lv = 0f;
			}
			else if (num2 > 1f)
			{
				zero = b;
				lv = 1f;
			}
			else
			{
				zero = a + vector * num2;
				lv = num2;
			}
			return Vector2.Distance(zero, point);
		}

		public static float GetDistance(Vector2 a, Vector2 b, Vector2 point)
		{
			Vector2 vector = b - a;
			float magnitude = vector.magnitude;
			float num = magnitude * magnitude;
			float num2 = Vector2.Dot(point - a, vector) / num;
			Vector3 zero = Vector3.zero;
			zero = ((num2 < 0f) ? ((Vector3)a) : ((!(num2 > 1f)) ? ((Vector3)(a + vector * num2)) : ((Vector3)b)));
			return Vector2.Distance(zero, point);
		}

		public static float GetDistance(Vector2 a, Vector2 b, Vector2 point, ref float lv, ref float slope)
		{
			Vector2 vector = b - a;
			float magnitude = vector.magnitude;
			float num = magnitude * magnitude;
			float num2 = Vector2.Dot(point - a, vector) / num;
			Vector2 zero = Vector2.zero;
			if (num2 < 0f)
			{
				zero = a;
				lv = 0f;
			}
			else if (num2 > 1f)
			{
				zero = b;
				lv = 1f;
			}
			else
			{
				zero = a + vector * num2;
				lv = num2;
			}
			slope = Mathf.Abs(Vector2.Dot((zero - point).normalized, vector / magnitude));
			return Vector2.Distance(zero, point);
		}

		public static float RotationAngle(Vector2 a, Vector2 b)
		{
			Vector2 vector = b - a;
			return Mathf.Atan2(vector.y, vector.x);
		}

		public static float RotationAngle(Vector2 dir)
		{
			return Mathf.Atan2(dir.y, dir.x);
		}

		public static Vector2 Rotate90(Vector2 vector)
		{
			return new Vector2(vector.y, 0f - vector.x);
		}

		public static bool LineLineItersection(Vector2 redOrigin, Vector2 redDir, Vector2 greenOrigin, Vector2 greenDir, ref Vector2 intersection)
		{
			bool flag = MathUtility.ApproximatelyEquals(redDir.x, 0f);
			bool flag2 = MathUtility.ApproximatelyEquals(redDir.y, 0f);
			bool flag3 = MathUtility.ApproximatelyEquals(greenDir.x, 0f);
			bool flag4 = MathUtility.ApproximatelyEquals(greenDir.y, 0f);
			if (flag2 && flag4)
			{
				return false;
			}
			if (flag && flag3)
			{
				return false;
			}
			if (flag2)
			{
				intersection.x = greenOrigin.x + (redOrigin.y - greenOrigin.y) * (greenDir.x / greenDir.y);
				intersection.y = redOrigin.y;
				return true;
			}
			if (flag)
			{
				intersection.x = redOrigin.x;
				intersection.y = greenOrigin.y + (redOrigin.x - greenOrigin.x) * (greenDir.y / greenDir.x);
				return true;
			}
			if (flag4)
			{
				intersection.x = redOrigin.x + (greenOrigin.y - redOrigin.y) * (redDir.x / redDir.y);
				intersection.y = greenOrigin.y;
				return true;
			}
			if (flag3)
			{
				intersection.x = greenOrigin.x;
				intersection.y = redOrigin.y + (greenOrigin.x - redOrigin.x) * (redDir.y / redDir.x);
				return true;
			}
			float num = redDir.y / redDir.x;
			float num2 = greenDir.y / greenDir.x;
			if (MathUtility.ApproximatelyEquals(num, num2))
			{
				return false;
			}
			float num3 = redOrigin.y - num * redOrigin.x;
			float num4 = greenOrigin.y - num2 * greenOrigin.x;
			intersection.x = (num4 - num3) / (num - num2);
			intersection.y = num * intersection.x + num3;
			return true;
		}

		public static bool LineLineItersection(Ray2D r0, Ray2D r1, ref Vector2 intersection)
		{
			return LineLineItersection(r0.origin, r0.direction, r1.origin, r1.direction, ref intersection);
		}

		public static float SignedAngle(Vector2 dirA, Vector2 dirB)
		{
			dirA.Normalize();
			dirB.Normalize();
			float num = Vector2.Dot(new Vector2(dirA.y, 0f - dirA.x), dirB);
			float num2 = Mathf.Acos(Vector2.Dot(dirA, dirB)) * 57.29578f;
			if (!(num < 0f))
			{
				return num2;
			}
			return 360f - num2;
		}
	}
}
