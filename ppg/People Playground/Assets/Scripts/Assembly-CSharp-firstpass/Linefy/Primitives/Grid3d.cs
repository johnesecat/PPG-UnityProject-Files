using Linefy.Serialization;
using UnityEngine;

namespace Linefy.Primitives
{
	public class Grid3d : Drawable
	{
		private float _width;

		private float _height;

		private float _length;

		private int _widthSegments;

		private int _heightSegments;

		private int _lengthSegments;

		private bool dTopology = true;

		private bool dSize = true;

		private Lines wireframe;

		public SerializationData_LinesBase wireframeProperties = new SerializationData_LinesBase(3f, Color.white, 1f);

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

		public float length
		{
			get
			{
				return _length;
			}
			set
			{
				if (_length != value)
				{
					_length = value;
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

		public int lengthSegments
		{
			get
			{
				return _lengthSegments;
			}
			set
			{
				value = Mathf.Max(value, 1);
				if (_lengthSegments != value)
				{
					_lengthSegments = value;
					dTopology = true;
				}
			}
		}

		public Grid3d(float width, float height, float length, int widthSegments, int heightSegments, int lengthSegments, SerializationData_LinesBase wireframeProperties)
		{
			_width = width;
			_height = height;
			_length = length;
			_widthSegments = widthSegments;
			_heightSegments = heightSegments;
			_lengthSegments = lengthSegments;
			this.wireframeProperties = wireframeProperties;
		}

		private void PreDraw()
		{
			if (wireframe == null)
			{
				wireframe = new Lines(1);
				wireframe.capacityChangeStep = 8;
			}
			wireframe.autoTextureOffset = true;
			if (dTopology)
			{
				int num = widthSegments + 1;
				int num2 = heightSegments + 1;
				int num3 = lengthSegments + 1;
				int num4 = num * num2;
				int num5 = num3 * num2;
				int num6 = num3 * num;
				int count = num4 + num5 + num6;
				wireframe.count = count;
				for (int i = 0; i < wireframe.count; i++)
				{
					wireframe.SetTextureOffset(i, 0f, 0f);
				}
				dTopology = false;
				dSize = true;
			}
			if (dSize)
			{
				float num7 = (0f - width) / 2f;
				float num8 = (0f - length) / 2f;
				float num9 = (0f - height) / 2f;
				float x = 0f - num7;
				float y = 0f - num9;
				float z = 0f - num8;
				float num10 = width / (float)widthSegments;
				float num11 = length / (float)lengthSegments;
				float num12 = height / (float)heightSegments;
				int num13 = 0;
				for (int j = 0; j <= heightSegments; j++)
				{
					for (int k = 0; k <= widthSegments; k++)
					{
						float x2 = num7 + num10 * (float)k;
						float y2 = num9 + num12 * (float)j;
						Vector3 positionA = new Vector3(x2, y2, num8);
						Vector3 positionB = new Vector3(x2, y2, z);
						wireframe.SetPosition(num13, positionA, positionB);
						num13++;
					}
				}
				for (int l = 0; l <= lengthSegments; l++)
				{
					for (int m = 0; m <= heightSegments; m++)
					{
						float z2 = num8 + num11 * (float)l;
						float y3 = num9 + num12 * (float)m;
						Vector3 positionA2 = new Vector3(num7, y3, z2);
						Vector3 positionB2 = new Vector3(x, y3, z2);
						wireframe.SetPosition(num13, positionA2, positionB2);
						num13++;
					}
				}
				for (int n = 0; n <= lengthSegments; n++)
				{
					for (int num14 = 0; num14 <= widthSegments; num14++)
					{
						float x3 = num7 + num10 * (float)num14;
						float z3 = num8 + num11 * (float)n;
						Vector3 positionA3 = new Vector3(x3, num9, z3);
						Vector3 positionB3 = new Vector3(x3, y, z3);
						wireframe.SetPosition(num13, positionA3, positionB3);
						num13++;
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
			wireframe.Dispose();
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
