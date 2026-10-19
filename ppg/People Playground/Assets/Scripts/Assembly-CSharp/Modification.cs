using System;
using UnityEngine;

public class Modification
{
	public SpawnableAsset OriginalItem;

	public string NameOverride;

	public string DescriptionOverride;

	public string NameToOrderByOverride;

	public Category CategoryOverride;

	public Sprite ThumbnailOverride;

	public Action<GameObject> AfterSpawn;
}
