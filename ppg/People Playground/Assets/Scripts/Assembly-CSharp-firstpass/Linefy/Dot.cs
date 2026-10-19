using System;
using UnityEngine;

namespace Linefy
{
	[Serializable]
	public struct Dot
	{
		public bool enabled;

		public Vector3 position;

		public Vector2 size2d;

		public int rectIndex;

		public Color color;

		public Vector2 offset;

		public float size
		{
			get
			{
				return size2d.x;
			}
			set
			{
				size2d.Set(value, value);
			}
		}

		public Dot(Vector3 pos, float size, int rectIndex, Color color)
		{
			enabled = true;
			position = pos;
			size2d = new Vector2(size, size);
			this.color = color;
			this.rectIndex = rectIndex;
			offset = Vector2.zero;
		}

		public Dot(Vector3 pos, float size, int rectIndex, Color color, Vector2 pixelOffset)
		{
			enabled = true;
			position = pos;
			size2d = new Vector2(size, size);
			this.color = color;
			this.rectIndex = rectIndex;
			offset = pixelOffset;
		}

		public Dot(Vector3 pos, float size, DefaultDotAtlasShape shape, int outlineWidth, Color color)
		{
			enabled = true;
			position = pos;
			this.color = color;
			int num = Mathf.Clamp((int)shape, 0, 8);
			outlineWidth = Mathf.Clamp(outlineWidth, 0, 16);
			rectIndex = num * 16 + outlineWidth;
			size2d = new Vector2(size, size);
			offset = Vector2.zero;
		}

		public Dot(Vector3 pos, Vector2 size2d, DefaultDotAtlasShape shape, int outlineWidth, Color color)
		{
			enabled = true;
			position = pos;
			this.color = color;
			int num = Mathf.Clamp((int)shape, 0, 8);
			outlineWidth = Mathf.Clamp(outlineWidth, 0, 16);
			rectIndex = num * 16 + outlineWidth;
			this.size2d = size2d;
			offset = Vector2.zero;
		}

		public static implicit operator PolylineVertex(Dot d)
		{
			return new PolylineVertex(d.position, d.color, d.size);
		}

		public override string ToString()
		{
			return $"enabled:{enabled} positions:{position} size2d:{size2d} rectIndex:{rectIndex} color:{color} pixelOffset:{offset} \n";
		}
	}
}
