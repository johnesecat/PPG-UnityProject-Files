using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectionMenuBehaviour : MonoBehaviour
{
	[Obsolete]
	public Map[] Maps;

	public Image PreviewImage;

	public bool AutoAdjustContainerSize;

	[ShowIf("AutoAdjustContainerSize")]
	public float WidthPerItem = 300f;

	[ShowIf("AutoAdjustContainerSize")]
	public float SpacingPerItem = 12f;

	[Obsolete]
	public bool AutoPopulateMapList;

	[Space]
	public GameObject MapViewPrefab;

	public void RegenerateView()
	{
		RectTransform component = GetComponent<RectTransform>();
		foreach (Transform item in base.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		IEnumerable<Map> obj = ((Maps == null) ? MapRegistry.GetAllMaps() : MapRegistry.GetAllMaps().Concat(Maps));
		int num = 0;
		foreach (Map item2 in obj)
		{
			if ((bool)item2)
			{
				UnityEngine.Object.Instantiate(MapViewPrefab, Vector3.zero, Quaternion.identity, base.transform).GetComponent<MapViewBehaviour>().Map = item2;
				num++;
			}
		}
		if (AutoAdjustContainerSize)
		{
			Vector2 sizeDelta = component.sizeDelta;
			sizeDelta.x = (float)num * WidthPerItem + (float)Mathf.Max(0, num - 1) * SpacingPerItem;
			component.sizeDelta = sizeDelta;
		}
		UISoundBehaviour.Refresh();
	}

	private void OnPageSelected()
	{
		RegenerateView();
	}
}
