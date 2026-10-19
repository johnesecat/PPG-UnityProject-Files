using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Linefy
{
	[Serializable]
	[Obsolete("VisualPropertiesBlock is depricated. Use classes from Linefy.Serialization namespace")]
	public struct VisualPropertiesBlock
	{
		public bool transparent;

		public float widthMuliplier;

		public WidthMode widthMode;

		public Color colorMuliplier;

		public float feather;

		public int renderOrder;

		public CompareFunction zTest;

		public float viewOffset;

		public float depthOffset;

		public Texture texture;

		public VisualPropertiesBlock(float width, Color color, bool transparent)
		{
			this.transparent = transparent;
			widthMuliplier = width;
			colorMuliplier = color;
			feather = 1f;
			renderOrder = 0;
			viewOffset = 0f;
			depthOffset = 0f;
			zTest = CompareFunction.LessEqual;
			widthMode = WidthMode.PixelsBillboard;
			texture = null;
		}

		public VisualPropertiesBlock(float width, Color color, float feather)
		{
			transparent = true;
			widthMuliplier = width;
			colorMuliplier = color;
			this.feather = feather;
			renderOrder = 0;
			viewOffset = 0f;
			depthOffset = 0f;
			zTest = CompareFunction.LessEqual;
			widthMode = WidthMode.PixelsBillboard;
			texture = null;
		}

		public VisualPropertiesBlock(float width, Color color, float feather, int renderOrder)
		{
			transparent = true;
			widthMuliplier = width;
			colorMuliplier = color;
			this.feather = feather;
			this.renderOrder = renderOrder;
			viewOffset = 0f;
			depthOffset = 0f;
			zTest = CompareFunction.LessEqual;
			widthMode = WidthMode.PixelsBillboard;
			texture = null;
		}

		public VisualPropertiesBlock(float width, Color color)
		{
			transparent = false;
			widthMuliplier = width;
			colorMuliplier = color;
			feather = 1f;
			renderOrder = 0;
			viewOffset = 0f;
			depthOffset = 0f;
			zTest = CompareFunction.LessEqual;
			widthMode = WidthMode.PixelsBillboard;
			texture = null;
		}
	}
}
