using System;
using UnityEngine;

namespace Linefy.Serialization
{
	[Serializable]
	public class SerializationData_Polyline : SerializationData_LinesBase
	{
		[Tooltip("If enabled,  connects first and last vertex. ")]
		public bool isClosed;

		[Tooltip("The texture offset of the last virtual vertex when the polyline is closed.")]
		public float lastVertexTextureOffset = 1f;

		public SerializationData_Polyline()
		{
			name = "new Polyline";
			transparent = true;
			feather = 2f;
			widthMultiplier = 20f;
			isClosed = true;
		}

		public SerializationData_Polyline(float width, Color color, float feather, bool isClosed)
		{
			name = "new Polyline";
			transparent = true;
			base.feather = feather;
			widthMultiplier = width;
			colorMultiplier = color;
			this.isClosed = isClosed;
		}

		public SerializationData_Polyline(float lastVertexTextureOffset)
		{
			name = "new Polyline";
			transparent = true;
			feather = 2f;
			widthMultiplier = 20f;
			this.lastVertexTextureOffset = lastVertexTextureOffset;
			isClosed = true;
		}
	}
}
