using System;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "PPG/Spawnable Asset")]
public class SpawnableAsset : ScriptableObject
{
	[TextArea(20, 100)]
	public string Description;

	public string NameToOrderBy = "";

	public bool VisibleInCatalog = true;

	[SerializeField]
	private bool Locked;

	[Space]
	public Category Category;

	[Space]
	[ShowAssetPreview(256, 256)]
	public GameObject Prefab;

	[Space]
	[ShowAssetPreview(256, 256)]
	public Sprite ViewSprite;

	[Obsolete]
	[HideInInspector]
	public Sprite DragSprite;

	[NonSerialized]
	[SkipSerialisation]
	internal ModMetaData RelevantModMetadata;

	[Space]
	public bool HasPriorName;

	[ShowIf("HasPriorName")]
	public string PriorName;

	[SkipSerialisation]
	public MigrationEvent[] MigrationEvents;

	[SkipSerialisation]
	public bool IsLocked => Locked;
}
