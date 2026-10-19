using Linefy.Serialization;
using UnityEngine;

namespace Linefy.Primitives
{
	public class Cylinder : Drawable
	{
		private bool dSize = true;

		private bool dTopology = true;

		private float _radiusTop = 1f;

		private float _radiusBottom = 1f;

		private float _height = 1f;

		private float _pivotOffset = 0.5f;

		private int _radiusSegments = 32;

		private int _radialsCount = 8;

		private Texture _tex;

		private float _textureScale;

		private Lines radials;

		private Polyline topRadius;

		private Polyline bottomRadius;

		public SerializationData_LinesBase wireframeProperties = new SerializationData_LinesBase(3f, Color.white, 1f);

		public float radiusTop
		{
			get
			{
				return _radiusTop;
			}
			set
			{
				if (_radiusTop != value)
				{
					dSize = true;
					_radiusTop = value;
				}
			}
		}

		public float radiusBottom
		{
			get
			{
				return _radiusBottom;
			}
			set
			{
				if (_radiusBottom != value)
				{
					dSize = true;
					_radiusBottom = value;
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
					dSize = true;
					_height = value;
				}
			}
		}

		public float pivotOffset
		{
			get
			{
				return _pivotOffset;
			}
			set
			{
				if (_pivotOffset != value)
				{
					_pivotOffset = value;
					dSize = true;
				}
			}
		}

		public int radiusSegments
		{
			get
			{
				return _radiusSegments;
			}
			set
			{
				if (_radiusSegments != value)
				{
					dTopology = true;
					_radiusSegments = value;
				}
			}
		}

		public int radialsCount
		{
			get
			{
				return _radialsCount;
			}
			set
			{
				if (_radialsCount != value)
				{
					dTopology = true;
					_radialsCount = value;
				}
			}
		}

		public Cylinder()
		{
		}

		public Cylinder(float radiusTop, float radiusBottom, float height, int radiusSegments, int radialsCount, SerializationData_LinesBase wireframeProperties)
		{
			_radiusTop = radiusTop;
			_radiusBottom = radiusBottom;
			_height = height;
			_radiusSegments = radiusSegments;
			_radialsCount = radialsCount;
			this.wireframeProperties = wireframeProperties;
		}

		private void PreDraw()
		{
			if (radials == null)
			{
				bottomRadius = new Polyline(radiusSegments, isClosed: true);
				bottomRadius.capacityChangeStep = 16;
				topRadius = new Polyline(radiusSegments, isClosed: true);
				topRadius.capacityChangeStep = 16;
				radials = new Lines(radialsCount);
				radials.capacityChangeStep = 8;
				radials.autoTextureOffset = true;
			}
			if (dTopology)
			{
				radials.count = radialsCount;
				bottomRadius.count = radiusSegments;
				topRadius.count = radiusSegments;
				for (int i = 0; i < radials.count; i++)
				{
					radials.SetTextureOffset(i, 0f, 0f);
				}
				dTopology = false;
				dSize = true;
			}
			radials.LoadSerializationData(wireframeProperties);
			topRadius.LoadSerializationData(wireframeProperties);
			bottomRadius.LoadSerializationData(wireframeProperties);
			topRadius.textureScale = 1f;
			bottomRadius.textureScale = 1f;
			if (wireframeProperties.texture != _tex)
			{
				_tex = wireframeProperties.texture;
				dSize = true;
			}
			if (wireframeProperties.textureScale != _textureScale)
			{
				_textureScale = wireframeProperties.textureScale;
				dSize = true;
			}
			if (dSize)
			{
				float num = 6.283185f / (float)_radiusSegments;
				float num2 = (0f - _height) * _pivotOffset;
				float y = _height + num2;
				for (int j = 0; j < radiusSegments; j++)
				{
					float f = (float)j * num;
					float num3 = Mathf.Cos(f);
					float num4 = Mathf.Sin(f);
					topRadius.SetPosition(j, new Vector3(num3 * _radiusTop, y, num4 * _radiusTop));
					bottomRadius.SetPosition(j, new Vector3(num3 * _radiusBottom, num2, num4 * _radiusBottom));
				}
				num = 6.283185f / (float)_radialsCount;
				for (int k = 0; k < _radialsCount; k++)
				{
					float f2 = (float)k * num;
					float num5 = Mathf.Cos(f2);
					float num6 = Mathf.Sin(f2);
					Vector3 positionA = new Vector3(num5 * _radiusTop, y, num6 * _radiusTop);
					Vector3 positionB = new Vector3(num5 * _radiusBottom, num2, num6 * _radiusBottom);
					radials.SetPosition(k, positionA, positionB);
				}
				if (wireframeProperties.texture != null)
				{
					topRadius.RecalculateDistances(wireframeProperties.textureScale);
					bottomRadius.RecalculateDistances(wireframeProperties.textureScale);
				}
				dSize = false;
			}
		}

		public override void DrawNow(Matrix4x4 matrix)
		{
			PreDraw();
			radials.DrawNow(matrix);
			topRadius.DrawNow(matrix);
			bottomRadius.DrawNow(matrix);
		}

		public override void Draw(Matrix4x4 tm, Camera cam, int layer)
		{
			PreDraw();
			radials.Draw(tm, cam, layer);
			topRadius.Draw(tm, cam, layer);
			bottomRadius.Draw(tm, cam, layer);
		}

		public override void Dispose()
		{
			if (radials != null)
			{
				radials.Dispose();
			}
			if (bottomRadius != null)
			{
				bottomRadius.Dispose();
			}
			if (topRadius != null)
			{
				topRadius.Dispose();
			}
		}

		public override void GetStatistic(ref int linesCount, ref int totallinesCount, ref int dotsCount, ref int totalDotsCount, ref int polylinesCount, ref int totalPolylineVerticesCount)
		{
			if (radials != null)
			{
				totallinesCount += radials.count;
				totalPolylineVerticesCount += bottomRadius.count * 2;
			}
		}
	}
}
