using UnityEngine;

namespace Linefy.Internal
{
	public static class PlaneExtension
	{
		public static bool RaycastXYToLocal(this Matrix4x4 tm, Ray r, ref Vector2 result)
		{
			Plane plane = new Plane(tm.GetColumn(2), tm.GetColumn(3));
			float enter = 0f;
			if (plane.Raycast(r, out enter))
			{
				result = tm.inverse.MultiplyPoint3x4(r.GetPoint(enter));
				return true;
			}
			return false;
		}

		public static bool RaycastDoublesided(this Plane p, Ray r, ref Vector3 hit)
		{
			_ = r.origin;
			float enter = 0f;
			if (p.Raycast(r, out enter))
			{
				hit = r.GetPoint(enter);
				return true;
			}
			Plane plane = p;
			plane.normal = -plane.normal;
			if (plane.Raycast(r, out enter))
			{
				hit = r.GetPoint(enter);
				return true;
			}
			return false;
		}
	}
}
