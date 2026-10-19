using System;
using UnityEngine;
using UnityEngine.UI;

public class ToyStatusView : MonoBehaviour
{
	public Image Image;

	public CatalogBehaviour Catalog;

	private void Start()
	{
		Catalog.SelectionChanged += Catalog_SelectionChanged;
		UpdateSprite();
	}

	private void Catalog_SelectionChanged(object sender, EventArgs e)
	{
		base.gameObject.SetActive((bool)Catalog.SelectedItem || Catalog.SelectedContraption != null);
		UpdateSprite();
	}

	private void UpdateSprite()
	{
		if ((bool)Catalog.SelectedItem)
		{
			Image.sprite = Catalog.SelectedItem.ViewSprite;
		}
		else if (Catalog.SelectedContraption != null)
		{
			Image.sprite = ContraptionSpriteStorage.GetFor(Catalog.SelectedContraptionMetaData);
		}
	}
}
