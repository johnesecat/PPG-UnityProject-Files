using Linefy.Serialization;
using UnityEngine;

namespace Linefy.Primitives
{
	[ExecuteInEditMode]
	public class LinefyGrid3d : DrawableComponent
	{
		public float width = 1f;

		public float height = 1f;

		public float length = 1f;

		[Range(1f, 128f)]
		public int widthSegments = 4;

		[Range(1f, 128f)]
		public int heightSegments = 4;

		[Range(1f, 128f)]
		public int lengthSegments = 4;

		public SerializationData_LinesBase wireframeProperties = new SerializationData_LinesBase(3f, Color.white, 1f);

		private Grid3d _grid;

		private Grid3d grid
		{
			get
			{
				if (_grid == null)
				{
					_grid = new Grid3d(width, height, length, widthSegments, heightSegments, lengthSegments, wireframeProperties);
				}
				return _grid;
			}
		}

		public override Drawable drawable => grid;

		protected override void PreDraw()
		{
			grid.width = width;
			grid.height = height;
			grid.length = length;
			grid.widthSegments = widthSegments;
			grid.heightSegments = heightSegments;
			grid.lengthSegments = lengthSegments;
			grid.wireframeProperties = wireframeProperties;
		}

		public static LinefyGrid3d CreateInstance()
		{
			return new GameObject("New Grid3d").AddComponent<LinefyGrid3d>();
		}
	}
}
