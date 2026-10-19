using Linefy.Serialization;
using UnityEngine;

namespace Linefy.Primitives
{
	[ExecuteInEditMode]
	public class LinefyCone : DrawableComponent
	{
		public float radius = 1f;

		public float height = 1f;

		[Range(3f, 256f)]
		public int radiusSegments = 32;

		[Range(0f, 256f)]
		public int radialsCount = 8;

		[Range(0f, 1f)]
		public float pivotOffset = 0.5f;

		public SerializationData_LinesBase wireframeProperties = new SerializationData_LinesBase(3f, Color.white, 1f);

		private Cone _cone;

		private Cone cone
		{
			get
			{
				if (_cone == null)
				{
					_cone = new Cone(radius, height, radiusSegments, radialsCount, wireframeProperties);
				}
				return _cone;
			}
		}

		public override Drawable drawable => cone;

		protected override void PreDraw()
		{
			cone.radius = radius;
			cone.height = height;
			cone.radiusSegments = radiusSegments;
			cone.pivotOffset = pivotOffset;
			cone.radialsCount = radialsCount;
			cone.wireframeProperties = wireframeProperties;
		}

		public static LinefyCone CreateInstance()
		{
			return new GameObject("New Cone").AddComponent<LinefyCone>();
		}
	}
}
