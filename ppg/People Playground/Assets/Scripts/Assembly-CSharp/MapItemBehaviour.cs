using UnityEngine;
using UnityEngine.UI;

public class MapItemBehaviour : MonoBehaviour
{
	public MapEditorAsset MapEditorAsset;

	[Space]
	public Image Image;

	public HasTooltipBehaviour TooltipBehaviour;

	private void Start()
	{
		Image.sprite = MapEditorAsset.ViewSprite;
		TooltipBehaviour.Text = "<b>" + MapEditorAsset.name + "</b>\n" + MapEditorAsset.Description;
	}
}
