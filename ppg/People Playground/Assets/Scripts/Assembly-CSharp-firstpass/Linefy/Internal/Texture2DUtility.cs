using UnityEngine;

namespace Linefy.Internal
{
	public static class Texture2DUtility
	{
		public static Texture2D InvertRGB(this Texture2D source)
		{
			Texture2D texture2D = Object.Instantiate(source);
			texture2D.hideFlags = HideFlags.HideAndDontSave;
			Color[] pixels = texture2D.GetPixels();
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i].r = 1f - pixels[i].r;
				pixels[i].g = 1f - pixels[i].g;
				pixels[i].b = 1f - pixels[i].b;
			}
			texture2D.SetPixels(pixels);
			texture2D.Apply();
			return texture2D;
		}

		public static Texture2D MultiplyAlpha(this Texture2D source, float multiplier)
		{
			Texture2D texture2D = Object.Instantiate(source);
			texture2D.hideFlags = HideFlags.HideAndDontSave;
			Color[] pixels = texture2D.GetPixels();
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i].a = pixels[i].a * multiplier;
			}
			texture2D.SetPixels(pixels);
			texture2D.Apply();
			return texture2D;
		}

		public static Texture2D SolidTexture2D(int sizeX, int sizeY, Color color)
		{
			Texture2D texture2D = new Texture2D(sizeX, sizeY, TextureFormat.RGBA32, mipChain: false, linear: false);
			texture2D.hideFlags = HideFlags.HideAndDontSave;
			Color[] array = new Color[sizeX * sizeY];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = color;
			}
			texture2D.SetPixels(array);
			texture2D.Apply();
			return texture2D;
		}

		public static Texture2D GetReadableCopy(this Texture2D source)
		{
			RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
			Graphics.Blit(source, temporary);
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = temporary;
			Texture2D texture2D = new Texture2D(source.width, source.height);
			texture2D.ReadPixels(new Rect(0f, 0f, temporary.width, temporary.height), 0, 0);
			texture2D.Apply();
			RenderTexture.active = active;
			RenderTexture.ReleaseTemporary(temporary);
			return texture2D;
		}
	}
}
