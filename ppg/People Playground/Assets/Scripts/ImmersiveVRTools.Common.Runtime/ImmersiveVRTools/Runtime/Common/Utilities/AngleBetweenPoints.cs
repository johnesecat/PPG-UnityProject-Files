using System;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public class AngleBetweenPoints
	{
		public static bool GlobalDebug = false;

		public static float GlobalScale = 1f;

		[Obsolete("Use GetThreePointAngleSigned180 instead")]
		public static float GetThreePointAngle(Vector3 first, Vector3 second, Vector3 pivot, Vector3 axis, bool debug = false)
		{
			Vector3 lhs = pivot - first;
			Vector3 rhs = pivot - second;
			float result = Mathf.Atan2(Vector3.Dot(axis, Vector3.Cross(lhs, rhs)), Vector3.Dot(lhs, rhs)) * 57.29578f;
			if (!debug)
			{
				_ = GlobalDebug;
			}
			return result;
		}

		public static float GetThreePointAngleSigned180(Vector3 first, Vector3 second, Vector3 pivot, Vector3 axis, bool debug = false)
		{
			return GetThreePointAngle(first, second, pivot, axis, debug);
		}

		public static float GetThreePointAngleUnsigned360(Vector3 first, Vector3 second, Vector3 pivot, Vector3 axis, bool debug = false)
		{
			return AdjustSigned180AngleToUnsigned360(GetThreePointAngle(first, second, pivot, axis, debug));
		}

		public static float AdjustSigned180AngleToUnsigned360(float angle180)
		{
			if (!(angle180 < 0f))
			{
				if (!(angle180 <= 180f))
				{
					return -1f;
				}
				return 360f - angle180;
			}
			return 0f - angle180;
		}
	}
}
