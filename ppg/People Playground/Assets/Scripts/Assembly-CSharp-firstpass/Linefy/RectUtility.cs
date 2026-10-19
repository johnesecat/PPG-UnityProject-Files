using Linefy.Internal;
using UnityEngine;

namespace Linefy
{
	public static class RectUtility
	{
		public static Rect Inflate(this Rect r, float inflateFactor)
		{
			r.xMin -= inflateFactor;
			r.xMax += inflateFactor;
			r.yMin -= inflateFactor;
			r.yMax += inflateFactor;
			return r;
		}

		public static Vector2 Point0(this Rect r)
		{
			return r.min;
		}

		public static Vector2 Point1(this Rect r)
		{
			return new Vector2(r.min.x, r.max.y);
		}

		public static Vector2 Point2(this Rect r)
		{
			return r.max;
		}

		public static Vector2 Point3(this Rect r)
		{
			return new Vector2(r.max.x, r.min.y);
		}

		public static Vector2 Clamp(this Rect r, Vector2 v)
		{
			v.x = Mathf.Clamp(v.x, r.xMin, r.xMax);
			v.y = Mathf.Clamp(v.y, r.yMin, r.yMax);
			return v;
		}

		public static Rect Multiply(this Rect a, Rect b)
		{
			Vector2 vector = Rect.NormalizedToPoint(a, b.position);
			Vector2 size = Rect.NormalizedToPoint(a, b.max) - vector;
			return new Rect(vector, size);
		}

		public static Rect Scale(this Rect a, float scaleFactor)
		{
			return new Rect(a.x * scaleFactor, a.y * scaleFactor, a.width * scaleFactor, a.height * scaleFactor);
		}

		public static Rect Offset(this Rect a, float offsetX, float offsetY)
		{
			Rect result = a;
			result.x += offsetX;
			result.y += offsetY;
			return result;
		}

		public static RectInt Scale(this RectInt a, float scaleFactor)
		{
			return new RectInt((int)((float)a.x * scaleFactor), (int)((float)a.y * scaleFactor), (int)((float)a.width * scaleFactor), (int)((float)a.height * scaleFactor));
		}

		public static Rect GetRect(this RectInt a)
		{
			return new Rect(a.position, a.size);
		}

		public static float Distance(this Rect r, Vector2 point)
		{
			if (r.Contains(point))
			{
				return 0f;
			}
			float b = float.MaxValue;
			b = Mathf.Min(Edge2D.GetDistance(r.Point0(), r.Point1(), point), b);
			b = Mathf.Min(Edge2D.GetDistance(r.Point1(), r.Point2(), point), b);
			b = Mathf.Min(Edge2D.GetDistance(r.Point2(), r.Point3(), point), b);
			return Mathf.Min(Edge2D.GetDistance(r.Point3(), r.Point0(), point), b);
		}
	}
}
