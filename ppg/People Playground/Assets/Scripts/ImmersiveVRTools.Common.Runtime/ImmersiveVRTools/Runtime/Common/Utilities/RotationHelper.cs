using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public static class RotationHelper
	{
		public static Quaternion GetQuaternionRotationChildRelativeParentApplicable(Quaternion targetRotation, Quaternion parentRotation, Quaternion childRotation)
		{
			Quaternion rotation = Quaternion.Inverse(parentRotation) * childRotation;
			return targetRotation * Quaternion.Inverse(rotation);
		}

		public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
		{
			return Quaternion.Euler(angles) * (point - pivot) + pivot;
		}

		public static PositionRotationPair RotateAround(PositionRotationPair original, Vector3 center, Vector3 axis, float angle)
		{
			Quaternion quaternion = Quaternion.AngleAxis(angle, axis);
			Vector3 vector = original.Position - center;
			vector = quaternion * vector;
			Vector3 position = center + vector;
			Quaternion rotation = original.Rotation;
			Quaternion rotation2 = original.Rotation * Quaternion.Inverse(rotation) * quaternion * rotation;
			return new PositionRotationPair(position, rotation2);
		}
	}
}
