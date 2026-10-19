using UnityEngine;

namespace Linefy.Internal
{
	public class Tetrahedron
	{
		private Vector3 a;

		private Vector3 b;

		private Vector3 c;

		private Vector3 d;

		private Triangle3D t;

		public Matrix4x4 tm;

		public Matrix4x4 tminverse;

		public Tetrahedron(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
		{
			this.a = a;
			this.b = b;
			this.c = c;
			this.d = d;
			t = new Triangle3D(a, b, c);
			tm.SetColumn(0, b - a);
			tm.SetColumn(1, d - a);
			tm.SetColumn(2, c - a);
			tm.SetColumn(3, new Vector4(a.x, a.y, a.z, 1f));
			tminverse = tm.inverse;
		}

		public bool Test(Vector3 point, ref Vector4 coords)
		{
			Ray r = new Ray(d, point - d);
			Vector3 bary = Vector3.zero;
			Vector3 hit = Vector3.zero;
			if (t.Raycast(r, ref bary, ref hit))
			{
				coords = bary;
				float num = Vector3.Distance(d, hit);
				float num2 = Vector3.Distance(d, point);
				if (num2 > num)
				{
					return false;
				}
				coords.w = num2 / num;
				return true;
			}
			return false;
		}

		public static Color CalcColor(Vector4 adress, Color ca, Color cb, Color cc, Color cd)
		{
			return Color.LerpUnclamped(cd, ca * adress.x + cb * adress.y + cc * adress.z, adress.w);
		}

		public void DrawDebug(Color color)
		{
			Debug.DrawLine(a, b, color);
			Debug.DrawLine(b, c, color);
			Debug.DrawLine(c, a, color);
			Debug.DrawLine(d, a, color);
			Debug.DrawLine(d, b, color);
			Debug.DrawLine(d, c, color);
		}

		public float GetVolume()
		{
			return tm.determinant * 0.1666666f;
		}
	}
}
