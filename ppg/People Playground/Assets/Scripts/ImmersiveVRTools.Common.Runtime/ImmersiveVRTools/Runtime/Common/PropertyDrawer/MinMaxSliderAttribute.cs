using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.PropertyDrawer
{
	public class MinMaxSliderAttribute : PropertyAttribute
	{
		public float Min { get; }

		public float Max { get; }

		public MinMaxSliderAttribute(float min, float max)
		{
			Min = min;
			Max = max;
		}
	}
}
