using System;
using Linefy.Serialization;
using UnityEngine;

namespace Linefy.Primitives
{
	[ExecuteInEditMode]
	public class LinefyCircularPolyline : DrawableComponent
	{
		private int _segmentsCount = 64;

		[Range(3f, 256f)]
		public int segmentsCount = 64;

		private float _radius;

		public float radius = 1f;

		private float _angle = 360f;

		public float angle = 360f;

		private float _offsetAngle;

		public float offsetAngle;

		private Polyline _polyline;

		public SerializationData_Polyline polylineProperties = new SerializationData_Polyline();

		private float _widthCurveTiling = 1f;

		[Header("Width")]
		public float widthCurveTiling = 1f;

		public AnimationCurve widthCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

		[Header("Transparency")]
		public float transparencyCurveTiling = 1f;

		public AnimationCurve transparencyCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

		[Header("Auto animated value")]
		public float textureOffsetSpeed;

		public float angleOffsetSpeed;

		public Polyline polyline
		{
			get
			{
				if (_polyline == null)
				{
					_polyline = new Polyline(segmentsCount, isClosed: true);
				}
				return _polyline;
			}
		}

		public override Drawable drawable => polyline;

		protected override void PreDraw()
		{
			bool flag = !Application.isPlaying;
			if (_segmentsCount != segmentsCount)
			{
				_segmentsCount = segmentsCount;
				polyline.count = segmentsCount;
				flag = true;
			}
			if (_offsetAngle != offsetAngle)
			{
				flag = true;
				_offsetAngle = offsetAngle;
			}
			if (_angle != angle)
			{
				flag = true;
				_angle = angle;
			}
			if (_radius != radius)
			{
				flag = true;
				_radius = radius;
			}
			if (_widthCurveTiling != widthCurveTiling)
			{
				flag = true;
				_widthCurveTiling = widthCurveTiling;
			}
			if (flag)
			{
				float num = 1f / (float)segmentsCount;
				float num2 = offsetAngle * ((float)Math.PI / 180f);
				float num3 = angle * ((float)Math.PI / 180f);
				for (int i = 0; i < segmentsCount; i++)
				{
					float num4 = (float)i * num;
					float f = num2 + num4 * num3;
					float x = Mathf.Cos(f) * radius;
					float y = Mathf.Sin(f) * radius;
					float a = transparencyCurve.Evaluate(num4 * transparencyCurveTiling % 1f);
					polyline[i] = new PolylineVertex(new Vector3(x, y), new Color(1f, 1f, 1f, a), widthCurve.Evaluate(num4 * widthCurveTiling % 1f), num * (float)i);
				}
				polyline.lastVertexTextureOffset = 1f;
			}
			polyline.LoadSerializationData(polylineProperties);
			if (Application.isPlaying)
			{
				polyline.textureOffset += Time.timeSinceLevelLoad * textureOffsetSpeed;
				offsetAngle = Time.timeSinceLevelLoad * angleOffsetSpeed;
			}
		}

		public static LinefyCircularPolyline CreateInstance()
		{
			return new GameObject("New CircularPolyline").AddComponent<LinefyCircularPolyline>();
		}
	}
}
