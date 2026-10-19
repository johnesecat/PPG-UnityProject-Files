using System;
using UnityEngine;

namespace Linefy.Serialization
{
	[Serializable]
	public class SerializationData_Lines : SerializationData_LinesBase
	{
		public SerializationData_Lines()
		{
			name = "new Lines";
			transparent = true;
			feather = 2f;
			widthMultiplier = 20f;
		}

		public SerializationData_Lines(float widthMultiplier, Color color, float feather)
		{
			base.widthMultiplier = widthMultiplier;
			colorMultiplier = color;
			base.feather = feather;
			transparent = true;
		}

		public SerializationData_Lines(float widthMultiplier, Color color)
		{
			base.widthMultiplier = widthMultiplier;
			colorMultiplier = color;
			transparent = false;
		}
	}
}
