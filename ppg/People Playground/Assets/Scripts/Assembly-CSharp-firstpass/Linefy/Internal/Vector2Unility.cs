using UnityEngine;

namespace Linefy.Internal
{
	public static class Vector2Unility
	{
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

		public static Vector2 HermitePoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
		{
			return new Vector3(MathUtility.HermiteValue(p0.x, p1.x, p2.x, p3.x, t), MathUtility.HermiteValue(p0.y, p1.y, p2.y, p3.y, t));
		}
	}
}
