using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Linefy.Internal
{
	public static class Utility
	{
		public static float TicksPerMilliseconds => 10000f;

		public static bool RayCircleInterction(Ray2D ray, Vector2 circleCenter, float circleRadius, ref float i0, ref float i1)
		{
			Vector2 nearestPoint = default(Vector2);
			float num = ray.DistanceToPoint(circleCenter, ref nearestPoint);
			if (num < circleRadius)
			{
				Vector2 lhs = nearestPoint - ray.origin;
				float num2 = Mathf.Sign(Vector2.Dot(lhs, ray.direction));
				float magnitude = lhs.magnitude;
				float num3 = Mathf.Sqrt(circleRadius * circleRadius - num * num);
				i0 = (magnitude - num3) * num2;
				i1 = (magnitude + num3) * num2;
				return true;
			}
			return false;
		}

		public static float DistanceToPoint(this Ray2D r, Vector2 point, ref Vector2 nearestPoint)
		{
			Vector2 direction = r.direction;
			Edge2D.LineLineItersection(greenDir: new Vector2(0f - direction.y, direction.x), redOrigin: r.origin, redDir: r.direction, greenOrigin: point, intersection: ref nearestPoint);
			return Vector2.Distance(point, nearestPoint);
		}

		public static Ray Multiply(this Ray r, Matrix4x4 m)
		{
			Vector3 direction = m.MultiplyVector(r.direction);
			return new Ray(m.MultiplyPoint3x4(r.origin), direction);
		}

		public static float Milliseconds(this Stopwatch sw)
		{
			return (float)sw.ElapsedTicks / TicksPerMilliseconds;
		}

		public static bool IsNotValid(this Matrix4x4 tm)
		{
			for (int i = 0; i < 4; i++)
			{
				if (tm.GetColumn(i).magnitude.EqualsApproximately(0f))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsNotValid(this Quaternion q)
		{
			return (q.x + q.y + q.z + q.w).EqualsApproximately(0f);
		}

		public static Vector3 HermitePoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float s)
		{
			return new Vector3(HermiteValue(p0.x, p1.x, p2.x, p3.x, s), HermiteValue(p0.y, p1.y, p2.y, p3.y, s), HermiteValue(p0.z, p1.z, p2.z, p3.z, s));
		}

		public static Vector2 HermitePoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float s)
		{
			return new Vector3(HermiteValue(p0.x, p1.x, p2.x, p3.x, s), HermiteValue(p0.y, p1.y, p2.y, p3.y, s));
		}

		public static float HermiteValue(float y0, float y1, float y2, float y3, float s)
		{
			float num = s * s;
			float num2 = num * s;
			float num3 = (y1 - y0) / 2f;
			num3 += (y2 - y1) / 2f;
			float num4 = (y2 - y1) / 2f;
			num4 += (y3 - y2) / 2f;
			float num5 = 2f * num2 - 3f * num + 1f;
			float num6 = num2 - 2f * num + s;
			float num7 = num2 - num;
			float num8 = -2f * num2 + 3f * num;
			return num5 * y1 + num6 * num3 + num7 * num4 + num8 * y2;
		}

		public static bool EqualsApproximately(this Vector2 a, Vector2 b)
		{
			if (MathUtility.ApproximatelyZero(a.x - b.x))
			{
				return MathUtility.ApproximatelyZero(a.y - b.y);
			}
			return false;
		}

		public static bool EqualsApproximately(this Vector3 a, Vector3 b)
		{
			if (MathUtility.ApproximatelyZero(a.x - b.x) && MathUtility.ApproximatelyZero(a.y - b.y))
			{
				return MathUtility.ApproximatelyZero(a.z - b.z);
			}
			return false;
		}

		public static Vector3 XYtoXyZ(this Vector2 v)
		{
			return new Vector3(v.x, 0f, v.y);
		}

		public static Vector3 XYtoXyZ(this Vector2 v, float y)
		{
			return new Vector3(v.x, y, v.y);
		}

		public static Vector2 XyZtoXY(this Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}

		public static void DebugDrawPoint(Vector3 point, float size, Color color)
		{
			float num = size * 0.5f;
			Vector3 end = new Vector3(point.x, point.y + num, point.z);
			Vector3 end2 = new Vector3(point.x, point.y - num, point.z);
			Vector3 vector = new Vector3(point.x - num, point.y, point.z);
			Vector3 vector2 = new Vector3(point.x, point.y, point.z + num);
			Vector3 vector3 = new Vector3(point.x + num, point.y, point.z);
			Vector3 vector4 = new Vector3(point.x, point.y, point.z - num);
			UnityEngine.Debug.DrawLine(vector, end, color);
			UnityEngine.Debug.DrawLine(vector2, end, color);
			UnityEngine.Debug.DrawLine(vector3, end, color);
			UnityEngine.Debug.DrawLine(vector4, end, color);
			UnityEngine.Debug.DrawLine(vector, end2, color);
			UnityEngine.Debug.DrawLine(vector2, end2, color);
			UnityEngine.Debug.DrawLine(vector3, end2, color);
			UnityEngine.Debug.DrawLine(vector4, end2, color);
			UnityEngine.Debug.DrawLine(vector, vector2, color);
			UnityEngine.Debug.DrawLine(vector2, vector3, color);
			UnityEngine.Debug.DrawLine(vector3, vector4, color);
			UnityEngine.Debug.DrawLine(vector4, vector, color);
		}

		public static void DrawCircleXY(Vector2 center, float radius, Color color, int segments)
		{
			float num = 1f / (float)segments;
			for (int i = 0; i < segments; i++)
			{
				float f = num * (float)i * (float)Math.PI * 2f;
				float f2 = num * (float)(i + 1) * (float)Math.PI * 2f;
				Vector2 vector = new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * radius + center;
				UnityEngine.Debug.DrawLine(end: new Vector2(Mathf.Cos(f2), Mathf.Sin(f2)) * radius + center, start: vector, color: color);
			}
		}

		public static void Resize<T>(this List<T> list, int size, T element = default(T))
		{
			int count = list.Count;
			if (size < count)
			{
				list.RemoveRange(size, count - size);
			}
			else if (size > count)
			{
				if (size > list.Capacity)
				{
					list.Capacity = size;
				}
				list.AddRange(Enumerable.Repeat(element, size - count));
			}
		}
	}
}
