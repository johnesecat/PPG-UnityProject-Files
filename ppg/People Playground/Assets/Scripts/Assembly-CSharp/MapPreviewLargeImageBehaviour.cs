using UnityEngine;
using UnityEngine.UI;

public class MapPreviewLargeImageBehaviour : MonoBehaviour
{
	public Image Image;

	public Sprite Fallback;

	private void Update()
	{
		Image.sprite = (MapLoaderBehaviour.CurrentMap ? MapLoaderBehaviour.CurrentMap.Preview : Fallback);
	}
}
