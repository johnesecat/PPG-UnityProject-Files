using Linefy.Serialization;
using UnityEngine;

namespace Linefy.Primitives
{
	[ExecuteInEditMode]
	public class LinefyGrid2d : DrawableComponent
	{
		public float width = 1f;

		public float length = 1f;

		[Range(1f, 128f)]
		public int widthSegments = 4;

		[Range(1f, 128f)]
		public int lengthSegments = 4;

		public bool linePerSegment;

		public SerializationData_LinesBase wireframeProperties = new SerializationData_LinesBase(3f, Color.white, 1f);

		private Grid2d _grid;

		private Grid2d grid
		{
			get
			{
				if (_grid == null)
				{
					_grid = new Grid2d(width, length, widthSegments, lengthSegments, linePerSegment, wireframeProperties);
				}
				return _grid;
			}
		}

		public override Drawable drawable => grid;

		protected override void PreDraw()
		{
			grid.width = width;
			grid.height = length;
			grid.widthSegments = widthSegments;
			grid.heightSegments = lengthSegments;
			grid.linePerSegment = linePerSegment;
			grid.wireframeProperties = wireframeProperties;
		}

		public static LinefyGrid2d CreateInstance()
		{
			return new GameObject("New Grid2d").AddComponent<LinefyGrid2d>();
		}
	}
}
