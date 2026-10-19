using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Extensions
{
	public static class Vector3Extensions
	{
		public static Vector3 AddAdjustmentOnSingleAxis(this Vector3 vector, Vector3Axis axis, float adjustment)
		{
			return new Vector3((axis == Vector3Axis.X) ? (vector.x + adjustment) : vector.x, (axis == Vector3Axis.Y) ? (vector.y + adjustment) : vector.y, (axis == Vector3Axis.Z) ? (vector.z + adjustment) : vector.z);
		}

		public static Vector3 Random(Vector3 min, Vector3 max)
		{
			return new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
		}
	}
}
