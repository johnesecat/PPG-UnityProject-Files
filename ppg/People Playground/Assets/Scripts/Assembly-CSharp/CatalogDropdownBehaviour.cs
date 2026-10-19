using System.Collections.Generic;
using UnityEngine;

public class CatalogDropdownBehaviour : MonoBehaviour
{
	public List<GameObject> CategoryContainers;

	private float moveAmount;

	private float catalogY;

	private float itemY;

	private bool clicked;

	private void Awake()
	{
		moveAmount = CatalogBehaviour.Main.CategoryContainer.GetComponent<RectTransform>().sizeDelta.y;
		itemY = CatalogBehaviour.Main.ItemPanel.GetComponent<RectTransform>().offsetMax.y;
	}

	public void Drop()
	{
		clicked = !clicked;
		foreach (GameObject categoryContainer in CategoryContainers)
		{
			categoryContainer.SetActive(clicked);
		}
		if (clicked)
		{
			CatalogBehaviour.Main.ItemPanel.GetComponent<RectTransform>().offsetMax = new Vector2(0f, itemY - moveAmount * (float)((CatalogBehaviour.Main.ExtraCategoryCount + 9) / 10));
		}
		else
		{
			CatalogBehaviour.Main.ItemPanel.GetComponent<RectTransform>().offsetMax = new Vector2(0f, itemY);
		}
	}
}
