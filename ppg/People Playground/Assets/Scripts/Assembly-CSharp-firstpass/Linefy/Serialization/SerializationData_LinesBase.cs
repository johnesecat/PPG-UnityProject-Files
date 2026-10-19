using System;
using UnityEngine;

namespace Linefy.Serialization
{
	[Serializable]
	public class SerializationData_LinesBase : SerializationData_PrimitivesGroup
	{
		[Tooltip("The smoothness of the edges.  Works only when transparent material is on. This value defines the distance the color.alpha decays from the edge of the line. Can be used to draw  anti-aliased lines. When used widthMode: PixelsBillboard, WorldspaceBillboard, PercentOfScreenHeight is measured with onscreen pixels. When used  widthMode: WorldspaceXY -  in world units.")]
		public float feather = 2f;

		[Tooltip("The multiplier of x texture coordinate (also known as tiling).")]
		public float textureScale = 1f;

		[Tooltip("The offset of x texture coordinate.  ")]
		public float textureOffset;

		[Tooltip("When on the textures offset foreach vertex will recalculated automatically when its positions changed")]
		public bool autoTextureOffset;

		public SerializationData_LinesBase(float width, Color color, float feather)
		{
			widthMultiplier = width;
			colorMultiplier = color;
			this.feather = feather;
			transparent = true;
		}

		public SerializationData_LinesBase()
		{
		}
	}
}
