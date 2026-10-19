using Linefy.Serialization;
using UnityEngine;

namespace Linefy.Primitives
{
	public class Box : Drawable
	{
		private float _width = 1f;

		private float _height = 1f;

		private float _length = 1f;

		private int _widthSegments = 1;

		private int _heightSegments = 1;

		private int _lengthSegments = 1;

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

		public Box(float width, float height, float length, int widthSegments, int heightSegments, int lengthSegments, SerializationData_LinesBase wireframeProperties)
		{
			_width = width;
			_height = height;
			_length = length;
			_widthSegments = widthSegments;
			_heightSegments = heightSegments;
			_lengthSegments = lengthSegments;
			this.wireframeProperties = wireframeProperties;
		}

		public Box(float width, float height, float length, int widthSegments, int heightSegments, int lengthSegments)
		{
			_width = width;
			_height = height;
			_length = length;
			_widthSegments = widthSegments;
			_heightSegments = heightSegments;
			_lengthSegments = lengthSegments;
		}

		public Box(float size, Color color)
		{
			_width = size;
			_height = size;
			_length = size;
			wireframeProperties.colorMultiplier = color;
		}

		public Box()
		{
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
				int num = (_heightSegments + 1) * 4;
				num += (_widthSegments - 1) * 4;
				num += (_lengthSegments - 1) * 4;
				num += 4;
				wireframe.count = num;
				for (int i = 0; i < num; i++)
				{
					wireframe.SetTextureOffset(i, 0f, 0f);
				}
				dTopology = false;
				dSize = true;
			}
			if (dSize)
			{
				float num2 = (0f - width) / 2f;
				float num3 = (0f - length) / 2f;
				float num4 = (0f - height) / 2f;
				float x = 0f - num2;
				float y = 0f - num4;
				float z = 0f - num3;
				float num5 = width / (float)widthSegments;
				float num6 = length / (float)lengthSegments;
				float num7 = height / (float)heightSegments;
				int num8 = 0;
				for (int j = 0; j <= heightSegments; j++)
				{
					float y2 = num4 + (float)j * num7;
					Vector3 vector = new Vector3(num2, y2, num3);
					Vector3 vector2 = new Vector3(num2, y2, z);
					Vector3 vector3 = new Vector3(x, y2, z);
					Vector3 vector4 = new Vector3(x, y2, num3);
					wireframe.SetPosition(num8, vector, vector2);
					num8++;
					wireframe.SetPosition(num8, vector2, vector3);
					num8++;
					wireframe.SetPosition(num8, vector3, vector4);
					num8++;
					wireframe.SetPosition(num8, vector4, vector);
					num8++;
				}
				for (int k = 1; k < widthSegments; k++)
				{
					float x2 = num2 + (float)k * num5;
					Vector3 vector5 = new Vector3(x2, num4, num3);
					Vector3 vector6 = new Vector3(x2, num4, z);
					Vector3 vector7 = new Vector3(x2, y, z);
					Vector3 vector8 = new Vector3(x2, y, num3);
					wireframe.SetPosition(num8, vector5, vector6);
					num8++;
					wireframe.SetPosition(num8, vector6, vector7);
					num8++;
					wireframe.SetPosition(num8, vector7, vector8);
					num8++;
					wireframe.SetPosition(num8, vector8, vector5);
					num8++;
				}
				for (int l = 1; l < lengthSegments; l++)
				{
					float z2 = num3 + (float)l * num6;
					Vector3 vector9 = new Vector3(num2, num4, z2);
					Vector3 vector10 = new Vector3(num2, y, z2);
					Vector3 vector11 = new Vector3(x, y, z2);
					Vector3 vector12 = new Vector3(x, num4, z2);
					wireframe.SetPosition(num8, vector9, vector10);
					num8++;
					wireframe.SetPosition(num8, vector10, vector11);
					num8++;
					wireframe.SetPosition(num8, vector11, vector12);
					num8++;
					wireframe.SetPosition(num8, vector12, vector9);
					num8++;
				}
				wireframe.SetPosition(num8, new Vector3(num2, num4, num3), new Vector3(num2, y, num3));
				num8++;
				wireframe.SetPosition(num8, new Vector3(num2, num4, z), new Vector3(num2, y, z));
				num8++;
				wireframe.SetPosition(num8, new Vector3(x, num4, num3), new Vector3(x, y, num3));
				num8++;
				wireframe.SetPosition(num8, new Vector3(x, num4, z), new Vector3(x, y, z));
				num8++;
				dSize = false;
			}
			wireframe.autoTextureOffset = true;
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
