using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Primitives/Squircle")]
	public class UISquircle : UIPrimitiveBase
	{
		public enum Type
		{
			Classic,
			Scaled
		}

		private const float C = 1f;

		[Space]
		public Type squircleType = Type.Scaled;

		[Range(1f, 40f)]
		public float n = 4f;

		[Min(0.1f)]
		public float delta = 5f;

		public float quality = 0.1f;

		[Min(0f)]
		public float radius = 1000f;

		private float a;

		private float b;

		private List<Vector2> vert = new List<Vector2>();

		private float SquircleFunc(float t, bool xByY)
		{
			if (xByY)
			{
				return (float)Math.Pow(1.0 - Math.Pow(t / a, n), 1f / n) * b;
			}
			return (float)Math.Pow(1.0 - Math.Pow(t / b, n), 1f / n) * a;
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = base.rectTransform.rect.width / 2f;
			float num4 = base.rectTransform.rect.height / 2f;
			if (squircleType == Type.Classic)
			{
				a = num3;
				b = num4;
			}
			else
			{
				a = Mathf.Min(num3, num4, radius);
				b = a;
				num = num3 - a;
				num2 = num4 - a;
			}
			float num5 = 0f;
			float num6 = 1f;
			vert.Clear();
			vert.Add(new Vector2(0f, num4));
			for (; num5 < num6; num5 += delta)
			{
				num6 = SquircleFunc(num5, xByY: true);
				vert.Add(new Vector2(num + num5, num2 + num6));
			}
			if (float.IsNaN(vert.Last().y))
			{
				vert.RemoveAt(vert.Count - 1);
			}
			while (num6 > 0f)
			{
				num5 = SquircleFunc(num6, xByY: false);
				vert.Add(new Vector2(num + num5, num2 + num6));
				num6 -= delta;
			}
			vert.Add(new Vector2(num3, 0f));
			for (int i = 1; i < vert.Count - 1; i++)
			{
				if (vert[i].x < vert[i].y)
				{
					if (vert[i - 1].y - vert[i].y < quality)
					{
						vert.RemoveAt(i);
						i--;
					}
				}
				else if (vert[i].x - vert[i - 1].x < quality)
				{
					vert.RemoveAt(i);
					i--;
				}
			}
			vert.AddRange(from t in vert.AsEnumerable().Reverse()
				select new Vector2(t.x, 0f - t.y));
			vert.AddRange(from t in vert.AsEnumerable().Reverse()
				select new Vector2(0f - t.x, t.y));
			vh.Clear();
			for (int j = 0; j < vert.Count - 1; j++)
			{
				vh.AddVert(vert[j], color, Vector2.zero);
				vh.AddVert(vert[j + 1], color, Vector2.zero);
				vh.AddVert(Vector2.zero, color, Vector2.zero);
				vh.AddTriangle(j * 3, j * 3 + 1, j * 3 + 2);
			}
		}
	}
}
