using UnityEngine;

public class MapEditorSelectionOutlineBehaviour : MonoBehaviour
{
	private MaterialPropertyBlock propertyBlock;

	private void Start()
	{
		UpdateOutline();
	}

	public void UpdateOutline()
	{
		Utils.UpdateOutlineMaterial(ref propertyBlock, GetComponent<SpriteRenderer>(), GetComponentInParent<SpriteRenderer>().sprite);
	}
}
