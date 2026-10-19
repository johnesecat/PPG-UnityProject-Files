using Linefy.Serialization;
using UnityEngine;

namespace Linefy.Primitives
{
	public class Grid2d : Drawable
	{
		private float _width;

		private float _height;

		private int _widthSegments;

		private int _heightSegments;

		private bool _linePerSegment;

		private bool dTopology = true;

		private bool dSize = true;

		private Lines wireframe;

		public SerializationData_LinesBase wireframeProperties = new SerializationData_LinesBase();

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

		public float height
		{
			get
			{
				return _height;
			}
			set
			{
				if (_height != value)
				{
					_height = value;
					dSize = true;
				}
			}
		}

		public int widthSegments
		{
			get
			{
				return _widthSegments;
			}
			set
			{
				value = Mathf.Max(value, 1);
				if (_widthSegments != value)
				{
					_widthSegments = value;
					dTopology = true;
				}
			}
		}

		public int heightSegments
		{
			get
			{
				return _heightSegments;
			}
			set
			{
				value = Mathf.Max(value, 1);
				if (_heightSegments != value)
				{
					_heightSegments = value;
					dTopology = true;
				}
			}
		}

		public bool linePerSegment
		{
			get
			{
				return _linePerSegment;
			}
			set
			{
				if (_linePerSegment != value)
				{
					_linePerSegment = value;
					dTopology = true;
				}
			}
		}

		public Grid2d(float width, float height, int widthSegments, int heightSegments, bool linePerSegment, SerializationData_LinesBase wireframeProperties)
		{
			_width = width;
			_height = height;
			_widthSegments = widthSegments;
			_heightSegments = heightSegments;
			_linePerSegment = linePerSegment;
			this.wireframeProperties = wireframeProperties;
		}

		private void PreDraw()
		{
			if (wireframe == null)
			{
				wireframe = new Lines(1);
				wireframe.capacityChangeStep = 8;
			}
			if (dTopology)
			{
				int num = 0;
				if (_linePerSegment)
				{
					int num2 = widthSegments + 1;
					int num3 = heightSegments + 1;
					num = num2 * heightSegments + num3 * widthSegments;
				}
				else
				{
					int num4 = widthSegments + 1;
					int num5 = heightSegments + 1;
					num = num4 + num5;
				}
				wireframe.count = num;
				for (int i = 0; i < wireframe.count; i++)
				{
					wireframe.SetTextureOffset(i, 0f, 0f);
				}
				dTopology = false;
				dSize = true;
			}
			wireframe.autoTextureOffset = true;
			if (dSize)
			{
				float num6 = (0f - width) / 2f;
				float num7 = (0f - height) / 2f;
				float x = 0f - num6;
				float y = 0f - num7;
				float num8 = width / (float)widthSegments;
				float num9 = height / (float)heightSegments;
				int num10 = 0;
				if (linePerSegment)
				{
					for (int j = 0; j <= heightSegments; j++)
					{
						float y2 = num7 + num9 * (float)j;
						for (int k = 0; k < widthSegments; k++)
						{
							float num11 = num6 + num8 * (float)k;
							Vector3 positionA = new Vector3(num11, y2, 0f);
							Vector3 positionB = new Vector3(num11 + num8, y2, 0f);
							wireframe.SetPosition(num10, positionA, positionB);
							num10++;
						}
					}
					for (int l = 0; l <= widthSegments; l++)
					{
						float x2 = num6 + num8 * (float)l;
						for (int m = 0; m < heightSegments; m++)
						{
							float num12 = num7 + num9 * (float)m;
							Vector3 positionA2 = new Vector3(x2, num12, 0f);
							Vector3 positionB2 = new Vector3(x2, num12 + num9, 0f);
							wireframe.SetPosition(num10, positionA2, positionB2);
							num10++;
						}
					}
				}
				else
				{
					for (int n = 0; n <= heightSegments; n++)
					{
						float y3 = num7 + num9 * (float)n;
						Vector3 positionA3 = new Vector3(num6, y3, 0f);
						Vector3 positionB3 = new Vector3(x, y3, 0f);
						wireframe.SetPosition(num10, positionA3, positionB3);
						num10++;
					}
					for (int num13 = 0; num13 <= widthSegments; num13++)
					{
						float x3 = num6 + num8 * (float)num13;
						Vector3 positionA4 = new Vector3(x3, num7, 0f);
						Vector3 positionB4 = new Vector3(x3, y, 0f);
						wireframe.SetPosition(num10, positionA4, positionB4);
						num10++;
					}
				}
				dSize = false;
			}
			wireframe.LoadSerializationData(wireframeProperties);
		}

		public override void DrawNow(Matrix4x4 matrix)
		{
			PreDraw();
			wireframe.DrawNow(matrix);
		}

		public override void Draw(Matrix4x4 tm, Camera cam, int layer)
		{
			PreDraw();
			wireframe.Draw(tm, cam, layer);
		}

		public override void Dispose()
		{
			if (wireframe != null)
			{
				wireframe.Dispose();
			}
		}

		public override void GetStatistic(ref int linesCount, ref int totallinesCount, ref int dotsCount, ref int totalDotsCount, ref int polylinesCount, ref int totalPolylineVerticesCount)
		{
			if (wireframe != null)
			{
				linesCount++;
				totallinesCount += wireframe.count;
			}
		}
	}
}
