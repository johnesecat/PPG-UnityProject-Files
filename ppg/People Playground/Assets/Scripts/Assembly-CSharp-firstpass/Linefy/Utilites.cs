using System;
using UnityEngine;

namespace Linefy
{
	public static class Utilites
	{
		[Obsolete("NearClipPlaneGUISpaceMatrix is Obsolete , use NearClipPlaneMatrix.GUISpace(camera, offset)")]
		public static Matrix4x4 NearClipPlaneGUISpaceMatrix(Camera camera, float offset)
		{
			return NearClipPlaneMatrix.GUISpace(camera, offset);
		}

		[Obsolete("LinearToSin() is Obsolete , use MathUtility.LinearToSin(camera, offset)")]
		public static float LinearToSin(float t)
		{
			return 1f - (Mathf.Sin(t * 3.141592f + 1.5708f) * 0.49999f + 0.5f);
		}
	}
}
