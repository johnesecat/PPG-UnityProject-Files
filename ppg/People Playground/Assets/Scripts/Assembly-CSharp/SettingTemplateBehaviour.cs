using TMPro;
using UnityEngine;

public class SettingTemplateBehaviour : MonoBehaviour
{
	public TextMeshProUGUI Label;

	public TextMeshProUGUI Description;

	public Transform ControlContainer;

	public SettingCategory Category;

	public void SetHeight(float h)
	{
		RectTransform component = GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(component.sizeDelta.x, h);
	}

	public void SetTitle(string title)
	{
		Label.text = title;
	}

	public void SetDescription(string description)
	{
		if (string.IsNullOrWhiteSpace(description))
		{
			Description.gameObject.SetActive(value: false);
			return;
		}
		Description.gameObject.SetActive(value: true);
		Description.text = description;
	}
}
