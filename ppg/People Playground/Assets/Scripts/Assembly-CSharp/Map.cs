using System;
using UnityEngine;

[CreateAssetMenu(menuName = "PPG/Map")]
public class Map : ScriptableObject
{
	[ContextMenuItem("Regenerate identity", "RegenerateIdentity")]
	public string UniqueIdentity;

	[Multiline]
	public string Description;

	public Sprite Thumbnail;

	public Sprite Preview;

	[Space]
	public GameObject Prefab;

	[NonSerialized]
	public Action<Transform> InstantiateOverride;

	private void RegenerateIdentity()
	{
		UniqueIdentity = Guid.NewGuid().ToString();
	}
}
