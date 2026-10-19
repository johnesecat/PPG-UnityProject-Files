using UnityEngine;

namespace Linefy
{
	public static class RectTransformExtensions
	{
		private static Vector3[] fourWorldPoint = new Vector3[4];

		public static Matrix4x4 GetCenteredWorldMatrix(this RectTransform rt)
		{
			rt.GetWorldCorners(fourWorldPoint);
			Vector4 column = Vector3.LerpUnclamped(fourWorldPoint[0], fourWorldPoint[2], 0.5f);
			column.w = 1f;
			return new Matrix4x4(fourWorldPoint[3] - fourWorldPoint[0], fourWorldPoint[1] - fourWorldPoint[0], rt.transform.forward, column);
		}

		public static Matrix4x4 GetWorldMatrix(this RectTransform rt)
		{
			rt.GetWorldCorners(fourWorldPoint);
			Vector4 column = fourWorldPoint[0];
			column.w = 1f;
			return new Matrix4x4(fourWorldPoint[3] - fourWorldPoint[0], fourWorldPoint[1] - fourWorldPoint[0], rt.transform.forward, column);
		}
	}
}
