using System;
using System.Diagnostics;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Debug
{
	public static class DebugDraw
	{
		public const int DrawLineLayer = 4;

		public const int DrawTextDefaultSize = 12;

		public static Color DrawDefaultColor = Color.white;

		private static RuntimeDebugDraw _rtDraw;

		private static string HIDDEN_GO_NAME = "________HIDDEN_C4F6A87F298241078E21C0D7C1D87A76_";

		public static Camera GetDebugDrawCamera()
		{
			return Camera.main;
		}

		[Conditional("DEBUG")]
		public static void Text(Vector3 pos, string text, Color color, int size, float duration, bool popUp)
		{
			CheckAndBuildHiddenRTDrawObject();
			_rtDraw.RegisterDrawText(pos, text, color, size, duration, popUp);
		}

		[Conditional("DEBUG")]
		public static void AttachText(Transform transform, Func<string> strFunc, Vector3 offset, Color color, int size)
		{
			CheckAndBuildHiddenRTDrawObject();
			_rtDraw.RegisterAttachText(transform, strFunc, offset, color, size);
		}

		[Conditional("DEBUG")]
		public static void LocalDirections(Transform transform, float length = 0.1f)
		{
		}

		[Conditional("DEBUG")]
		public static void Line(Vector3 start, Vector3 end, Color col)
		{
			UnityEngine.Debug.DrawLine(start, end, col);
		}

		[Conditional("DEBUG")]
		public static void Ray(Vector3 start, Vector3 dir, Color col, float length = 1f)
		{
			UnityEngine.Debug.DrawLine(start, start + dir.normalized * length, col);
		}

		[Conditional("DEBUG")]
		public static void Cube(Vector3 pos, Color col, Vector3 scale)
		{
			Vector3 vector = scale * 0.5f;
			Vector3[] array = new Vector3[8]
			{
				pos + new Vector3(vector.x, vector.y, vector.z),
				pos + new Vector3(0f - vector.x, vector.y, vector.z),
				pos + new Vector3(0f - vector.x, 0f - vector.y, vector.z),
				pos + new Vector3(vector.x, 0f - vector.y, vector.z),
				pos + new Vector3(vector.x, vector.y, 0f - vector.z),
				pos + new Vector3(0f - vector.x, vector.y, 0f - vector.z),
				pos + new Vector3(0f - vector.x, 0f - vector.y, 0f - vector.z),
				pos + new Vector3(vector.x, 0f - vector.y, 0f - vector.z)
			};
			UnityEngine.Debug.DrawLine(array[0], array[1], col);
			UnityEngine.Debug.DrawLine(array[1], array[2], col);
			UnityEngine.Debug.DrawLine(array[2], array[3], col);
			UnityEngine.Debug.DrawLine(array[3], array[0], col);
		}

		[Conditional("DEBUG")]
		public static void Rect(Rect rect, Color col)
		{
			new Vector3(rect.x + rect.width / 2f, rect.y + rect.height / 2f, 0f);
			new Vector3(rect.width, rect.height, 0f);
		}

		[Conditional("DEBUG")]
		public static void Rect(Vector3 pos, Color col, Vector3 scale)
		{
			Vector3 vector = scale * 0.5f;
			Vector3[] array = new Vector3[4]
			{
				pos + new Vector3(vector.x, vector.y, vector.z),
				pos + new Vector3(0f - vector.x, vector.y, vector.z),
				pos + new Vector3(0f - vector.x, 0f - vector.y, vector.z),
				pos + new Vector3(vector.x, 0f - vector.y, vector.z)
			};
			UnityEngine.Debug.DrawLine(array[0], array[1], col);
			UnityEngine.Debug.DrawLine(array[1], array[2], col);
			UnityEngine.Debug.DrawLine(array[2], array[3], col);
			UnityEngine.Debug.DrawLine(array[3], array[0], col);
		}

		[Conditional("DEBUG")]
		public static void Point(Vector3 pos, Color col, float scale)
		{
			Vector3[] array = new Vector3[6]
			{
				pos + Vector3.up * scale,
				pos - Vector3.up * scale,
				pos + Vector3.right * scale,
				pos - Vector3.right * scale,
				pos + Vector3.forward * scale,
				pos - Vector3.forward * scale
			};
			UnityEngine.Debug.DrawLine(array[0], array[1], col);
			UnityEngine.Debug.DrawLine(array[2], array[3], col);
			UnityEngine.Debug.DrawLine(array[4], array[5], col);
			UnityEngine.Debug.DrawLine(array[0], array[2], col);
			UnityEngine.Debug.DrawLine(array[0], array[3], col);
			UnityEngine.Debug.DrawLine(array[0], array[4], col);
			UnityEngine.Debug.DrawLine(array[0], array[5], col);
			UnityEngine.Debug.DrawLine(array[1], array[2], col);
			UnityEngine.Debug.DrawLine(array[1], array[3], col);
			UnityEngine.Debug.DrawLine(array[1], array[4], col);
			UnityEngine.Debug.DrawLine(array[1], array[5], col);
			UnityEngine.Debug.DrawLine(array[4], array[2], col);
			UnityEngine.Debug.DrawLine(array[4], array[3], col);
			UnityEngine.Debug.DrawLine(array[5], array[2], col);
			UnityEngine.Debug.DrawLine(array[5], array[3], col);
		}

		[Conditional("DEBUG")]
		public static void Text(Vector3 pos, string text)
		{
		}

		[Conditional("DEBUG")]
		public static void Text(Vector3 pos, string text, Color color)
		{
		}

		[Conditional("DEBUG")]
		public static void Text(Vector3 pos, string text, Color color, int size)
		{
		}

		[Conditional("DEBUG")]
		public static void Text(Vector3 pos, string text, Color color, int size, float duration)
		{
		}

		[Conditional("DEBUG")]
		public static void AttachText(Transform transform, Func<string> strFunc)
		{
		}

		[Conditional("DEBUG")]
		public static void AttachText(Transform transform, Func<string> strFunc, Vector3 offset)
		{
		}

		[Conditional("DEBUG")]
		public static void AttachText(Transform transform, Func<string> strFunc, Vector3 offset, Color color)
		{
		}

		private static void CheckAndBuildHiddenRTDrawObject()
		{
			if (_rtDraw != null)
			{
				return;
			}
			_rtDraw = UnityEngine.Object.FindObjectOfType<RuntimeDebugDraw>();
			if (!(_rtDraw != null))
			{
				GameObject gameObject = new GameObject(HIDDEN_GO_NAME);
				GameObject gameObject2 = new GameObject(HIDDEN_GO_NAME);
				gameObject2.transform.parent = gameObject.transform;
				_rtDraw = gameObject2.AddComponent<RuntimeDebugDraw>();
				gameObject.hideFlags = HideFlags.HideAndDontSave;
				if (Application.isPlaying)
				{
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
				}
			}
		}
	}
}
