using System;
using Linefy.Serialization;
using UnityEngine;

namespace Linefy
{
	public class Lines : LinesBase
	{
		private int id_autoTextureOffset = Shader.PropertyToID("_AutoTextureOffset");

		private Vector3[] mpos;

		private Vector3[] mnorm;

		private Color[] mcolors;

		private Vector2[] muvs0;

		private Vector2[] muvs2;

		private int[] mtriangles;

		private bool posDirty;

		private bool colorsDirty;

		private bool widthDirty;

		private bool topologyIsDirty;

		private bool visualTopologyIsDirty;

		private const int vertsStride = 4;

		private const int trisStride = 6;

		public override int maxCount => 160000;

		public Line this[int lineIdx]
		{
			get
			{
				Line result = default(Line);
				if (validateLineIdx(lineIdx))
				{
					int num = lineIdx * 4;
					int num2 = num + 2;
					result.positionA = mpos[num];
					result.positionB = mpos[num2];
					result.colorA = mcolors[num];
					result.colorB = mcolors[num2];
					result.widthA = muvs0[num].y;
					result.widthB = muvs0[num2].y;
					result.textureOffsetA = muvs0[num].x;
					result.textureOffsetB = muvs0[num].x;
				}
				return result;
			}
			set
			{
				if (validateLineIdx(lineIdx))
				{
					int num = lineIdx * 4;
					int num2 = num + 1;
					int num3 = num + 2;
					int num4 = num + 3;
					mpos[num] = value.positionA;
					mpos[num2] = value.positionA;
					mpos[num3] = value.positionB;
					mpos[num4] = value.positionB;
					mnorm[num] = value.positionB;
					mnorm[num2] = value.positionB;
					mnorm[num3] = value.positionA;
					mnorm[num4] = value.positionA;
					mcolors[num] = value.colorA;
					mcolors[num2] = value.colorA;
					mcolors[num3] = value.colorB;
					mcolors[num4] = value.colorB;
					Vector2 vector = new Vector3(value.textureOffsetA, value.widthA);
					Vector2 vector2 = new Vector3(value.textureOffsetB, value.widthB);
					muvs0[num] = vector;
					muvs0[num2] = vector;
					muvs0[num3] = vector2;
					muvs0[num4] = vector2;
					posDirty = true;
					colorsDirty = true;
					widthDirty = true;
				}
			}
		}

		public override bool autoTextureOffset
		{
			get
			{
				return _autoTextureOffset;
			}
			set
			{
				if (_autoTextureOffset != value)
				{
					_autoTextureOffset = value;
					base.material.SetFloat(id_autoTextureOffset, _autoTextureOffset ? 1 : 0);
				}
			}
		}

		protected override void SetDirtyAttributes()
		{
			boundsDirty = true;
			posDirty = true;
			colorsDirty = true;
			widthDirty = true;
			topologyIsDirty = true;
			visualTopologyIsDirty = true;
		}

		public Lines(int count)
		{
			InternalConstructor("", count, transparent: false, 0f);
		}

		public Lines(int count, bool transparent)
		{
			InternalConstructor("", count, transparent, 1f);
		}

		public Lines(string name, int count, bool transparent, float feather)
		{
			InternalConstructor(name, count, transparent, feather);
		}

		public Lines(int count, bool transparent, float feather, float widthMult, Color colorMult)
		{
			InternalConstructor(name, count, transparent, feather);
			base.widthMultiplier = widthMult;
			base.colorMultiplier = colorMult;
		}

		public Lines(string name, Line[] lines, bool transparent, float feather)
		{
			InternalConstructor(name, lines.Length, transparent, feather);
			for (int i = 0; i < lines.Length; i++)
			{
				this[i] = lines[i];
			}
		}

		public Lines(SerializationData_Lines data)
		{
			LoadSerializationData(data);
		}

		private void InternalConstructor(string name, int count, bool transparent, float feather)
		{
			base.name = name;
			base.transparent = transparent;
			base.feather = feather;
			this.count = count;
			for (int i = 0; i < this.count; i++)
			{
				this[i] = new Line(1f, Color.white);
			}
		}

		protected override void SetCapacity(int prevCapacity)
		{
			int newSize = capacity * 4;
			Array.Resize(ref mpos, newSize);
			Array.Resize(ref mnorm, newSize);
			Array.Resize(ref mcolors, newSize);
			Array.Resize(ref muvs0, newSize);
			Array.Resize(ref muvs2, newSize);
			int newSize2 = capacity * 6;
			Array.Resize(ref mtriangles, newSize2);
			for (int i = prevCapacity; i < capacity; i++)
			{
				int num = i * 4;
				int num2 = num + 1;
				int num3 = num + 2;
				int num4 = num + 3;
				muvs2[num].x = 0f;
				muvs2[num2].x = 1f;
				muvs2[num3].x = 2f;
				muvs2[num4].x = 3f;
				int num5 = i * 6;
				mtriangles[num5 + 2] = num4;
				mtriangles[num5 + 1] = num2;
				mtriangles[num5] = num;
				mtriangles[num5 + 5] = num3;
				mtriangles[num5 + 4] = num2;
				mtriangles[num5 + 3] = num4;
			}
			SetDirtyAttributes();
		}

		protected override void SetCount(int prevCount)
		{
			int num = Mathf.Min(prevCount, _count);
			int num2 = Mathf.Max(prevCount, _count);
			int num3 = Mathf.Clamp(num - 1, 0, capacity);
			num2 = Mathf.Clamp(num2 + 1, 0, capacity);
			for (int i = num3; i < num2; i++)
			{
				int num4 = ((i >= _count) ? 1 : 0);
				if (num4 == 0)
				{
					this[i] = new Line(1f, Color.white);
				}
				int num5 = i * 4;
				int num6 = num5 + 1;
				int num7 = num5 + 2;
				int num8 = num5 + 3;
				muvs2[num5].y = num4;
				muvs2[num6].y = num4;
				muvs2[num7].y = num4;
				muvs2[num8].y = num4;
			}
			visualTopologyIsDirty = true;
		}

		protected override void PreDraw()
		{
			base.PreDraw();
			if (topologyIsDirty)
			{
				mesh.Clear();
			}
			if (posDirty)
			{
				mesh.vertices = mpos;
				mesh.normals = mnorm;
				posDirty = false;
				boundsDirty = true;
			}
			if (colorsDirty)
			{
				mesh.colors = mcolors;
				colorsDirty = false;
			}
			if (widthDirty)
			{
				mesh.uv = muvs0;
				widthDirty = false;
			}
			if (visualTopologyIsDirty)
			{
				mesh.uv2 = muvs2;
				visualTopologyIsDirty = false;
			}
			if (topologyIsDirty)
			{
				mesh.triangles = mtriangles;
				topologyIsDirty = false;
			}
			if (boundsDirty)
			{
				if (base.boundSize <= 0f)
				{
					mesh.RecalculateBounds();
					mBounds = mesh.bounds;
				}
				mesh.bounds = mBounds;
				boundsDirty = false;
			}
		}

		public void SetPosition(int lineIdx, Vector3 positionA, Vector3 positionB)
		{
			if (validateLineIdx(lineIdx))
			{
				int num = lineIdx * 4;
				int num2 = num + 1;
				int num3 = num + 2;
				int num4 = num + 3;
				mpos[num] = positionA;
				mpos[num2] = positionA;
				mpos[num3] = positionB;
				mpos[num4] = positionB;
				mnorm[num] = positionB;
				mnorm[num2] = positionB;
				mnorm[num3] = positionA;
				mnorm[num4] = positionA;
				posDirty = true;
			}
		}

		public void SetTextureOffset(int lineIdx, float textureOffsetA, float textureOffsetB)
		{
			if (validateLineIdx(lineIdx))
			{
				int num = lineIdx * 4;
				int num2 = num + 1;
				int num3 = num + 2;
				int num4 = num + 3;
				muvs0[num].x = textureOffsetA;
				muvs0[num2].x = textureOffsetA;
				muvs0[num3].x = textureOffsetB;
				muvs0[num4].x = textureOffsetB;
				widthDirty = true;
			}
		}

		public void SetColor(int lineIdx, Color color)
		{
			if (validateLineIdx(lineIdx))
			{
				int num = lineIdx * 4;
				int num2 = num + 1;
				int num3 = num + 2;
				int num4 = num + 3;
				mcolors[num] = color;
				mcolors[num2] = color;
				mcolors[num3] = color;
				mcolors[num4] = color;
				colorsDirty = true;
			}
		}

		public void SetColor(int lineIdx, Color colorA, Color colorB)
		{
			if (validateLineIdx(lineIdx))
			{
				int num = lineIdx * 4;
				int num2 = num + 1;
				int num3 = num + 2;
				int num4 = num + 3;
				mcolors[num] = colorA;
				mcolors[num2] = colorA;
				mcolors[num3] = colorB;
				mcolors[num4] = colorB;
				colorsDirty = true;
			}
		}

		public void SetColorAlpha(int lineIdx, float alphaA, float alphaB)
		{
			if (validateLineIdx(lineIdx))
			{
				int num = lineIdx * 4;
				int num2 = num + 1;
				int num3 = num + 2;
				int num4 = num + 3;
				mcolors[num].a = alphaA;
				mcolors[num2].a = alphaA;
				mcolors[num3].a = alphaB;
				mcolors[num4].a = alphaB;
				colorsDirty = true;
			}
		}

		public void SetWidth(int lineIdx, float widthA, float widthB)
		{
			int num = lineIdx * 4;
			int num2 = num + 1;
			int num3 = num + 2;
			int num4 = num + 3;
			muvs0[num].y = widthA;
			muvs0[num2].y = widthA;
			muvs0[num3].y = widthB;
			muvs0[num4].y = widthB;
			widthDirty = true;
		}

		public void SetWidth(int lineIdx, float width)
		{
			if (validateLineIdx(lineIdx))
			{
				int num = lineIdx * 4;
				int num2 = num + 1;
				int num3 = num + 2;
				int num4 = num + 3;
				muvs0[num].y = width;
				muvs0[num2].y = width;
				muvs0[num3].y = width;
				muvs0[num4].y = width;
				widthDirty = true;
			}
		}

		protected override string opaqueShaderName()
		{
			if (base.widthMode == WidthMode.WorldspaceBillboard)
			{
				return "Hidden/Linefy/LinesWorldspaceBillboard";
			}
			if (base.widthMode == WidthMode.WorldspaceXY)
			{
				return "Hidden/Linefy/LinesWorldspaceXY";
			}
			return "Hidden/Linefy/LinesPixelBillboard";
		}

		protected override string transparentShaderName()
		{
			if (base.widthMode == WidthMode.WorldspaceBillboard)
			{
				return "Hidden/Linefy/LinesTransparentWorldspaceBillboard";
			}
			if (base.widthMode == WidthMode.WorldspaceXY)
			{
				return "Hidden/Linefy/LinesTransparentWorldspaceXY";
			}
			return "Hidden/Linefy/LinesTransparentPixelBillboard";
		}

		protected override void OnAfterMaterialCreated()
		{
			base.OnAfterMaterialCreated();
			base.material.SetFloat(id_autoTextureOffset, _autoTextureOffset ? 1 : 0);
		}

		public void LoadSerializationData(SerializationData_Lines inputData)
		{
			if (inputData == null)
			{
				Debug.LogError("Lines.SetSerializableData (inputData)  inputData  == null");
				return;
			}
			LoadSerializationData((SerializationData_LinesBase)inputData);
			autoTextureOffset = inputData.autoTextureOffset;
		}

		public void SaveSerializationData(ref SerializationData_Lines outputData)
		{
			if (outputData == null)
			{
				Debug.LogError("Lines.GetSerializableData (outputData)  outputData == null");
				return;
			}
			SaveSerializationData(outputData);
			outputData.autoTextureOffset = autoTextureOffset;
		}

		private bool validateLineIdx(int vertexIdx)
		{
			return true;
		}

		public override void GetStatistic(ref int linesCount, ref int totallinesCount, ref int dotsCount, ref int totalDotsCount, ref int polylinesCount, ref int totalPolylineVerticesCount)
		{
			linesCount++;
			totallinesCount += count;
		}
	}
}
