using UnityEngine;

[CreateAssetMenu(menuName = "PPG/Catalog")]
public class CatalogData : ScriptableObject
{
	public Category[] Categories;

	public SpawnableAsset[] Items;
}
