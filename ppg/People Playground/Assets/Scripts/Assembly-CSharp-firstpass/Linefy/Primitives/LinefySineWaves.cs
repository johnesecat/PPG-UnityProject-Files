using Linefy.Serialization;
using UnityEngine;

namespace Linefy.Primitives
{
	[ExecuteInEditMode]
	public class LinefySineWaves : DrawableComponent
	{
		[Range(1f, 256f)]
		public int segmentsCount = 32;

		[Range(1f, 128f)]
		public int itemsCount = 4;

		public float width = 1f;

		public float heightSpacing = 0.2f;

		public float waveHeight = 0.04f;

		public float waveLength = 0.3f;

		public float waveOffset;

		public bool centerPivot = true;

		private SineWaves _sineWaves;

		public SerializationData_LinesBase wireframeProperties = new SerializationData_LinesBase(3f, Color.white, 1f);

		private SineWaves sineWaves
		{
			get
			{
				if (_sineWaves == null)
				{
					_sineWaves = new SineWaves();
				}
				return _sineWaves;
			}
		}

		public override Drawable drawable => sineWaves;

		protected override void PreDraw()
		{
			sineWaves.segmentsCount = segmentsCount;
			sineWaves.itemsCount = itemsCount;
			sineWaves.width = width;
			sineWaves.heightSpacing = heightSpacing;
			sineWaves.waveHeight = waveHeight;
			sineWaves.waveLength = waveLength;
			sineWaves.waveOffset = waveOffset;
			sineWaves.centerPivot = centerPivot;
			sineWaves.wireframeProperties = wireframeProperties;
		}

		public static LinefySineWaves CreateInstance()
		{
			return new GameObject("New SineWaves").AddComponent<LinefySineWaves>();
		}
	}
}
