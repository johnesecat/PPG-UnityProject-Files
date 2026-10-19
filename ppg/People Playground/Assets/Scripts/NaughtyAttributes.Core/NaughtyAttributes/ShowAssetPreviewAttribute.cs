using System;

namespace NaughtyAttributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class ShowAssetPreviewAttribute : DrawerAttribute
	{
		public const int DefaultWidth = 64;

		public const int DefaultHeight = 64;

		public int Width { get; private set; }

		public int Height { get; private set; }

		public ShowAssetPreviewAttribute(int width = 64, int height = 64)
		{
			Width = width;
			Height = height;
		}
	}
}
