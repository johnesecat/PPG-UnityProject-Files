using System;
using UnityEngine;

namespace Linefy
{
	[Serializable]
	public struct Line
	{
		public Vector3 positionA;

		public Vector3 positionB;

		public float widthA;

		public float widthB;

		public Color colorA;

		public Color colorB;

		public float textureOffsetA;

		public float textureOffsetB;

		public Line(Vector3 a, Vector3 b, Color ca, Color cb, float widthA, float widthB)
		{
			positionA = a;
			positionB = b;
			colorA = ca;
			colorB = cb;
			this.widthA = widthA;
			this.widthB = widthB;
			textureOffsetA = 0f;
			textureOffsetB = 1f;
		}

		public Line(Vector3 a, Vector3 b, Color ca, Color cb, float widthA, float widthB, float textureOffsetA, float textureOffsetB)
		{
			positionA = a;
			positionB = b;
			colorA = ca;
			colorB = cb;
			this.widthA = widthA;
			this.widthB = widthB;
			this.textureOffsetA = textureOffsetA;
			this.textureOffsetB = textureOffsetB;
		}

		public Line(Vector3 a, Vector3 b, Color ca, Color cb, float width)
		{
			positionA = a;
			positionB = b;
			colorA = ca;
			colorB = cb;
			widthA = width;
			widthB = width;
			textureOffsetA = 0f;
			textureOffsetB = 1f;
		}

		public Line(Vector3 a, Vector3 b, Color color)
		{
			positionA = a;
			positionB = b;
			colorA = color;
			colorB = color;
			widthA = 1f;
			widthB = 1f;
			textureOffsetA = 0f;
			textureOffsetB = 1f;
		}

		public Line(Vector3 a, Vector3 b, Color color, float width)
		{
			positionA = a;
			positionB = b;
			colorA = color;
			colorB = color;
			widthA = width;
			widthB = width;
			textureOffsetA = 0f;
			textureOffsetB = 1f;
		}

		public Line(Vector3 a, Vector3 b)
		{
			positionA = a;
			positionB = b;
			colorA = Color.white;
			colorB = Color.white;
			widthA = 1f;
			widthB = 1f;
			textureOffsetA = 0f;
			textureOffsetB = 1f;
		}

		public Line(float width, Color color)
		{
			positionA = Vector3.zero;
			positionB = Vector3.zero;
			widthA = width;
			widthB = width;
			colorA = color;
			colorB = color;
			textureOffsetA = 0f;
			textureOffsetB = 1f;
		}

		public override string ToString()
		{
			return $"A:{positionA} B:{positionB} widthA:{widthA} widthB:{widthB} colorA:{colorA} colorB:{colorB}";
		}
	}
}
