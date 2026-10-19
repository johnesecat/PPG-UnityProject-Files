using UnityEngine;

public class CategoryButtonBehaviour : MonoBehaviour
{
	public CatalogBehaviour CatalogBehaviour;

	public Category Category;

	public void Navigate()
	{
		CatalogBehaviour.SetCategory(Category);
	}
}
