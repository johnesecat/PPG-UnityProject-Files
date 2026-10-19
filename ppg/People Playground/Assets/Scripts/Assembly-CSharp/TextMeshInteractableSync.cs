using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextMeshInteractableSync : MonoBehaviour
{
	public TextMeshProUGUI TextMesh;

	public Selectable Other;

	public Color NormalColour;

	public Color DisabledColour;

	private void Update()
	{
		TextMesh.color = (Other.IsInteractable() ? NormalColour : DisabledColour);
	}
}
