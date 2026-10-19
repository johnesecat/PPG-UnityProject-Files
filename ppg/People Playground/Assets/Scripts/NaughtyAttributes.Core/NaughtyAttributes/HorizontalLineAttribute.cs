using System;

namespace NaughtyAttributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
	public class HorizontalLineAttribute : DrawerAttribute
	{
		public const float DefaultHeight = 2f;

		public const EColor DefaultColor = EColor.Gray;

		public float Height { get; private set; }

		public EColor Color { get; private set; }

		public HorizontalLineAttribute(float height = 2f, EColor color = EColor.Gray)
		{
			Height = height;
			Color = color;
		}
	}
}
