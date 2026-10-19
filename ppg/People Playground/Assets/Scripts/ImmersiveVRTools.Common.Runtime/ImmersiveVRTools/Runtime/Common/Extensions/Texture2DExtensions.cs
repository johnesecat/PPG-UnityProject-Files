using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Extensions
{
	public static class Texture2DExtensions
	{
		public static void SetColor(this Texture2D tex2, Color32 color)
		{
			Color32[] pixels = tex2.GetPixels32();
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = color;
			}
			tex2.SetPixels32(pixels);
			tex2.Apply();
		}
	}
}
