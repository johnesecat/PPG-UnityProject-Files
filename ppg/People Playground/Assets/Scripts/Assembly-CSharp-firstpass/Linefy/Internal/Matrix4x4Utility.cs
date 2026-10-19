using UnityEngine;

namespace Linefy.Internal
{
	public static class Matrix4x4Utility
	{
		public static Matrix4x4 Interpolate(Matrix4x4 a, Matrix4x4 b, float t)
		{
			Vector3 upwards = Vector3.LerpUnclamped(a.GetColumn(1), b.GetColumn(1), t);
			Vector3 forward = Vector3.LerpUnclamped(a.GetColumn(2), b.GetColumn(2), t);
			return Matrix4x4.TRS(Vector3.LerpUnclamped(a.GetColumn(3), b.GetColumn(3), t), Quaternion.LookRotation(forward, upwards), Vector3.one);
		}

		public static Matrix4x4 OrthonormalUnscaledInverse(this Matrix4x4 tm)
		{
			Matrix4x4 result = tm;
			result.m01 = tm.m10;
			result.m02 = tm.m20;
			result.m12 = tm.m21;
			result.m10 = tm.m01;
			result.m20 = tm.m02;
			result.m21 = tm.m12;
			float num = 0f - tm.m03;
			float num2 = 0f - tm.m13;
			float num3 = 0f - tm.m23;
			float m = result.m00 * num + result.m01 * num2 + result.m02 * num3;
			float m2 = result.m10 * num + result.m11 * num2 + result.m12 * num3;
			float m3 = result.m20 * num + result.m21 * num2 + result.m22 * num3;
			result.m03 = m;
			result.m13 = m2;
			result.m23 = m3;
			return result;
		}

		public static Matrix4x4 UnscaledTRSInverse(Vector3 position, Vector3 forward, Vector3 upward)
		{
			Vector3 binormal = Vector3.Cross(upward, forward);
			Vector3.OrthoNormalize(ref forward, ref upward, ref binormal);
			Matrix4x4 result = default(Matrix4x4);
			result.m00 = binormal.x;
			result.m10 = upward.x;
			result.m20 = forward.x;
			result.m01 = binormal.y;
			result.m11 = upward.y;
			result.m21 = forward.y;
			result.m02 = binormal.z;
			result.m12 = upward.z;
			result.m22 = forward.z;
			float num = 0f - position.x;
			float num2 = 0f - position.y;
			float num3 = 0f - position.z;
			float m = result.m00 * num + result.m01 * num2 + result.m02 * num3;
			float m2 = result.m10 * num + result.m11 * num2 + result.m12 * num3;
			float m3 = result.m20 * num + result.m21 * num2 + result.m22 * num3;
			result.m03 = m;
			result.m13 = m2;
			result.m23 = m3;
			result.m33 = 1f;
			return result;
		}

		public static Matrix4x4 UnscaledTRS(Vector3 position, Vector3 forward, Vector3 upward)
		{
			Vector3 binormal = Vector3.Cross(upward, forward);
			Vector3.OrthoNormalize(ref forward, ref upward, ref binormal);
			Matrix4x4 result = default(Matrix4x4);
			result.m00 = binormal.x;
			result.m10 = binormal.y;
			result.m20 = binormal.z;
			result.m01 = upward.x;
			result.m11 = upward.y;
			result.m21 = upward.z;
			result.m02 = forward.x;
			result.m12 = forward.y;
			result.m22 = forward.z;
			result.m03 = position.x;
			result.m13 = position.y;
			result.m23 = position.z;
			result.m33 = 1f;
			return result;
		}

		public static void Normalize(ref Matrix4x4 tm)
		{
			tm.SetColumn(0, ((Vector3)tm.GetColumn(0)).normalized);
			tm.SetColumn(1, ((Vector3)tm.GetColumn(1)).normalized);
			tm.SetColumn(2, ((Vector3)tm.GetColumn(2)).normalized);
		}

		public static Matrix4x4 ToUnscaled(this Matrix4x4 tm)
		{
			return UnscaledTRS(tm.GetPosition(), tm.GetColumn(2), tm.GetColumn(1));
		}

		public static Matrix4x4 SetRotation(this Matrix4x4 tm, Quaternion rot)
		{
			return Matrix4x4.TRS(tm.GetColumn(3), rot, Vector3.one);
		}

		public static Matrix4x4 SetPosition(this Matrix4x4 tm, Vector3 position)
		{
			return Matrix4x4.TRS(position, tm.GetRotation(), Vector3.one);
		}

		public static Quaternion GetRotation(this Matrix4x4 tm)
		{
			Vector3 forward = tm.GetColumn(2);
			Vector3 upwards = tm.GetColumn(1);
			return Quaternion.LookRotation(forward, upwards);
		}

		public static Vector3 GetPosition(this Matrix4x4 tm)
		{
			return tm.GetColumn(3);
		}

		public static Matrix4x4 NearClipPlaneGUISpaceMatrix(Camera cam, float offset)
		{
			Ray ray = cam.ViewportPointToRay(new Vector3(0f, 1f, 0f));
			Ray ray2 = cam.ViewportPointToRay(new Vector3(1f, 1f, 0f));
			Ray ray3 = cam.ViewportPointToRay(new Vector3(0f, 0f, 0f));
			Vector3 point = ray.GetPoint(offset);
			Vector3 point2 = ray2.GetPoint(offset);
			Vector3 point3 = ray3.GetPoint(offset);
			Rect pixelRect = cam.pixelRect;
			float num = (float)Screen.width / pixelRect.width;
			float num2 = (float)Screen.height / pixelRect.height;
			Vector3 vector = (point2 - point) / Screen.width * num;
			Vector3 vector2 = (point3 - point) / Screen.height * num2;
			Vector4 column = point - vector * pixelRect.x - vector2 * pixelRect.y;
			column.w = 1f;
			return new Matrix4x4(vector, vector2, ray.direction, column);
		}

		public static Matrix4x4 NearClipPlaneScreenSpaceMatrix(Camera camera, float offset)
		{
			Ray ray = camera.ViewportPointToRay(new Vector3(0f, 0f, 0f));
			Ray ray2 = camera.ViewportPointToRay(new Vector3(1f, 0f, 0f));
			Ray ray3 = camera.ViewportPointToRay(new Vector3(0f, 1f, 0f));
			Vector3 point = ray.GetPoint(offset);
			Vector3 point2 = ray2.GetPoint(offset);
			Vector3 point3 = ray3.GetPoint(offset);
			Rect pixelRect = camera.pixelRect;
			float num = (float)Screen.width / pixelRect.width;
			float num2 = (float)Screen.height / pixelRect.height;
			Vector3 vector = (point2 - point) / Screen.width * num;
			Vector3 vector2 = (point3 - point) / Screen.height * num2;
			Vector4 column = point - vector * pixelRect.x - vector2 * pixelRect.y;
			column.w = 1f;
			return new Matrix4x4(vector, vector2, ray.direction, column);
		}

		public static Matrix4x4 FarClipPlaneViewportMatrix(Camera camera)
		{
			Ray r = camera.ViewportPointToRay(new Vector3(0f, 0f, 0f));
			Ray r2 = camera.ViewportPointToRay(new Vector3(1f, 0f, 0f));
			Ray r3 = camera.ViewportPointToRay(new Vector3(0f, 1f, 0f));
			Plane p = new Plane(camera.transform.forward, camera.transform.TransformPoint(0f, 0f, camera.farClipPlane));
			Vector3 hit = Vector3.zero;
			p.RaycastDoublesided(r, ref hit);
			Vector3 hit2 = Vector3.zero;
			p.RaycastDoublesided(r2, ref hit2);
			Vector3 hit3 = Vector3.zero;
			p.RaycastDoublesided(r3, ref hit3);
			Vector3 vector = hit2 - hit;
			Vector3 vector2 = hit3 - hit;
			Vector4 column = hit;
			column.w = 1f;
			return new Matrix4x4(vector, vector2, r.direction, column);
		}

		public static string GetInfo(this Matrix4x4 tm)
		{
			Vector3 position = tm.GetPosition();
			return string.Format(arg1: tm.GetRotation().eulerAngles.ToString("F0"), format: "pos:{0} rot:{1}", arg0: position.ToString("F3"));
		}
	}
}
