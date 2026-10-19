using Linefy.Serialization;
using UnityEngine;

namespace Linefy.Primitives
{
	public class Cone : Drawable
	{
		private bool dSize = true;

		private bool dTopology = true;

		private float _radius = 1f;

		private float _height = 1f;

		private float _pivotOffset = 0.5f;

		private int _radiusSegments = 32;

		private int _radialsCount = 8;

		private Lines wireframe;

		private Polyline baseRadius;

		public SerializationData_LinesBase wireframeProperties = new SerializationData_LinesBase(3f, Color.white, 1f);

		private Texture _tex;

		private float _textureScale;

		public float radius
		{
			get
			{
				return _radius;
			}
			set
			{
				if (_radius != value)
				{
					dSize = true;
					_radius = value;
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

		public Cone()
		{
		}

		public Cone(float radius, float height, int radiusSegments, int radials)
		{
			_radius = radius;
			_height = height;
			_radiusSegments = radiusSegments;
			_radialsCount = radials;
		}

		public Cone(float radius, float height, int radiusSegments, int radials, SerializationData_LinesBase wireframeProperties)
		{
			_radius = radius;
			_height = height;
			_radiusSegments = radiusSegments;
			_radialsCount = radials;
			this.wireframeProperties = wireframeProperties;
		}

		private void PreDraw()
		{
			if (wireframe == null)
			{
				wireframe = new Lines(1);
				wireframe.capacityChangeStep = 16;
				baseRadius = new Polyline(radiusSegments, isClosed: true);
				baseRadius.capacityChangeStep = 16;
			}
			baseRadius.LoadSerializationData(wireframeProperties);
			baseRadius.textureScale = 1f;
			wireframe.LoadSerializationData(wireframeProperties);
			wireframe.autoTextureOffset = true;
			if (dTopology)
			{
				wireframe.count = radialsCount;
				for (int i = 0; i < radialsCount; i++)
				{
					wireframe.SetTextureOffset(i, 0f, 0f);
				}
				baseRadius.count = radiusSegments;
				dTopology = false;
				dSize = true;
			}
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
				float num = 6.283185f / (float)radiusSegments;
				float num2 = (0f - _height) * _pivotOffset;
				float y = _height + num2;
				for (int j = 0; j < radiusSegments; j++)
				{
					float f = (float)j * num;
					float x = Mathf.Cos(f) * radius;
					float z = Mathf.Sin(f) * radius;
					baseRadius.SetPosition(j, new Vector3(x, num2, z));
				}
				num = 6.283185f / (float)radialsCount;
				Vector3 positionB = new Vector3(0f, y, 0f);
				for (int k = 0; k < radialsCount; k++)
				{
					float f2 = (float)k * num;
					float x2 = Mathf.Cos(f2) * radius;
					float z2 = Mathf.Sin(f2) * radius;
					wireframe.SetPosition(k, new Vector3(x2, num2, z2), positionB);
				}
				if (wireframeProperties.texture != null)
				{
					baseRadius.RecalculateDistances(wireframeProperties.textureScale);
				}
				dSize = false;
			}
		}

		public override void DrawNow(Matrix4x4 matrix)
		{
			PreDraw();
			wireframe.DrawNow(matrix);
			baseRadius.DrawNow(matrix);
		}

		public override void Draw(Matrix4x4 tm, Camera cam, int layer)
		{
			PreDraw();
			wireframe.Draw(tm, cam, layer);
			baseRadius.Draw(tm, cam, layer);
		}

		public override void Dispose()
		{
			if (wireframe != null)
			{
				wireframe.Dispose();
			}
			if (baseRadius != null)
			{
				baseRadius.Dispose();
			}
		}

		public override void GetStatistic(ref int linesCount, ref int totallinesCount, ref int dotsCount, ref int totalDotsCount, ref int polylinesCount, ref int totalPolylineVerticesCount)
		{
			if (wireframe != null)
			{
				totallinesCount += wireframe.count;
				polylinesCount++;
				totalPolylineVerticesCount += baseRadius.count;
			}
		}
	}
}
