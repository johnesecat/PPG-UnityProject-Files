using Linefy.Serialization;
using UnityEngine;

namespace Linefy.Primitives
{
	public class CircularPolyline : Drawable
	{
		private bool dSize = true;

		private bool dTopology = true;

		private int _segments = 32;

		private float _radius = 1f;

		private Texture _tex;

		private float _textureScale;

		private Polyline baseRadius;

		public SerializationData_Polyline wireframeProperties = new SerializationData_Polyline(3f, Color.white, 1f, isClosed: true);

		public int segments
		{
			get
			{
				return _segments;
			}
			set
			{
				if (_segments != value)
				{
					dTopology = true;
					_segments = value;
				}
			}
		}

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

		public CircularPolyline(int segments, float radius)
		{
			this.segments = segments;
			this.radius = radius;
		}

		public CircularPolyline(int segments, float radius, SerializationData_Polyline wireframeProperties)
		{
			this.segments = segments;
			this.radius = radius;
			this.wireframeProperties = wireframeProperties;
		}

		public CircularPolyline(int segments, float radius, Color color)
		{
			this.segments = segments;
			this.radius = radius;
			wireframeProperties.colorMultiplier = color;
		}

		private void PreDraw()
		{
			if (baseRadius == null)
			{
				baseRadius = new Polyline(_segments);
			}
			baseRadius.LoadSerializationData(wireframeProperties);
			if (dTopology)
			{
				baseRadius.count = segments;
				float num = 1f / (float)_segments;
				for (int i = 0; i < segments; i++)
				{
					baseRadius.SetTextureOffset(i, (float)i * num);
				}
				baseRadius.lastVertexTextureOffset = 1f;
				dSize = true;
				dTopology = false;
			}
			if (dSize)
			{
				float num2 = 6.283185f / (float)_segments;
				for (int j = 0; j < segments; j++)
				{
					float f = (float)j * num2;
					float x = Mathf.Cos(f) * radius;
					float y = Mathf.Sin(f) * radius;
					baseRadius.SetPosition(j, new Vector3(x, y, 0f));
				}
				dSize = false;
			}
		}

		public override void Dispose()
		{
			if (baseRadius != null)
			{
				baseRadius.Dispose();
				baseRadius.capacityChangeStep = 8;
			}
		}

		public override void Draw(Matrix4x4 matrix, Camera cam, int layer)
		{
			PreDraw();
			baseRadius.Draw(matrix, cam, layer);
		}

		public override void DrawNow(Matrix4x4 matrix)
		{
			PreDraw();
			baseRadius.DrawNow(matrix);
		}

		public override void GetStatistic(ref int linesCount, ref int totallinesCount, ref int dotsCount, ref int totalDotsCount, ref int polylinesCount, ref int totalPolylineVerticesCount)
		{
			if (baseRadius != null)
			{
				polylinesCount++;
				totalPolylineVerticesCount += baseRadius.count;
			}
		}
	}
}
