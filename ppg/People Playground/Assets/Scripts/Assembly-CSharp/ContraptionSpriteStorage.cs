using System;
using System.Collections.Generic;
using UnityEngine;

public static class ContraptionSpriteStorage
{
	public static Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();

	public static void Clear()
	{
		foreach (KeyValuePair<string, Sprite> sprite in Sprites)
		{
			if ((bool)sprite.Value)
			{
				UnityEngine.Object.Destroy(sprite.Value.texture);
				UnityEngine.Object.Destroy(sprite.Value);
			}
		}
		Sprites.Clear();
	}

	public static bool HasFor(ContraptionMetaData c)
	{
		return Sprites.ContainsKey(c.PathToThumbnail);
	}

	public static Sprite GetFor(ContraptionMetaData c)
	{
		if (Sprites.TryGetValue(c.PathToThumbnail, out var value))
		{
			return value;
		}
		try
		{
			Texture2D texture2D = ContraptionSerialiser.LoadThumbnail(c);
			Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
			SetFor(c, sprite);
			return sprite;
		}
		catch (Exception exception)
		{
			Debug.LogWarning("Error while creating sprite thumbnail for " + c.DisplayName);
			Debug.LogException(exception);
			SetFor(c, null);
			return null;
		}
	}

	public static void SetFor(ContraptionMetaData c, Sprite s)
	{
		if (Sprites.ContainsKey(c.PathToThumbnail))
		{
			Sprites[c.PathToThumbnail] = s;
		}
		else
		{
			Sprites.Add(c.PathToThumbnail, s);
		}
	}
}
