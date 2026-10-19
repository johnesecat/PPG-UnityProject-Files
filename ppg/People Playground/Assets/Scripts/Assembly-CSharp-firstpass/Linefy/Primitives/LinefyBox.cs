using Linefy.Serialization;
using UnityEngine;

namespace Linefy.Primitives
{
	[ExecuteInEditMode]
	public class LinefyBox : DrawableComponent
	{
		public float width = 1f;

		public float height = 1f;

		public float length = 1f;

		[Range(1f, 128f)]
		public int widthSegments = 1;

		[Range(1f, 128f)]
		public int heightSegments = 1;

		[Range(1f, 128f)]
		public int lengthSegments = 1;

		public SerializationData_LinesBase wireframeProperties = new SerializationData_LinesBase(3f, Color.white, 1f);

		private Box _box;

		private Box box
		{
			get
			{
				if (_box == null)
				{
					_box = new Box(width, height, length, widthSegments, heightSegments, lengthSegments, wireframeProperties);
				}
				return _box;
			}
		}

		public override Drawable drawable => box;

		protected override void PreDraw()
		{
			box.width = width;
			box.height = height;
			box.length = length;
			box.widthSegments = widthSegments;
			box.heightSegments = heightSegments;
			box.lengthSegments = lengthSegments;
			box.wireframeProperties = wireframeProperties;
		}

		public static LinefyBox CreateInstance()
		{
			return new GameObject("New Box").AddComponent<LinefyBox>();
		}
	}
}
