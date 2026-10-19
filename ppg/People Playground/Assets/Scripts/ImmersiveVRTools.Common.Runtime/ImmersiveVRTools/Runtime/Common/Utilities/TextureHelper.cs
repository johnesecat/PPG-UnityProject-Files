using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public class TextureHelper
	{
		public static Texture2D CreateTexture(int width, int height, Color color)
		{
			Color[] array = new Color[width * height];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = color;
			}
			Texture2D texture2D = new Texture2D(width, height);
			texture2D.SetPixels(array);
			texture2D.Apply();
			return texture2D;
		}

		public static Texture2D CreateGuiBackgroundColor(Color color)
		{
			return CreateTexture(2, 2, color);
		}
	}
}
