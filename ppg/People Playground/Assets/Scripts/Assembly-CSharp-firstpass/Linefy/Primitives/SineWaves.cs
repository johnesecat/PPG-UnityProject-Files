using Linefy.Serialization;
using UnityEngine;

namespace Linefy.Primitives
{
	public class SineWaves : Drawable
	{
		private int _segmentsCount = 32;

		private int _itemsCount = 4;

		private float _width = 1f;

		private float _heightSpacing = 0.2f;

		private float _waveHeight = 0.04f;

		private float _waveLength = 0.3f;

		private float _waveOffset;

		private bool _centerPivot = true;

		private bool dTopology = true;

		private bool dSize = true;

		private Polyline polyline;

		public SerializationData_LinesBase wireframeProperties = new SerializationData_LinesBase(3f, Color.white, 1f);

		public int segmentsCount
		{
			get
			{
				return _segmentsCount;
			}
			set
			{
				if (_segmentsCount != value)
				{
					_segmentsCount = value;
					dTopology = true;
				}
			}
		}

		public int itemsCount
		{
			get
			{
				return _itemsCount;
			}
			set
			{
				if (_itemsCount != value)
				{
					_itemsCount = value;
					dTopology = true;
				}
			}
		}

		public float width
		{
			get
			{
				return _width;
			}
			set
			{
				if (_width != value)
				{
					_width = value;
					dSize = true;
				}
			}
		}

		public float heightSpacing
		{
			get
			{
				return _heightSpacing;
			}
			set
			{
				if (_heightSpacing != value)
				{
					_heightSpacing = value;
					dSize = true;
				}
			}
		}

		public float waveHeight
		{
			get
			{
				return _waveHeight;
			}
			set
			{
				if (_waveHeight != value)
				{
					_waveHeight = value;
					dSize = true;
				}
			}
		}

		public float waveLength
		{
			get
			{
				return _waveLength;
			}
			set
			{
				if (_waveLength != value)
				{
					_waveLength = value;
					dSize = true;
				}
			}
		}

		public float waveOffset
		{
			get
			{
				return _waveOffset;
			}
			set
			{
				if (_waveOffset != value)
				{
					_waveOffset = value;
					dSize = true;
				}
			}
		}

		public bool centerPivot
		{
			get
			{
				return _centerPivot;
			}
			set
			{
				if (_centerPivot != value)
				{
					_centerPivot = value;
					dSize = true;
				}
			}
		}

		private void PreDraw()
		{
			int count = (_segmentsCount + 1 + 2) * _itemsCount;
			if (polyline == null)
			{
				polyline = new Polyline(count);
			}
			if (dTopology)
			{
				polyline.count = count;
				int num = 0;
				for (int i = 0; i < _itemsCount; i++)
				{
					polyline.SetWidth(num, 0f);
					num++;
					float value = Random.value;
					for (int j = 0; j <= segmentsCount; j++)
					{
						polyline.SetWidth(num, 1f);
						polyline.SetTextureOffset(num, value + (float)j / (float)segmentsCount);
						num++;
					}
					polyline.SetWidth(num, 0f);
					num++;
				}
				dTopology = false;
				dSize = true;
			}
			if (dSize)
			{
				float num2 = 0f;
				float num3 = 0f;
				if (centerPivot)
				{
					num2 = (0f - _heightSpacing * (float)(_itemsCount - 1)) / 2f;
					num3 = (0f - _width) / 2f;
				}
				int num4 = 0;
				float num5 = _width / (float)_segmentsCount;
				for (int k = 0; k < _itemsCount; k++)
				{
					bool flag = false;
					bool flag2 = false;
					int num6 = 0;
					while (num6 <= _segmentsCount)
					{
						Vector3 position = new Vector3(num3 + (float)num6 * num5, num2 + _heightSpacing * (float)k, 0f);
						float num7 = Mathf.Sin((_waveOffset + position.x) / waveLength * 6.283185f) * _waveHeight;
						position.y += num7;
						polyline.SetPosition(num4, position);
						num4++;
						if (num6 == 0 && !flag)
						{
							flag = true;
						}
						else if (num6 == segmentsCount && !flag2)
						{
							flag2 = true;
						}
						else
						{
							num6++;
						}
					}
				}
				dSize = false;
			}
			polyline.LoadSerializationData(wireframeProperties);
		}

		public override void DrawNow(Matrix4x4 matrix)
		{
			PreDraw();
			polyline.DrawNow(matrix);
		}

		public override void Draw(Matrix4x4 tm, Camera cam, int layer)
		{
			PreDraw();
			polyline.Draw(tm, cam, layer);
		}

		public override void Dispose()
		{
			if (polyline != null)
			{
				polyline.Dispose();
			}
		}

		public override void GetStatistic(ref int linesCount, ref int totallinesCount, ref int dotsCount, ref int totalDotsCount, ref int polylinesCount, ref int totalPolylineVerticesCount)
		{
		}
	}
}
