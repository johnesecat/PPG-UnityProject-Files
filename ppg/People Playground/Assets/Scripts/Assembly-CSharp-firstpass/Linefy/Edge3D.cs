using UnityEngine;

namespace Linefy
{
	public class Edge3D
	{
		public Vector3 A;

		public Vector3 B;

		private Vector3 ab;

		private float length;

		private float length2;

		public Edge3D(Vector2 a, Vector2 b)
		{
			A = a;
			B = b;
			ab = B - A;
			length = ab.magnitude;
			length2 = length * length;
		}

		public float GetDistance(Vector3 point)
		{
			float num = Vector3.Dot(point - A, ab) / length2;
			Vector3 zero = Vector3.zero;
			zero = ((num < 0f) ? A : ((!(num > 1f)) ? (A + ab * num) : B));
			return Vector3.Distance(zero, point);
		}

		public static float LineLineDistance(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2, ref Vector3 pa, ref Vector3 pb)
		{
			Vector3 vector = a1 - b1;
			Vector3 vector2 = b2 - b1;
			Vector3 vector3 = a2 - a1;
			float num = vector.x * vector2.x + vector.y * vector2.y + vector.z * vector2.z;
			float num2 = vector2.x * vector3.x + vector2.y * vector3.y + vector2.z * vector3.z;
			float num3 = vector.x * vector3.x + vector.y * vector3.y + vector.z * vector3.z;
			float num4 = vector2.x * vector2.x + vector2.y * vector2.y + vector2.z * vector2.z;
			float num5 = (vector3.x * vector3.x + vector3.y * vector3.y + vector3.z * vector3.z) * num4 - num2 * num2;
			float num6 = (num * num2 - num3 * num4) / num5;
			float num7 = (num + num2 * num6) / num4;
			num6 = Mathf.Clamp01(num6);
			pa = a1 + vector3 * num6;
			pb = b1 + vector2 * num7;
			return Vector3.Distance(pa, pb);
		}
	}
}
