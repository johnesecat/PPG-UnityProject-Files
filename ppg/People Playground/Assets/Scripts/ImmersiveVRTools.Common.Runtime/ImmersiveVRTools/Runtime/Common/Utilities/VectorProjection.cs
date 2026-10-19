using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public static class VectorProjection
	{
		public static bool GlobalDebug = false;

		public static float GlobalScale = 1f;

		public static float GetPercentagePointProgressAlongLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point, bool isDebug = false)
		{
			if (isDebug || GlobalDebug)
			{
				DrawDebug(point, lineStart, lineEnd);
			}
			Vector3 vector = lineEnd - lineStart;
			return Vector3.Dot(point - lineStart, vector.normalized) / vector.magnitude;
		}

		public static Vector3 ClampPoint(Vector3 point, Vector3 segmentStart, Vector3 segmentEnd, bool isDebug = false)
		{
			if (isDebug)
			{
				DrawDebug(point, segmentStart, segmentEnd);
			}
			return ClampProjection(ProjectPoint(point, segmentStart, segmentEnd), segmentStart, segmentEnd);
		}

		public static Vector3 ProjectPoint(Vector3 point, Vector3 segmentStart, Vector3 segmentEnd, bool isDebug = false)
		{
			if (isDebug || GlobalDebug)
			{
				DrawDebug(point, segmentStart, segmentEnd);
			}
			return segmentStart + Vector3.Project(point - segmentStart, segmentEnd - segmentStart);
		}

		private static Vector3 ClampProjection(Vector3 point, Vector3 start, Vector3 end)
		{
			float sqrMagnitude = (point - start).sqrMagnitude;
			float sqrMagnitude2 = (point - end).sqrMagnitude;
			float sqrMagnitude3 = (start - end).sqrMagnitude;
			Vector3 result = point;
			if (sqrMagnitude > sqrMagnitude3 || sqrMagnitude2 > sqrMagnitude3)
			{
				result = ((sqrMagnitude > sqrMagnitude2) ? end : start);
			}
			return result;
		}

		private static void DrawDebug(Vector3 point, Vector3 start, Vector3 end)
		{
		}
	}
}
