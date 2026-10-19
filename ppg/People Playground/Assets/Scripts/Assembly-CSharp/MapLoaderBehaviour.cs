using UnityEngine;

public class MapLoaderBehaviour : MonoBehaviour
{
	public static Map CurrentMap;

	public Map MapLoadOverride;

	private void Awake()
	{
		Load();
	}

	public void Load()
	{
		if (base.transform.childCount > 0)
		{
			foreach (Transform item in base.transform)
			{
				Object.Destroy(item.gameObject);
			}
		}
		if ((bool)MapLoadOverride && Application.isEditor)
		{
			InstantiateMap(MapLoadOverride);
		}
		if (!CurrentMap)
		{
			return;
		}
		foreach (Transform item2 in base.transform)
		{
			item2.gameObject.SetActive(value: false);
		}
		InstantiateMap(CurrentMap);
	}

	private void InstantiateMap(Map map)
	{
		if (map.InstantiateOverride != null)
		{
			map.InstantiateOverride(base.transform);
		}
		else
		{
			Object.Instantiate(map.Prefab, Vector3.zero, Quaternion.identity, base.transform);
		}
	}
}
