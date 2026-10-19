using System;
using Linefy.Internal;
using Linefy.Serialization;
using UnityEngine;

namespace Linefy
{
	public class Dots : PrimitivesGroup
	{
		private Vector3[] mpos;

		private Vector3[] mnorm;

		private Color[] mcolors;

		private Vector2[] muvs0;

		private Vector2[] muvs2;

		private int[] rectIDs;

		private int[] mtriangles;

		private bool posDirty;

		private bool colorsDirty;

		private bool normalsOrWidthDirty;

		private bool uvsDirty;

		private bool uvs2Dirty;

		private bool topologyDirty;

		private const int vertsStride = 4;

		private const int trisStride = 6;

		private int atlasModificationsHash;

		private DotsAtlas _atlas;

		private Vector3 np0 = new Vector3(-5f, 5f, 1f);

		private Vector3 np1 = new Vector3(-5f, -5f, 1f);

		private Vector3 np2 = new Vector3(5f, -5f, 1f);

		private Vector3 np3 = new Vector3(5f, 5f, 1f);

		private Color whiteColor = Color.white;

		private bool _pixelPerfect;

		public DotsAtlas atlas
		{
			get
			{
				return _atlas;
			}
			set
			{
				if (value == null)
				{
					value = DotsAtlas.Default;
					atlasModificationsHash = value.modificationHash - 1;
				}
				if (_atlas != value)
				{
					_atlas = value;
					atlasModificationsHash = _atlas.modificationHash - 1;
				}
			}
		}

		public override int maxCount => 160000;

		public Dot this[int dotIdx]
		{
			get
			{
				if (validateDotIdx(dotIdx))
				{
					int num = dotIdx * 4;
					Dot result = default(Dot);
					result.position = mpos[num];
					result.color = mcolors[num];
					result.rectIndex = rectIDs[dotIdx];
					result.size2d = mnorm[num + 3];
					result.enabled = mnorm[num].z == 1f;
					result.offset = muvs2[num];
					return result;
				}
				return default(Dot);
			}
			set
			{
				if (validateDotIdx(dotIdx))
				{
					int num = dotIdx * 4;
					int num2 = num + 1;
					int num3 = num + 2;
					int num4 = num + 3;
					float z = (value.enabled ? 1 : 0);
					Vector2 vector = value.size2d * 0.5f;
					mnorm[num] = new Vector3(0f - vector.x, vector.y, z);
					mnorm[num2] = new Vector3(0f - vector.x, 0f - vector.y, z);
					mnorm[num3] = new Vector3(vector.x, 0f - vector.y, z);
					mnorm[num4] = new Vector3(vector.x, vector.y, z);
					mpos[num] = value.position;
					mpos[num2] = value.position;
					mpos[num3] = value.position;
					mpos[num4] = value.position;
					rectIDs[dotIdx] = value.rectIndex;
					DotsAtlas.Rect rect = atlas.rects[MathUtility.RoundedArrayIdx(value.rectIndex, atlas.rects.Length)];
					muvs0[num] = rect.v0;
					muvs0[num2] = rect.v1;
					muvs0[num3] = rect.v2;
					muvs0[num4] = rect.v3;
					muvs2[num] = value.offset;
					muvs2[num2] = value.offset;
					muvs2[num3] = value.offset;
					muvs2[num4] = value.offset;
					mcolors[num] = value.color;
					mcolors[num2] = value.color;
					mcolors[num3] = value.color;
					mcolors[num4] = value.color;
					posDirty = true;
					colorsDirty = true;
					normalsOrWidthDirty = true;
					uvsDirty = true;
					uvs2Dirty = true;
				}
			}
		}

		public bool pixelPerfect
		{
			get
			{
				return _pixelPerfect;
			}
			set
			{
				if (value != _pixelPerfect)
				{
					_pixelPerfect = value;
					ResetMaterial();
				}
			}
		}

		protected override void SetDirtyAttributes()
		{
			boundsDirty = true;
			posDirty = true;
			colorsDirty = true;
			normalsOrWidthDirty = true;
			uvsDirty = true;
			uvsDirty = true;
			topologyDirty = true;
		}

		public Dots(SerializationData_Dots data)
		{
			LoadSerializationData(data);
		}

		public Dots(int count)
		{
			atlas = DotsAtlas.Default;
			this.count = count;
		}

		public Dots(int count, bool transparent)
		{
			atlas = DotsAtlas.Default;
			base.transparent = transparent;
			this.count = count;
		}

		public Dots(string name, int count, DotsAtlas atlas)
		{
			base.name = name;
			this.atlas = atlas;
			this.count = count;
		}

		public Dots(int count, DotsAtlas atlas)
		{
			this.atlas = atlas;
			this.count = count;
		}

		public Dots(int count, DotsAtlas atlas, bool transparent)
		{
			this.atlas = atlas;
			base.transparent = transparent;
			this.count = count;
		}

		protected override void SetCount(int prevCount)
		{
			for (int i = 0; i < capacity; i++)
			{
				SetEnabledUnchecked(i, i < _count);
			}
		}

		protected override void SetCapacity(int _prevCapacity)
		{
			int newSize = capacity * 4;
			Array.Resize(ref mpos, newSize);
			Array.Resize(ref mnorm, newSize);
			Array.Resize(ref mcolors, newSize);
			Array.Resize(ref muvs0, newSize);
			Array.Resize(ref muvs2, newSize);
			Array.Resize(ref rectIDs, capacity);
			int newSize2 = capacity * 6;
			Array.Resize(ref mtriangles, newSize2);
			for (int i = _prevCapacity; i < capacity; i++)
			{
				int num = i * 4;
				int num2 = num + 1;
				int num3 = num + 2;
				int num4 = num + 3;
				mnorm[num] = np0;
				mnorm[num2] = np1;
				mnorm[num3] = np2;
				mnorm[num4] = np3;
				mcolors[num] = whiteColor;
				mcolors[num2] = whiteColor;
				mcolors[num3] = whiteColor;
				mcolors[num4] = whiteColor;
				int num5 = i * 6;
				mtriangles[num5] = num;
				mtriangles[num5 + 1] = num2;
				mtriangles[num5 + 2] = num3;
				mtriangles[num5 + 3] = num;
				mtriangles[num5 + 4] = num3;
				mtriangles[num5 + 5] = num4;
				SetRectIndexUnchecked(i, 0);
			}
			SetDirtyAttributes();
		}

		public void SetEnabled(int dotIdx, bool enabled)
		{
			if (validateDotIdx(dotIdx))
			{
				SetEnabledUnchecked(dotIdx, enabled);
			}
		}

		private void SetEnabledUnchecked(int dotIdx, bool enabled)
		{
			int num = dotIdx * 4;
			int num2 = num + 1;
			int num3 = num + 2;
			int num4 = num + 3;
			float z = (enabled ? 1 : 0);
			mnorm[num].z = z;
			mnorm[num2].z = z;
			mnorm[num3].z = z;
			mnorm[num4].z = z;
			normalsOrWidthDirty = true;
		}

		protected override void PreDraw()
		{
			base.PreDraw();
			if (topologyDirty)
			{
				mesh.Clear();
			}
			if (atlas.modificationHash != atlasModificationsHash)
			{
				for (int i = 0; i < count; i++)
				{
					SetRectIndex(i, this[i].rectIndex);
				}
				atlasModificationsHash = atlas.modificationHash;
			}
			base.texture = atlas.texture;
			if (posDirty)
			{
				mesh.vertices = mpos;
				posDirty = false;
				boundsDirty = true;
			}
			if (colorsDirty)
			{
				mesh.colors = mcolors;
				colorsDirty = false;
			}
			if (uvsDirty)
			{
				mesh.uv = muvs0;
				uvsDirty = false;
			}
			if (normalsOrWidthDirty)
			{
				mesh.normals = mnorm;
				normalsOrWidthDirty = false;
			}
			if (uvs2Dirty)
			{
				mesh.uv2 = muvs2;
				uvs2Dirty = false;
			}
			if (topologyDirty)
			{
				mesh.triangles = mtriangles;
				topologyDirty = false;
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

		public void SetPosition(int dotIdx, Vector3 position)
		{
			if (validateDotIdx(dotIdx))
			{
				int num = dotIdx * 4;
				int num2 = num + 1;
				int num3 = num + 2;
				int num4 = num + 3;
				mpos[num] = position;
				mpos[num2] = position;
				mpos[num3] = position;
				mpos[num4] = position;
				posDirty = true;
			}
		}

		public void SetPositionAndWidth(int dotIdx, Vector3 position, float width)
		{
			if (validateDotIdx(dotIdx))
			{
				float num = 0.5f * width;
				int num2 = dotIdx * 4;
				int num3 = num2 + 1;
				int num4 = num2 + 2;
				int num5 = num2 + 3;
				mpos[num2] = position;
				mpos[num3] = position;
				mpos[num4] = position;
				mpos[num5] = position;
				mnorm[num2].x = 0f - num;
				mnorm[num2].y = num;
				mnorm[num3].x = 0f - num;
				mnorm[num3].y = 0f - num;
				mnorm[num4].x = num;
				mnorm[num4].y = 0f - num;
				mnorm[num5].x = num;
				mnorm[num5].y = num;
				normalsOrWidthDirty = true;
				posDirty = true;
			}
		}

		public void SetWidth(int dotIdx, float width)
		{
			if (validateDotIdx(dotIdx))
			{
				int num = dotIdx * 4;
				int num2 = num + 1;
				int num3 = num + 2;
				int num4 = num + 3;
				float num5 = 0.5f * width;
				mnorm[num].x = 0f - num5;
				mnorm[num].y = num5;
				mnorm[num2].x = 0f - num5;
				mnorm[num2].y = 0f - num5;
				mnorm[num3].x = num5;
				mnorm[num3].y = 0f - num5;
				mnorm[num4].x = num5;
				mnorm[num4].y = num5;
				normalsOrWidthDirty = true;
			}
		}

		public void SetSize(int dotIdx, Vector2 size)
		{
			if (validateDotIdx(dotIdx))
			{
				int num = dotIdx * 4;
				int num2 = num + 1;
				int num3 = num + 2;
				int num4 = num + 3;
				float num5 = 0.5f * size.x;
				float num6 = 0.5f * size.y;
				mnorm[num].x = 0f - num5;
				mnorm[num].y = num6;
				mnorm[num2].x = 0f - num5;
				mnorm[num2].y = 0f - num6;
				mnorm[num3].x = num5;
				mnorm[num3].y = 0f - num6;
				mnorm[num4].x = num5;
				mnorm[num4].y = num6;
				normalsOrWidthDirty = true;
			}
		}

		public void SetPixelOffset(int dotIdx, Vector2 pixelOffset)
		{
			if (validateDotIdx(dotIdx))
			{
				int num = dotIdx * 4;
				int num2 = num + 1;
				int num3 = num + 2;
				int num4 = num + 3;
				muvs2[num] = pixelOffset;
				muvs2[num2] = pixelOffset;
				muvs2[num3] = pixelOffset;
				muvs2[num4] = pixelOffset;
				uvs2Dirty = true;
			}
		}

		public void SetColor(int dotIdx, Color color)
		{
			if (validateDotIdx(dotIdx))
			{
				int num = dotIdx * 4;
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

		public Color GetColor(int dotIdx)
		{
			return mcolors[dotIdx * 4];
		}

		public void SetRectIndex(int dotIdx, int rectIndex)
		{
			if (validateDotIdx(dotIdx))
			{
				int num = dotIdx * 4;
				int num2 = num + 1;
				int num3 = num + 2;
				int num4 = num + 3;
				rectIDs[dotIdx] = rectIndex;
				DotsAtlas.Rect rect = atlas.rects[MathUtility.RoundedArrayIdx(rectIndex, atlas.rects.Length)];
				muvs0[num] = rect.v0;
				muvs0[num2] = rect.v1;
				muvs0[num3] = rect.v2;
				muvs0[num4] = rect.v3;
				uvsDirty = true;
			}
		}

		public void SetRectIndexUnchecked(int dotIdx, int rectIndex)
		{
			int num = dotIdx * 4;
			int num2 = num + 1;
			int num3 = num + 2;
			int num4 = num + 3;
			rectIDs[dotIdx] = rectIndex;
			DotsAtlas.Rect rect = atlas.rects[rectIndex % atlas.rects.Length];
			muvs0[num] = rect.v0;
			muvs0[num2] = rect.v1;
			muvs0[num3] = rect.v2;
			muvs0[num4] = rect.v3;
			uvsDirty = true;
		}

		protected override string opaqueShaderName()
		{
			if (base.widthMode == WidthMode.WorldspaceBillboard)
			{
				return "Hidden/Linefy/DotsWorldspaceBillboard";
			}
			if (base.widthMode == WidthMode.WorldspaceXY)
			{
				return "Hidden/Linefy/DotsWorldspaceXY";
			}
			if (_pixelPerfect)
			{
				return "Hidden/Linefy/DotsPixelPerfectBillboard";
			}
			return "Hidden/Linefy/DotsPixelBillboard";
		}

		protected override string transparentShaderName()
		{
			if (base.widthMode == WidthMode.WorldspaceBillboard)
			{
				return "Hidden/Linefy/DotsTransparentWorldspaceBillboard";
			}
			if (base.widthMode == WidthMode.WorldspaceXY)
			{
				return "Hidden/Linefy/DotsTransparentWorldspaceXY";
			}
			if (_pixelPerfect)
			{
				return "Hidden/Linefy/DotsTransparentPixelPerfectBillboard";
			}
			return "Hidden/Linefy/DotsTransparentPixelBillboard";
		}

		public void LoadSerializationData(SerializationData_Dots inputData)
		{
			if (inputData == null)
			{
				Debug.LogError("Dots.SetSerializableData (inputData)  data == null");
				return;
			}
			LoadSerializationData((SerializationData_PrimitivesGroup)inputData);
			atlas = inputData.atlas;
			base.texture = atlas.texture;
			pixelPerfect = inputData.pixelPerfect;
		}

		public void SaveSerializationData(SerializationData_Dots outputData)
		{
			if (outputData == null)
			{
				Debug.LogError("Dots.GetSerializableData (outputData)  data == null");
				return;
			}
			SaveSerializationData((SerializationData_PrimitivesGroup)outputData);
			outputData.atlas = atlas;
			outputData.pixelPerfect = pixelPerfect;
		}

		private bool validateDotIdx(int dotIdx)
		{
			return true;
		}

		protected override void OnAfterMaterialCreated()
		{
			base.OnAfterMaterialCreated();
		}

		public override void GetStatistic(ref int linesCount, ref int totallinesCount, ref int dotsCount, ref int totalDotsCount, ref int polylinesCount, ref int totalPolylineVerticesCount)
		{
			dotsCount++;
			totalDotsCount += count;
		}

		public int GetNearestXY(Vector2 point, ref float dist)
		{
			float num = float.MaxValue;
			int result = 0;
			for (int i = 0; i < _count; i++)
			{
				float num2 = Vector2.Distance(point, mpos[i * 4]);
				if (num2 < num)
				{
					result = i;
					dist = num2;
					num = dist;
				}
			}
			return result;
		}
	}
}
