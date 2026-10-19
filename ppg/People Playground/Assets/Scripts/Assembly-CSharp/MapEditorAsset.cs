using UnityEngine;

[CreateAssetMenu(menuName = "PPG/Map Editor Asset")]
public class MapEditorAsset : ScriptableObject
{
	[TextArea(3, 8)]
	public string Description;

	public Sprite ViewSprite;

	public GameObject Prefab;
}
