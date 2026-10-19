using System;
using System.Collections.Generic;
using Linefy.Internal;
using Linefy.Serialization;
using UnityEngine;

namespace Linefy
{
	public class Polyline : LinesBase
	{
		private struct VertexTopology
		{
			public int t0;

			public int t1;

			public int t2;

			public int t3;

			public int t4;

			public int t5;

			public int t6;

			public int t7;

			public int t8;

			public int t9;

			public int n0;

			public int n1;

			public int n2;

			public int n3;

			public int n4;

			public int n5;

			public int n6;

			public int n7;

			public int n8;

			public int n9;

			public int p0;

			public int p1;

			public int p2;

			public int p3;

			public int p4;

			public int p5;

			public int p6;

			public int p7;

			public int p8;

			public int p9;

			public int a0;

			public int a1;

			public int a2;

			public int a3;

			public int a4;

			public int a5;

			public int a6;

			public int a7;

			public int a8;

			public int a9;

			public int[] tIndices => new int[10] { t0, t1, t2, t3, t4, t5, t6, t7, t8, t9 };

			public int[] nIndices => new int[10] { n0, n1, n2, n3, n4, n5, n6, n7, n8, n9 };

			public int[] pIndices => new int[10] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9 };

			public override string ToString()
			{
				return $"t[{aToString(tIndices)} ] p[{aToString(pIndices)} ] n[{aToString(nIndices)}]";
			}

			public string aToString(int[] arr)
			{
				string text = "t:";
				for (int i = 0; i < arr.Length; i++)
				{
					text += $"{arr[i]},";
				}
				return text;
			}
		}

		private bool _isClosed;

		private Vector3[] mpos;

		public Vector4[] mtan;

		private Vector3[] mnorm;

		private Color[] mcolors;

		private Vector2[] muvs0;

		private Vector2[] muvs1;

		private Vector2[] muvs2;

		private List<Vector4> muvs3 = new List<Vector4>();

		private int[] mtriangles;

		private float _lastVertexTextureOffset;

		private bool posDirty;

		private bool colorsDirty;

		private bool textureOffsetDirty;

		private bool widthDirty;

		private bool topologyIsDirty;

		private bool visualTopologyIsDirty;

		private VertexTopology[] vtopo;

		private const int vertsStride = 10;

		private const int trisStride = 24;

		private HashSet<int> dirtyTopoIndices = new HashSet<int>();

		public bool isClosed
		{
			get
			{
				return _isClosed;
			}
			set
			{
				if (value != _isClosed)
				{
					_isClosed = value;
					SetCount(_count);
				}
			}
		}

		public float lastVertexTextureOffset
		{
			get
			{
				return _lastVertexTextureOffset;
			}
			set
			{
				if (_lastVertexTextureOffset != value)
				{
					_lastVertexTextureOffset = value;
					if (count > 0)
					{
						SetTextureOffset(count, _lastVertexTextureOffset);
					}
				}
			}
		}

		public override int maxCount => 65000;

		public PolylineVertex this[int idx]
		{
			get
			{
				if (validateVertexIdx(idx))
				{
					int num = idx * 10;
					return new PolylineVertex(mpos[num], mcolors[num], muvs2[num].x, muvs0[num].x);
				}
				return default(PolylineVertex);
			}
			set
			{
				if (validateVertexIdx(idx))
				{
					Vector3 position = value.position;
					Vector4 vector = value.position;
					Color color = value.color;
					muvs2[vtopo[idx].p0].x = value.width;
					muvs2[vtopo[idx].p1].x = value.width;
					muvs2[vtopo[idx].p2].x = value.width;
					muvs2[vtopo[idx].p3].x = value.width;
					muvs2[vtopo[idx].p4].x = value.width;
					muvs2[vtopo[idx].p5].x = value.width;
					muvs2[vtopo[idx].p6].x = value.width;
					muvs2[vtopo[idx].p7].x = value.width;
					muvs2[vtopo[idx].p8].x = value.width;
					muvs2[vtopo[idx].p9].x = value.width;
					muvs2[vtopo[idx].n1].y = value.width;
					muvs2[vtopo[idx].n2].y = value.width;
					muvs2[vtopo[idx].n3].y = value.width;
					muvs2[vtopo[idx].n4].y = value.width;
					muvs2[vtopo[idx].n6].y = value.width;
					muvs2[vtopo[idx].n7].y = value.width;
					muvs2[vtopo[idx].n8].y = value.width;
					muvs2[vtopo[idx].n9].y = value.width;
					mtan[vtopo[idx].t1] = vector;
					mtan[vtopo[idx].t2] = vector;
					mtan[vtopo[idx].t3] = vector;
					mtan[vtopo[idx].t4] = vector;
					mtan[vtopo[idx].t6] = vector;
					mtan[vtopo[idx].t7] = vector;
					mtan[vtopo[idx].t8] = vector;
					mtan[vtopo[idx].t9] = vector;
					mpos[vtopo[idx].p0] = position;
					mpos[vtopo[idx].p1] = position;
					mpos[vtopo[idx].p2] = position;
					mpos[vtopo[idx].p3] = position;
					mpos[vtopo[idx].p4] = position;
					mpos[vtopo[idx].p5] = position;
					mpos[vtopo[idx].p6] = position;
					mpos[vtopo[idx].p7] = position;
					mpos[vtopo[idx].p8] = position;
					mpos[vtopo[idx].p9] = position;
					mnorm[vtopo[idx].n1] = position;
					mnorm[vtopo[idx].n2] = position;
					mnorm[vtopo[idx].n3] = position;
					mnorm[vtopo[idx].n4] = position;
					mnorm[vtopo[idx].n6] = position;
					mnorm[vtopo[idx].n7] = position;
					mnorm[vtopo[idx].n8] = position;
					mnorm[vtopo[idx].n9] = position;
					mcolors[vtopo[idx].p0] = color;
					mcolors[vtopo[idx].p1] = color;
					mcolors[vtopo[idx].p2] = color;
					mcolors[vtopo[idx].p3] = color;
					mcolors[vtopo[idx].p4] = color;
					mcolors[vtopo[idx].p5] = color;
					mcolors[vtopo[idx].p6] = color;
					mcolors[vtopo[idx].p7] = color;
					mcolors[vtopo[idx].p8] = color;
					mcolors[vtopo[idx].p9] = color;
					Vector4 value2 = color;
					muvs3[vtopo[idx].a0] = value2;
					muvs3[vtopo[idx].a1] = value2;
					muvs3[vtopo[idx].a2] = value2;
					muvs3[vtopo[idx].a3] = value2;
					muvs3[vtopo[idx].a4] = value2;
					muvs3[vtopo[idx].a5] = value2;
					muvs3[vtopo[idx].a6] = value2;
					muvs3[vtopo[idx].a7] = value2;
					muvs3[vtopo[idx].a8] = value2;
					muvs3[vtopo[idx].a9] = value2;
					muvs0[vtopo[idx].p0].x = value.textureOffset;
					muvs0[vtopo[idx].p1].x = value.textureOffset;
					muvs0[vtopo[idx].p2].x = value.textureOffset;
					muvs0[vtopo[idx].p3].x = value.textureOffset;
					muvs0[vtopo[idx].p4].x = value.textureOffset;
					muvs0[vtopo[idx].p5].x = value.textureOffset;
					muvs0[vtopo[idx].p6].x = value.textureOffset;
					muvs0[vtopo[idx].p7].x = value.textureOffset;
					muvs0[vtopo[idx].p8].x = value.textureOffset;
					muvs0[vtopo[idx].p9].x = value.textureOffset;
					muvs0[vtopo[idx].a1].y = value.textureOffset;
					muvs0[vtopo[idx].a3].y = value.textureOffset;
					muvs0[vtopo[idx].a6].y = value.textureOffset;
					muvs0[vtopo[idx].a8].y = value.textureOffset;
					posDirty = true;
					colorsDirty = true;
					textureOffsetDirty = true;
					widthDirty = true;
				}
			}
		}

		protected override void SetDirtyAttributes()
		{
			boundsDirty = true;
			posDirty = true;
			colorsDirty = true;
			textureOffsetDirty = true;
			widthDirty = true;
			topologyIsDirty = true;
			visualTopologyIsDirty = true;
		}

		public Polyline(int count)
		{
			InternalConstructor("New polyline", count, transparent: false, 0f, isClosed: false, null, 1f, 4);
		}

		public Polyline(int count, bool isClosed)
		{
			InternalConstructor("New polyline", count, transparent: false, 0f, isClosed, null, 1f, 4);
		}

		[Obsolete("SetVisualPropertyBlock is Obsolete , use LoadSerializationData instead")]
		public Polyline(int count, VisualPropertiesBlock propertiesBlock)
		{
			InternalConstructor("New polyline", count, transparent: false, 0f, isClosed: false, null, 1f, 4);
			SetVisualPropertyBlock(propertiesBlock);
		}

		public Polyline(int count, bool transparent, float feather, bool isClosed)
		{
			InternalConstructor("New polyline", count, transparent, feather, isClosed, null, 1f, 4);
		}

		public Polyline(int count, int capacityChangeStep)
		{
			InternalConstructor("New polyline", count, transparent: false, 0f, isClosed: false, null, 1f, capacityChangeStep);
		}

		public Polyline(int count, bool transparent, float feather, bool isClosed, Color colorMult, float widthMult)
		{
			InternalConstructor("", count, transparent, feather, isClosed, null, 1f, 1);
			base.colorMultiplier = colorMult;
			base.widthMultiplier = widthMult;
		}

		public Polyline(string name, int count, bool transparent, float feather, bool isClosed, Texture2D texture, float textureScale, int capacityChangeStep)
		{
			InternalConstructor(name, count, transparent, feather, isClosed, texture, textureScale, capacityChangeStep);
		}

		public Polyline(SerializationData_Polyline data)
		{
			LoadSerializationData(data);
		}

		private void InternalConstructor(string name, int count, bool transparent, float feather, bool isClosed, Texture2D texture, float textureScale, int capacityChangeStep)
		{
			base.name = name;
			base.transparent = transparent;
			base.capacityChangeStep = capacityChangeStep;
			this.count = count;
			for (int i = 0; i < this.count; i++)
			{
				this[i] = new PolylineVertex(Vector3.zero, Color.white, 1f, i);
			}
			base.texture = texture;
			base.textureScale = textureScale;
			base.feather = feather;
			this.isClosed = isClosed;
		}

		protected override void SetCapacity(int prevCapacity)
		{
			Array.Resize(ref vtopo, capacity);
			int num = capacity * 10;
			Array.Resize(ref mpos, num);
			Array.Resize(ref mnorm, num);
			Array.Resize(ref mtan, num);
			Array.Resize(ref mcolors, num);
			Array.Resize(ref muvs0, num);
			Array.Resize(ref muvs1, num);
			Array.Resize(ref muvs2, num);
			muvs3.Resize(num);
			int newSize = capacity * 24;
			Array.Resize(ref mtriangles, newSize);
			for (int i = prevCapacity; i < capacity; i++)
			{
				int num2 = i * 10;
				int num3 = num2 + 1;
				int num4 = num2 + 2;
				int num5 = num2 + 3;
				int num6 = num2 + 4;
				int num7 = num2 + 5;
				int num8 = num2 + 6;
				int num9 = num2 + 7;
				int num10 = num2 + 8;
				int num11 = num2 + 9;
				int num12 = i * 24;
				muvs1[num2].x = -1f;
				muvs1[num3].x = -2f;
				muvs1[num4].x = -3f;
				muvs1[num5].x = -4f;
				muvs1[num6].x = -5f;
				muvs1[num7].x = 1f;
				muvs1[num8].x = 2f;
				muvs1[num9].x = 3f;
				muvs1[num10].x = 4f;
				muvs1[num11].x = 5f;
				muvs1[num2].y = 2f;
				muvs1[num3].y = 2f;
				muvs1[num4].y = 2f;
				muvs1[num5].y = 2f;
				muvs1[num6].y = 2f;
				muvs1[num7].y = 2f;
				muvs1[num8].y = 2f;
				muvs1[num9].y = 2f;
				muvs1[num10].y = 2f;
				muvs1[num11].y = 2f;
				mtriangles[num12] = num2;
				mtriangles[num12 + 1] = num4;
				mtriangles[num12 + 2] = num3;
				mtriangles[num12 + 3] = num2;
				mtriangles[num12 + 4] = num3;
				mtriangles[num12 + 5] = num8;
				mtriangles[num12 + 6] = num2;
				mtriangles[num12 + 7] = num8;
				mtriangles[num12 + 8] = num7;
				mtriangles[num12 + 9] = num7;
				mtriangles[num12 + 10] = num8;
				mtriangles[num12 + 11] = num9;
				mtriangles[num12 + 12] = num5;
				mtriangles[num12 + 13] = num6;
				mtriangles[num12 + 14] = num2;
				mtriangles[num12 + 15] = num5;
				mtriangles[num12 + 16] = num2;
				mtriangles[num12 + 17] = num7;
				mtriangles[num12 + 18] = num5;
				mtriangles[num12 + 19] = num7;
				mtriangles[num12 + 20] = num10;
				mtriangles[num12 + 21] = num10;
				mtriangles[num12 + 22] = num7;
				mtriangles[num12 + 23] = num11;
			}
			SetDirtyAttributes();
		}

		private void setPointType(int vertIdx, int side, int typeId)
		{
			int num = vertIdx * 10;
			if (side == 1)
			{
				num += 5;
			}
			muvs1[num].y = typeId;
			muvs1[num + 1].y = typeId;
			muvs1[num + 2].y = typeId;
			muvs1[num + 3].y = typeId;
			muvs1[num + 4].y = typeId;
		}

		private void setPointType(int vertIdx, int typeId)
		{
			int num = vertIdx * 10;
			muvs1[num].y = typeId;
			muvs1[num + 1].y = typeId;
			muvs1[num + 2].y = typeId;
			muvs1[num + 3].y = typeId;
			muvs1[num + 4].y = typeId;
			muvs1[num + 5].y = typeId;
			muvs1[num + 6].y = typeId;
			muvs1[num + 7].y = typeId;
			muvs1[num + 8].y = typeId;
			muvs1[num + 9].y = typeId;
		}

		protected override void SetCount(int _prevCount)
		{
			FillDirtyTopoIndices(_prevCount);
			foreach (int dirtyTopoIndex in dirtyTopoIndices)
			{
				SetVTopo(dirtyTopoIndex);
				setPointType(dirtyTopoIndex, (dirtyTopoIndex >= _count) ? 2 : 0);
			}
			if (_count > 0 && !_isClosed)
			{
				setPointType(0, 0, 1);
				setPointType(Mathf.Max(0, _count - 1), 2);
				setPointType(Mathf.Max(0, _count - 2), 1, 1);
			}
			if (_prevCount < _count)
			{
				if (_prevCount > 0)
				{
					PolylineVertex value = this[_prevCount - 1];
					for (int i = _prevCount; i < _count; i++)
					{
						this[i] = value;
					}
					this[0] = this[0];
				}
				else
				{
					for (int j = Mathf.Max(0, _prevCount); j < _count; j++)
					{
						this[j] = new PolylineVertex(Vector3.zero, Color.white, 1f);
					}
				}
			}
			foreach (int dirtyTopoIndex2 in dirtyTopoIndices)
			{
				if (dirtyTopoIndex2 < _count)
				{
					this[dirtyTopoIndex2] = this[dirtyTopoIndex2];
				}
			}
			float num = _lastVertexTextureOffset;
			_lastVertexTextureOffset -= 1f;
			lastVertexTextureOffset = num;
			visualTopologyIsDirty = true;
		}

		private void FillDirtyTopoIndices(int _pCount)
		{
			dirtyTopoIndices.Clear();
			if (_count > 0)
			{
				dirtyTopoIndices.Add(0);
			}
			if (_count > 1)
			{
				dirtyTopoIndices.Add(1);
			}
			int num = Mathf.Min(_pCount, _count);
			int num2 = Mathf.Max(_pCount, _count);
			int num3 = Mathf.Clamp(num - 3, 0, capacity);
			int num4 = Mathf.Clamp(num2 + 2, 0, capacity);
			for (int i = num3; i < num4; i++)
			{
				dirtyTopoIndices.Add(i);
			}
		}

		private void SetVTopo(int vertIdx)
		{
			VertexTopology vertexTopology = vtopo[vertIdx];
			getIndices(vertIdx, 1, ref vertexTopology.t0, ref vertexTopology.t1, ref vertexTopology.t2, ref vertexTopology.t3, ref vertexTopology.t4);
			getIndices(vertIdx + 1, 0, ref vertexTopology.t5, ref vertexTopology.t6, ref vertexTopology.t7, ref vertexTopology.t8, ref vertexTopology.t9);
			getIndices(vertIdx - 1, 1, ref vertexTopology.p0, ref vertexTopology.p1, ref vertexTopology.p2, ref vertexTopology.p3, ref vertexTopology.p4);
			getIndices(vertIdx, 0, ref vertexTopology.p5, ref vertexTopology.p6, ref vertexTopology.p7, ref vertexTopology.p8, ref vertexTopology.p9);
			getIndices(vertIdx - 2, 1, ref vertexTopology.n0, ref vertexTopology.n1, ref vertexTopology.n2, ref vertexTopology.n3, ref vertexTopology.n4);
			getIndices(vertIdx - 1, 0, ref vertexTopology.n5, ref vertexTopology.n6, ref vertexTopology.n7, ref vertexTopology.n8, ref vertexTopology.n9);
			getIndices(vertIdx - 1, 0, ref vertexTopology.a0, ref vertexTopology.a1, ref vertexTopology.a2, ref vertexTopology.a3, ref vertexTopology.a4);
			getIndices(vertIdx, 1, ref vertexTopology.a5, ref vertexTopology.a6, ref vertexTopology.a7, ref vertexTopology.a8, ref vertexTopology.a9);
			vtopo[vertIdx] = vertexTopology;
		}

		private int roundedVertexIdx(int idx)
		{
			if (_count == 0)
			{
				return 0;
			}
			idx %= _count;
			if (idx < 0)
			{
				idx = (_count + idx) % _count;
			}
			return idx;
		}

		private void getIndices(int pointIdx, int side, ref int i0, ref int i1, ref int i2, ref int i3, ref int i4)
		{
			pointIdx = roundedVertexIdx(pointIdx);
			int num = pointIdx * 10;
			if (side == 1)
			{
				num += 5;
			}
			i0 = num;
			i1 = num + 1;
			i2 = num + 2;
			i3 = num + 3;
			i4 = num + 4;
		}

		public void SetWidth(int idx, float width)
		{
			if (idx == count)
			{
				if (isClosed)
				{
					int num = count * 10;
					muvs2[num - 1].x = width;
					muvs2[num - 2].x = width;
					muvs2[num - 3].x = width;
					muvs2[num - 4].x = width;
					muvs2[num - 5].x = width;
					muvs2[num - 6].y = width;
					muvs2[num - 7].y = width;
					muvs2[num - 8].y = width;
					muvs2[num - 9].y = width;
					muvs2[num - 10].y = width;
					textureOffsetDirty = true;
				}
			}
			else if (validateVertexIdx(idx))
			{
				muvs2[vtopo[idx].p1].x = width;
				muvs2[vtopo[idx].p2].x = width;
				muvs2[vtopo[idx].p3].x = width;
				muvs2[vtopo[idx].p4].x = width;
				muvs2[vtopo[idx].p6].x = width;
				muvs2[vtopo[idx].p7].x = width;
				muvs2[vtopo[idx].p8].x = width;
				muvs2[vtopo[idx].p9].x = width;
				muvs2[vtopo[idx].n1].y = width;
				muvs2[vtopo[idx].n2].y = width;
				muvs2[vtopo[idx].n3].y = width;
				muvs2[vtopo[idx].n4].y = width;
				muvs2[vtopo[idx].n6].y = width;
				muvs2[vtopo[idx].n7].y = width;
				muvs2[vtopo[idx].n8].y = width;
				muvs2[vtopo[idx].n9].y = width;
				textureOffsetDirty = true;
			}
		}

		public void SetTextureOffset(int idx, float textureOffset)
		{
			if (idx == count && _count > 0)
			{
				if (isClosed)
				{
					int num = count * 10;
					muvs0[num - 1].x = textureOffset;
					muvs0[num - 2].x = textureOffset;
					muvs0[num - 3].x = textureOffset;
					muvs0[num - 4].x = textureOffset;
					muvs0[num - 5].x = textureOffset;
					muvs0[num - 6].y = textureOffset;
					muvs0[num - 7].y = textureOffset;
					muvs0[num - 8].y = textureOffset;
					muvs0[num - 9].y = textureOffset;
					muvs0[num - 10].y = textureOffset;
					textureOffsetDirty = true;
				}
			}
			else if (validateVertexIdx(idx))
			{
				muvs0[vtopo[idx].p0].x = textureOffset;
				muvs0[vtopo[idx].p1].x = textureOffset;
				muvs0[vtopo[idx].p2].x = textureOffset;
				muvs0[vtopo[idx].p3].x = textureOffset;
				muvs0[vtopo[idx].p4].x = textureOffset;
				muvs0[vtopo[idx].p5].x = textureOffset;
				muvs0[vtopo[idx].p6].x = textureOffset;
				muvs0[vtopo[idx].p7].x = textureOffset;
				muvs0[vtopo[idx].p8].x = textureOffset;
				muvs0[vtopo[idx].p9].x = textureOffset;
				muvs0[vtopo[idx].a1].y = textureOffset;
				muvs0[vtopo[idx].a3].y = textureOffset;
				muvs0[vtopo[idx].a6].y = textureOffset;
				muvs0[vtopo[idx].a8].y = textureOffset;
				textureOffsetDirty = true;
			}
		}

		public void SetPosition(int idx, Vector3 position)
		{
			if (validateVertexIdx(idx))
			{
				Vector4 vector = position;
				mtan[vtopo[idx].t1] = vector;
				mtan[vtopo[idx].t2] = vector;
				mtan[vtopo[idx].t3] = vector;
				mtan[vtopo[idx].t4] = vector;
				mtan[vtopo[idx].t6] = vector;
				mtan[vtopo[idx].t7] = vector;
				mtan[vtopo[idx].t8] = vector;
				mtan[vtopo[idx].t9] = vector;
				mnorm[vtopo[idx].n1] = position;
				mnorm[vtopo[idx].n2] = position;
				mnorm[vtopo[idx].n3] = position;
				mnorm[vtopo[idx].n4] = position;
				mnorm[vtopo[idx].n6] = position;
				mnorm[vtopo[idx].n7] = position;
				mnorm[vtopo[idx].n8] = position;
				mnorm[vtopo[idx].n9] = position;
				mpos[vtopo[idx].p0] = position;
				mpos[vtopo[idx].p1] = position;
				mpos[vtopo[idx].p2] = position;
				mpos[vtopo[idx].p3] = position;
				mpos[vtopo[idx].p4] = position;
				mpos[vtopo[idx].p5] = position;
				mpos[vtopo[idx].p6] = position;
				mpos[vtopo[idx].p7] = position;
				mpos[vtopo[idx].p8] = position;
				mpos[vtopo[idx].p9] = position;
				posDirty = true;
			}
		}

		public void SetColor(int idx, Color color)
		{
			if (validateVertexIdx(idx))
			{
				mcolors[vtopo[idx].p0] = color;
				mcolors[vtopo[idx].p1] = color;
				mcolors[vtopo[idx].p2] = color;
				mcolors[vtopo[idx].p3] = color;
				mcolors[vtopo[idx].p4] = color;
				mcolors[vtopo[idx].p5] = color;
				mcolors[vtopo[idx].p6] = color;
				mcolors[vtopo[idx].p7] = color;
				mcolors[vtopo[idx].p8] = color;
				mcolors[vtopo[idx].p9] = color;
				colorsDirty = true;
			}
		}

		public void SetAlpha(int idx, float alpha)
		{
			if (validateVertexIdx(idx))
			{
				mcolors[vtopo[idx].p0].a = alpha;
				mcolors[vtopo[idx].p1].a = alpha;
				mcolors[vtopo[idx].p2].a = alpha;
				mcolors[vtopo[idx].p3].a = alpha;
				mcolors[vtopo[idx].p4].a = alpha;
				mcolors[vtopo[idx].p5].a = alpha;
				mcolors[vtopo[idx].p6].a = alpha;
				mcolors[vtopo[idx].p7].a = alpha;
				mcolors[vtopo[idx].p8].a = alpha;
				mcolors[vtopo[idx].p9].a = alpha;
				colorsDirty = true;
			}
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
				mesh.tangents = mtan;
				posDirty = false;
				boundsDirty = true;
				if (autoTextureOffset)
				{
					RecalculateTextureOffsets();
				}
			}
			if (colorsDirty)
			{
				mesh.colors = mcolors;
				mesh.SetUVs(3, muvs3);
				colorsDirty = false;
			}
			if (textureOffsetDirty)
			{
				mesh.uv = muvs0;
				textureOffsetDirty = false;
			}
			if (widthDirty)
			{
				mesh.uv3 = muvs2;
				widthDirty = false;
			}
			if (topologyIsDirty)
			{
				mesh.uv2 = muvs1;
				mesh.triangles = mtriangles;
				topologyIsDirty = false;
				visualTopologyIsDirty = false;
			}
			if (visualTopologyIsDirty)
			{
				mesh.uv2 = muvs1;
				visualTopologyIsDirty = false;
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

		public void AddVertex(PolylineVertex pv)
		{
			int idx = count;
			count++;
			this[idx] = pv;
		}

		public void AddWithDistance(PolylineVertex vertex)
		{
			int num = count;
			count++;
			if (num > 0)
			{
				int idx = num - 1;
				PolylineVertex polylineVertex = this[idx];
				float num2 = Vector3.Distance(polylineVertex.position, vertex.position);
				vertex.textureOffset = polylineVertex.textureOffset + num2;
			}
			this[num] = vertex;
		}

		private bool validateVertexIdx(int vertexIdx)
		{
			return true;
		}

		public void RecalculateDistances(float distanceMultiplier)
		{
			float num = 0f;
			Vector3 a = mpos[0];
			for (int i = 0; i < count; i++)
			{
				int num2 = i * 10;
				Vector3 vector = mpos[num2];
				num += Vector3.Distance(a, vector) * distanceMultiplier;
				SetTextureOffset(i, num);
				a = vector;
			}
			if (_isClosed)
			{
				Vector3 b = mpos[0];
				num += Vector3.Distance(a, b) * distanceMultiplier;
				SetTextureOffset(count, num);
			}
			textureOffsetDirty = true;
		}

		public void RecalculateTextureOffsets()
		{
			float num = 0f;
			Vector3 a = mpos[0];
			for (int i = 0; i < count; i++)
			{
				int num2 = i * 10;
				Vector3 vector = mpos[num2];
				num += Vector3.Distance(a, vector);
				SetTextureOffset(i, num);
				a = vector;
			}
			if (_isClosed)
			{
				Vector3 b = mpos[0];
				num += Vector3.Distance(a, b);
				SetTextureOffset(count, num);
			}
			textureOffsetDirty = true;
		}

		protected override void OnAutoTextureOffsetChanged()
		{
			posDirty = true;
		}

		public Color GetColor(int vertexIndex)
		{
			if (validateVertexIdx(vertexIndex))
			{
				int num = vertexIndex * 10;
				return mcolors[num];
			}
			return Color.magenta;
		}

		public float GetDistance(int vertexIndex)
		{
			if (validateVertexIdx(vertexIndex))
			{
				int num = vertexIndex * 10;
				return muvs0[num].x;
			}
			return 0f;
		}

		public Vector3 GetPosition(int vertexIndex)
		{
			if (validateVertexIdx(vertexIndex))
			{
				int num = vertexIndex * 10;
				return mpos[num];
			}
			return Vector3.zero;
		}

		protected override string opaqueShaderName()
		{
			if (base.widthMode == WidthMode.WorldspaceBillboard)
			{
				return "Hidden/Linefy/PolylineWorldspaceBillboard";
			}
			if (base.widthMode == WidthMode.WorldspaceXY)
			{
				return "Hidden/Linefy/PolylineWorldspaceXY";
			}
			return "Hidden/Linefy/PolylinePixelBillboard";
		}

		protected override string transparentShaderName()
		{
			if (base.widthMode == WidthMode.WorldspaceBillboard)
			{
				return "Hidden/Linefy/PolylineTransparentWorldspaceBillboard";
			}
			if (base.widthMode == WidthMode.WorldspaceXY)
			{
				return "Hidden/Linefy/PolylineTransparentWorldspaceXY";
			}
			return "Hidden/Linefy/PolylineTransparentPixelBillboard";
		}

		public void LoadSerializationData(SerializationData_Polyline inputData)
		{
			if (inputData == null)
			{
				Debug.LogError("Polyline.SetSerializableData (inputData)  inputData == null )");
				return;
			}
			LoadSerializationData((SerializationData_LinesBase)inputData);
			base.textureOffset = inputData.textureOffset;
			isClosed = inputData.isClosed;
			_lastVertexTextureOffset -= 0.1f;
			lastVertexTextureOffset = inputData.lastVertexTextureOffset;
		}

		public void SaveSerializationData(ref SerializationData_Polyline outputData)
		{
			if (outputData == null)
			{
				Debug.LogError("Polyline.GetSerializableData (outputData)  outputData == null");
				return;
			}
			SaveSerializationData(outputData);
			outputData.isClosed = isClosed;
			outputData.textureOffset = base.textureOffset;
			outputData.lastVertexTextureOffset = _lastVertexTextureOffset;
		}

		protected override void OnAfterMaterialCreated()
		{
			base.OnAfterMaterialCreated();
		}

		public void DrawInstanced(Matrix4x4[] matrices)
		{
			PreDraw();
			base.material.enableInstancing = true;
			Graphics.DrawMeshInstanced(mesh, 0, base.material, matrices);
		}

		public override void GetStatistic(ref int linesCount, ref int totallinesCount, ref int dotsCount, ref int totalDotsCount, ref int polylinesCount, ref int totalPolylineVerticesCount)
		{
			polylinesCount++;
			totalPolylineVerticesCount += count;
		}

		private Vector3 getPosition(int idx)
		{
			int num = idx * 10;
			if (idx >= _count)
			{
				num = (idx - 1) * 10 + 5;
			}
			return mpos[num];
		}

		public override float GetDistanceXY(Vector2 point, ref int segmentIdx, ref float segmentPersentage)
		{
			float num = float.MaxValue;
			int num2 = ((!_isClosed) ? 1 : 0);
			for (int i = 0; i < count - num2; i++)
			{
				float lv = 0f;
				Vector2 a = getPosition(i);
				Vector2 b = getPosition(i + 1);
				float distance = Edge2D.GetDistance(a, b, point, ref lv);
				if (distance < num)
				{
					segmentIdx = i;
					segmentPersentage = lv;
					num = distance;
				}
			}
			return num;
		}

		[Obsolete("TransparentPropertyBlock is Obsolete , use Linefy.Serialization.SerializationData_Polyline and Linefy.Serialization.SerializationDataFull_Polyline instead")]
		public Polyline(TransparentPropertyBlock t)
		{
		}

		[Obsolete("PolylineSerializableData is Obsolete , use Linefy.Serialization.SerializationData_Polyline and Linefy.Serialization.SerializationDataFull_Polyline instead")]
		public Polyline(PolylineSerializableData t)
		{
		}

		[Obsolete("TransparentPropertyBlock is Obsolete , use Linefy.Serialization.SerializationData_Polyline and Linefy.Serialization.SerializationDataFull_Polyline instead")]
		public void SetTransparentProperty(TransparentPropertyBlock t)
		{
		}

		[Obsolete("PolylineSerializableData is Obsolete , use Linefy.Serialization.SerializationData_Polyline and Linefy.Serialization.SerializationDataFull_Polyline instead")]
		public PolylineSerializableData GetSerializableData()
		{
			return null;
		}
	}
}
