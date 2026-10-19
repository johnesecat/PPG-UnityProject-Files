using UnityEngine;

public class VolumeTests
{
	public static bool IsPointInCylinder(Vector3 cylinderStart, Vector3 CylinderEnd, float radius, Vector3 testPoint)
	{
		float num = Vector3.Distance(cylinderStart, CylinderEnd);
		return GetDistanceSquaredFromCylinderAxis(cylinderStart, CylinderEnd, num * num, radius, testPoint) > 0f;
	}

	private static float GetDistanceSquaredFromCylinderAxis(Vector3 cylinderStart, Vector3 CylinderEnd, float LengthSquared, float RadiusSquared, Vector3 testPoint)
	{
		float num = CylinderEnd.x - cylinderStart.x;
		float num2 = CylinderEnd.y - cylinderStart.y;
		float num3 = CylinderEnd.z - cylinderStart.z;
		float num4 = testPoint.x - cylinderStart.x;
		float num5 = testPoint.y - cylinderStart.y;
		float num6 = testPoint.z - cylinderStart.z;
		float num7 = num4 * num + num5 * num2 + num6 * num3;
		if (num7 < 0f || num7 > LengthSquared)
		{
			return -1f;
		}
		float num8 = num4 * num4 + num5 * num5 + num6 * num6 - num7 * num7 / LengthSquared;
		if (num8 > RadiusSquared)
		{
			return -1f;
		}
		return num8;
	}
}
