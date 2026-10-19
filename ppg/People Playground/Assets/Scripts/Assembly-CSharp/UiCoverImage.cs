using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class UiCoverImage : MonoBehaviour
{
	public enum ImageMode
	{
		Cover,
		Contain
	}

	public Image Image;

	public RectTransform RectTransform;

	public ImageMode Mode;

	private void Start()
	{
		ScaleImageToFit();
	}

	private void OnRectTransformDimensionsChange()
	{
		if (base.enabled && base.gameObject.activeInHierarchy)
		{
			ScaleImageToFit();
		}
	}

	private void ScaleImageToFit()
	{
		if ((bool)Image.sprite && (bool)RectTransform)
		{
			Vector2 size = RectTransform.rect.size;
			Vector2 size2 = Image.sprite.rect.size;
			float num = size2.x / size2.y;
			Vector2 sizeDelta = size;
			bool flag = size.x / num > size.y;
			if (Mode == ImageMode.Contain)
			{
				flag = !flag;
			}
			if (flag)
			{
				sizeDelta.y = size.x / num;
			}
			else
			{
				sizeDelta.x = size.y * num;
			}
			Image.rectTransform.sizeDelta = sizeDelta;
		}
	}
}
