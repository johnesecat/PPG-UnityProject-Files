using Linefy.Serialization;
using UnityEngine;

namespace Linefy.Primitives
{
	[ExecuteInEditMode]
	public class LinefyCylinder : DrawableComponent
	{
		public float radiusTop = 1f;

		public float radiusBottom = 1f;

		public float height = 1f;

		[Range(0f, 1f)]
		public float pivotOffset = 0.5f;

		[Range(3f, 256f)]
		public int radiusSegments = 32;

		[Range(0f, 256f)]
		public int radialsCount = 8;

		public SerializationData_LinesBase wireframeProperties = new SerializationData_LinesBase(3f, Color.white, 1f);

		private Cylinder _cylinder;

		private Cylinder cylinder
		{
			get
			{
				if (_cylinder == null)
				{
					_cylinder = new Cylinder();
				}
				return _cylinder;
			}
		}

		public override Drawable drawable => cylinder;

		protected override void PreDraw()
		{
			cylinder.radiusTop = radiusTop;
			cylinder.radiusBottom = radiusBottom;
			cylinder.height = height;
			cylinder.pivotOffset = pivotOffset;
			cylinder.radiusSegments = radiusSegments;
			cylinder.radialsCount = radialsCount;
			cylinder.wireframeProperties = wireframeProperties;
		}

		public static LinefyCylinder CreateInstance()
		{
			return new GameObject("New Cylinder").AddComponent<LinefyCylinder>();
		}
	}
}
