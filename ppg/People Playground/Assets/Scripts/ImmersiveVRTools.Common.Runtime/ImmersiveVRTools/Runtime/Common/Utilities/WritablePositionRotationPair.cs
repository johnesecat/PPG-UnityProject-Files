using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public struct WritablePositionRotationPair
	{
		public Vector3 Position;

		public Quaternion Rotation;

		public WritablePositionRotationPair(Vector3 position, Quaternion rotation)
		{
			Position = position;
			Rotation = rotation;
		}
	}
}
