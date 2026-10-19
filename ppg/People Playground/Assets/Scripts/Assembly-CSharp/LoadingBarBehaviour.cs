using TMPro;
using UnityEngine;

public class LoadingBarBehaviour : MonoBehaviour
{
	public RectTransform background;

	public TextMeshProUGUI text;

	public void SetProgress(float progress)
	{
		background.localScale = new Vector3(progress, 1f, 1f);
		text.text = Mathf.CeilToInt(progress * 100f) + "%";
	}
}
