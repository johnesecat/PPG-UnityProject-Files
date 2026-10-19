using Linefy.Internal;
using UnityEngine;

namespace Linefy
{
	public static class Vector3Utility
	{
		public static Vector3 HermitePoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			return new Vector3(MathUtility.HermiteValue(p0.x, p1.x, p2.x, p3.x, t), MathUtility.HermiteValue(p0.y, p1.y, p2.y, p3.y, t), MathUtility.HermiteValue(p0.z, p1.z, p2.z, p3.z, t));
		}

		public static Vector3 HermiteInterpolate(Vector3 y0, Vector3 y1, Vector3 y2, Vector3 y3, float mu, float tension)
		{
			float num = mu * mu;
			float num2 = num * mu;
			Vector3 vector = (y1 - y0) * (1f - tension) / 2f;
			vector += (y2 - y1) * (1f - tension) / 2f;
			Vector3 vector2 = (y2 - y1) * (1f - tension) / 2f;
			vector2 += (y3 - y2) * (1f - tension) / 2f;
			float num3 = 2f * num2 - 3f * num + 1f;
			float num4 = num2 - 2f * num + mu;
			float num5 = num2 - num;
			float num6 = -2f * num2 + 3f * num;
			return num3 * y1 + num4 * vector + num5 * vector2 + num6 * y2;
		}
	}
}
