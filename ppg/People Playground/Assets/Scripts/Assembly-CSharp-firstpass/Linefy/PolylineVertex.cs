using System;
using UnityEngine;

namespace Linefy
{
	[Serializable]
	public struct PolylineVertex : IVector3GetSet
	{
		public Vector3 position;

		public Color color;

		public float width;

		public float textureOffset;

		public Vector3 vector3
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
			}
		}

		public PolylineVertex(Vector3 pos, Color color, float width, float textureOffset)
		{
			position = pos;
			this.color = color;
			this.width = width;
			this.textureOffset = textureOffset;
		}

		public PolylineVertex(Vector3 pos, Color color, float width)
		{
			position = pos;
			this.color = color;
			this.width = width;
			textureOffset = 0f;
		}

		public static implicit operator Dot(PolylineVertex pv)
		{
			return new Dot(pv.position, pv.width, 0, pv.color);
		}

		public override string ToString()
		{
			return $"pos:{position} col:{color} width:{width} to:{textureOffset}";
		}

		public PolylineVertex Interpolate(PolylineVertex other, float t)
		{
			Vector3 pos = Vector3.LerpUnclamped(position, other.position, t);
			Color color = Color.LerpUnclamped(this.color, other.color, t);
			float num = Mathf.LerpUnclamped(width, other.width, t);
			float num2 = Mathf.LerpUnclamped(textureOffset, other.textureOffset, t);
			return new PolylineVertex(pos, color, num, num2);
		}
	}
}
