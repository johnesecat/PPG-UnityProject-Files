using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Extensions
{
	public static class UnityNativeObjectSetFieldIndirectorExtensions
	{
		public static void SetLayer(this GameObject go, int layer)
		{
			go.layer = layer;
		}

		public static void SetActiveTracked(this GameObject go, bool isActive)
		{
			go.SetActive(isActive);
		}

		public static void SetPosition(this Transform t, Vector3 position)
		{
			t.position = position;
		}

		public static void SetLocalPosition(this Transform t, Vector3 localPosition)
		{
			t.localPosition = localPosition;
		}

		public static void SetRotation(this Transform t, Quaternion rotation)
		{
			t.rotation = rotation;
		}

		public static void SetLocalRotation(this Transform t, Quaternion localRotation)
		{
			t.localRotation = localRotation;
		}

		public static void SetLocalScale(this Transform t, Vector3 localScale)
		{
			t.localScale = localScale;
		}

		public static void SetParentTracked(this Transform t, Transform parent)
		{
			t.SetParentTracked(parent, worldPositionStays: true);
		}

		public static void SetParentTracked(this Transform t, Transform parent, bool worldPositionStays)
		{
			t.SetParent(parent, worldPositionStays);
		}

		public static void RotateAroundTracked(this Transform t, Vector3 point, Vector3 axis, float angle)
		{
			t.RotateAround(point, axis, angle);
		}

		public static void RotateTracked(this Transform t, Vector3 axis, float angle, Space relativeTo)
		{
			t.Rotate(axis, angle, relativeTo);
		}

		public static void IgnoreCollision(this Collider collider1, Collider collider2, bool ignore)
		{
			Physics.IgnoreCollision(collider1, collider2, ignore);
		}
	}
}
