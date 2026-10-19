using System;
using UnityEngine;

namespace Linefy.Serialization
{
	[Serializable]
	public class SerializationData_Dots : SerializationData_PrimitivesGroup
	{
		[Tooltip("Enables  pixel perfect rendering mode, which ensures that the screen pixel size and defined dot size are always the same. Only works for widthMode == PixelsBillboard.")]
		public bool pixelPerfect;

		[Tooltip("The used DotsAtlas. If null then used default atlas that located in Assets/Plugins/Linefy/Resources/Default DotsAtlas")]
		public DotsAtlas atlas;

		public SerializationData_Dots()
		{
			name = "new Dots";
			widthMultiplier = 64f;
			transparent = true;
		}
	}
}
